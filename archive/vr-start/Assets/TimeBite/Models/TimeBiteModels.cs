using System;
using System.Collections.Generic;

namespace TimeBite.VRStart.Models
{
    [Serializable]
    public sealed class HealthResponse
    {
        public string status;
        public string storage;
        public string instance;
    }

    [Serializable]
    public sealed class TaskItem
    {
        public string id;
        public string title;
        public int priority;
        public string status;
    }

    [Serializable]
    public sealed class TaskList
    {
        public List<TaskItem> tasks = new();
    }

    [Serializable]
    public sealed class ProcessRequest
    {
        public string user_utterance;
        public string transcribed_text;
        public string extracted_text;
        public string task;
        public string title;
        public int priority;
        public string status;
        public string gaze_target;
        public string gesture_type;
        public float interaction_distance;
        public float hit_accuracy = 1f;
        public string spatial_action_type;
        public string ui_element_targeted;
        public bool ui_element_selected;
        public string transaction_type;
        public bool argument_validation_passed = true;
        public float step_sequence_correctness = 1f;
    }

    [Serializable]
    public sealed class ProcessResponse
    {
        public string status;
        public string message;
        public string task_filename;
        public TelemetryEnvelope telemetry;
    }

    [Serializable]
    public sealed class TelemetryEnvelope
    {
        public long timestamp;
        public string user_utterance;
        public string transcribed_text;
        public string parsed_intent;
        public string function_call_name;
        public string function_call_args;
        public int processing_latency_ms;
        public bool task_success;
        public string error_type;
        public int retry_count;
        public string spatial_action_type;
        public string ui_element_targeted;
        public bool ui_element_selected;
        public string gaze_target;
        public string gesture_type;
        public float interaction_distance;
        public float hit_accuracy;
        public string transaction_type;
        public bool argument_validation_passed;
        public float step_sequence_correctness;
        public string task_filename;
    }
}
