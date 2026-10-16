using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TimeBite.VRStart.Config;
using TimeBite.VRStart.Models;
using UnityEngine;
using UnityEngine.Networking;

namespace TimeBite.VRStart.Services
{
    public sealed class TimeBiteApiClient : MonoBehaviour
    {
        [SerializeField] private TimeBiteApiConfig config;

        public IEnumerator GetHealth(Action<HealthResponse> onSuccess, Action<string> onError)
        {
            if (UseMockData())
            {
                onSuccess?.Invoke(new HealthResponse { status = "healthy", instance = "Editor Mock", storage = "mock" });
                yield break;
            }

            yield return SendGet("/health", onSuccess, onError);
        }

        public IEnumerator GetTasks(Action<IReadOnlyList<TaskItem>> onSuccess, Action<string> onError)
        {
            if (UseMockData())
            {
                onSuccess?.Invoke(MockTasks());
                yield break;
            }

            yield return SendGetRaw("/tasks", body =>
            {
                var wrapped = JsonUtility.FromJson<TaskList>("{\"tasks\":" + body + "}");
                onSuccess?.Invoke(wrapped.tasks);
            }, onError);
        }

        public IEnumerator Process(ProcessRequest payload, Action<ProcessResponse> onSuccess, Action<string> onError)
        {
            if (UseMockData())
            {
                onSuccess?.Invoke(new ProcessResponse
                {
                    status = "success",
                    task_filename = "task_editor_mock.json",
                    telemetry = new TelemetryEnvelope
                    {
                        function_call_name = "process_task",
                        task_success = true,
                        processing_latency_ms = 12,
                        spatial_action_type = payload.spatial_action_type,
                        hit_accuracy = payload.hit_accuracy
                    }
                });
                yield break;
            }

            yield return SendPost("/process", payload, onSuccess, onError);
        }

        public IEnumerator RecordInteractionTelemetry(ProcessRequest payload, Action<ProcessResponse> onSuccess, Action<string> onError)
        {
            var stopwatch = Stopwatch.StartNew();
            yield return Process(payload, response =>
            {
                stopwatch.Stop();
                if (response.telemetry != null)
                {
                    response.telemetry.processing_latency_ms = Mathf.Max(response.telemetry.processing_latency_ms, (int)stopwatch.ElapsedMilliseconds);
                }
                onSuccess?.Invoke(response);
            }, onError);
        }

        private IEnumerator SendGet<T>(string path, Action<T> onSuccess, Action<string> onError)
        {
            yield return SendGetRaw(path, body => onSuccess?.Invoke(JsonUtility.FromJson<T>(body)), onError);
        }

        private IEnumerator SendGetRaw(string path, Action<string> onSuccess, Action<string> onError)
        {
            using var request = UnityWebRequest.Get(config.BaseUrl + path);
            request.timeout = config.RequestTimeoutSeconds;
            yield return request.SendWebRequest();
            Complete(request, () => onSuccess?.Invoke(request.downloadHandler.text), onError);
        }

        private IEnumerator SendPost<T>(string path, ProcessRequest payload, Action<T> onSuccess, Action<string> onError)
        {
            var json = JsonUtility.ToJson(payload);
            using var request = new UnityWebRequest(config.BaseUrl + path, UnityWebRequest.kHttpVerbPOST);
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = config.RequestTimeoutSeconds;
            yield return request.SendWebRequest();
            Complete(request, () => onSuccess?.Invoke(JsonUtility.FromJson<T>(request.downloadHandler.text)), onError);
        }

        private static void Complete(UnityWebRequest request, Action onSuccess, Action<string> onError)
        {
            if (request.result == UnityWebRequest.Result.Success && request.responseCode >= 200 && request.responseCode < 300)
            {
                onSuccess?.Invoke();
                return;
            }

            onError?.Invoke($"{request.responseCode}: {request.error}\n{request.downloadHandler?.text}");
        }

        private bool UseMockData()
        {
#if UNITY_EDITOR
            return config == null || config.UseMockDataInEditor;
#else
            return false;
#endif
        }

        private static IReadOnlyList<TaskItem> MockTasks()
        {
            return new[]
            {
                new TaskItem { id = "mock-1", title = "Review planner capture", priority = 2, status = "in_progress" },
                new TaskItem { id = "mock-2", title = "Confirm Quest telemetry", priority = 1, status = "pending" },
                new TaskItem { id = "mock-3", title = "Send sprint summary", priority = 0, status = "done" }
            };
        }
    }
}
