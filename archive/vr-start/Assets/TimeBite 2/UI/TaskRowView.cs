using TimeBite.VRStart.Models;
using TMPro;
using UnityEngine;

namespace TimeBite.VRStart.UI
{
    public sealed class TaskRowView : MonoBehaviour
    {
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text priority;
        [SerializeField] private TMP_Text status;

        public void Configure(TMP_Text titleText, TMP_Text priorityText, TMP_Text statusText)
        {
            title = titleText;
            priority = priorityText;
            status = statusText;
        }

        public void Bind(TaskItem task)
        {
            if (title != null)
            {
                title.text = task.title;
            }

            if (priority != null)
            {
                priority.text = PriorityLabel(task.priority);
            }

            if (status != null)
            {
                status.text = task.status;
            }
        }

        private static string PriorityLabel(int value)
        {
            return value switch
            {
                >= 2 => "High",
                1 => "Medium",
                _ => "Low"
            };
        }
    }
}
