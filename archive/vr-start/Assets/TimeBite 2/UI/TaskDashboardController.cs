using System.Collections.Generic;
using TimeBite.VRStart.Models;
using TimeBite.VRStart.Services;
using UnityEngine;

namespace TimeBite.VRStart.UI
{
    public sealed class TaskDashboardController : MonoBehaviour
    {
        [SerializeField] private TimeBiteApiClient apiClient;
        [SerializeField] private Transform rowsRoot;
        [SerializeField] private TaskRowView rowPrefab;
        [SerializeField] private ProgressRingView progressRing;

        private readonly List<TaskRowView> visibleRows = new();

        public void Configure(TimeBiteApiClient client, Transform root, TaskRowView prefab, ProgressRingView ring)
        {
            apiClient = client;
            rowsRoot = root;
            rowPrefab = prefab;
            progressRing = ring;
        }

        private void Start()
        {
            Refresh();
        }

        public void Refresh()
        {
            if (apiClient == null)
            {
                Debug.LogError("TaskDashboardController needs a TimeBiteApiClient.");
                return;
            }

            StartCoroutine(apiClient.GetTasks(RenderTasks, Debug.LogError));
        }

        public void AddTaskFromAffordance()
        {
            var request = new ProcessRequest
            {
                user_utterance = "Add a task from the Quest dashboard",
                transcribed_text = "Add a task from the Quest dashboard",
                task = "Task from Quest dashboard",
                title = "Task from Quest dashboard",
                priority = 1,
                status = "pending",
                spatial_action_type = "task_create",
                ui_element_targeted = "add-task-affordance",
                ui_element_selected = true,
                gesture_type = "poke",
                gaze_target = "task-dashboard",
                interaction_distance = 0.35f,
                hit_accuracy = 1f,
                transaction_type = "task_creation",
                argument_validation_passed = true,
                step_sequence_correctness = 1f
            };

            StartCoroutine(apiClient.RecordInteractionTelemetry(request, _ => Refresh(), Debug.LogError));
        }

        private void RenderTasks(IReadOnlyList<TaskItem> tasks)
        {
            foreach (var row in visibleRows)
            {
                if (row != null)
                {
                    Destroy(row.gameObject);
                }
            }
            visibleRows.Clear();

            var completed = 0;
            foreach (var task in tasks)
            {
                if (task.status == "done" || task.status == "completed")
                {
                    completed++;
                }

                if (rowPrefab != null && rowsRoot != null)
                {
                    var row = Instantiate(rowPrefab, rowsRoot);
                    row.gameObject.SetActive(true);
                    row.transform.localPosition = new Vector3(0f, -visibleRows.Count * 86f, 0f);
                    row.Bind(task);
                    visibleRows.Add(row);
                }
            }

            if (progressRing != null)
            {
                progressRing.SetProgress(tasks.Count == 0 ? 0f : completed / (float)tasks.Count);
            }
        }
    }
}
