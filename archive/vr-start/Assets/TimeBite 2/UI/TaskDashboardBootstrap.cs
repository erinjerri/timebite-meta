using TimeBite.VRStart.Services;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TimeBite.VRStart.UI
{
    public sealed class TaskDashboardBootstrap : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void BootstrapTaskDashboard()
        {
            if (SceneManager.GetActiveScene().name != "TaskDashboard")
            {
                return;
            }

            var root = new GameObject("TaskDashboard Runtime UI");
            var bootstrap = root.AddComponent<TaskDashboardBootstrap>();
            bootstrap.Build(root.transform);
        }

        private void Build(Transform root)
        {
            var apiClient = root.gameObject.AddComponent<TimeBiteApiClient>();
            var controller = root.gameObject.AddComponent<TaskDashboardController>();

            var canvasObject = new GameObject("Hands-First Dashboard Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(root, false);
            canvasObject.transform.localPosition = new Vector3(0f, 1.45f, 1.35f);
            canvasObject.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            canvasObject.transform.localScale = Vector3.one * 0.0025f;

            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.pixelRect = new Rect(0f, 0f, 900f, 620f);

            var panel = CreatePanel(canvasObject.transform, "Dashboard Panel", new Vector2(900f, 620f), new Color(0.06f, 0.07f, 0.09f, 0.92f));
            var title = CreateText(panel.transform, "TimeBite", 42, FontStyles.Bold, new Vector2(-385f, 250f), new Vector2(320f, 60f));
            title.alignment = TextAlignmentOptions.Left;

            var rowsRoot = new GameObject("Task Rows").transform;
            rowsRoot.SetParent(panel.transform, false);
            rowsRoot.localPosition = new Vector3(0f, 60f, 0f);

            var rowPrefab = CreateRowPrefab(panel.transform);
            rowPrefab.gameObject.SetActive(false);

            var ring = CreateProgressRing(panel.transform);
            var addButton = CreateAddButton(panel.transform);
            addButton.onClick.AddListener(controller.AddTaskFromAffordance);

            controller.Configure(apiClient, rowsRoot, rowPrefab, ring);
        }

        private static RectTransform CreatePanel(Transform parent, string name, Vector2 size, Color color)
        {
            var obj = new GameObject(name, typeof(RectTransform), typeof(Image));
            obj.transform.SetParent(parent, false);
            var rect = obj.GetComponent<RectTransform>();
            rect.sizeDelta = size;
            obj.GetComponent<Image>().color = color;
            return rect;
        }

        private static TaskRowView CreateRowPrefab(Transform parent)
        {
            var row = CreatePanel(parent, "Task Row Prefab", new Vector2(720f, 72f), new Color(0.13f, 0.15f, 0.18f, 1f));
            var title = CreateText(row, "Task title", 24, FontStyles.Bold, new Vector2(-230f, 8f), new Vector2(360f, 42f));
            var priority = CreateText(row, "High", 18, FontStyles.Normal, new Vector2(140f, 8f), new Vector2(110f, 36f));
            var status = CreateText(row, "pending", 18, FontStyles.Normal, new Vector2(285f, 8f), new Vector2(150f, 36f));
            var view = row.gameObject.AddComponent<TaskRowView>();
            view.Configure(title, priority, status);
            return view;
        }

        private static ProgressRingView CreateProgressRing(Transform parent)
        {
            var ringRoot = CreatePanel(parent, "Progress Ring", new Vector2(120f, 120f), new Color(0.1f, 0.12f, 0.15f, 1f));
            ringRoot.anchoredPosition = new Vector2(330f, 230f);
            var fillObject = new GameObject("Ring Fill", typeof(RectTransform), typeof(Image));
            fillObject.transform.SetParent(ringRoot, false);
            var fill = fillObject.GetComponent<Image>();
            fill.color = new Color(0.24f, 0.78f, 0.62f, 1f);
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Radial360;
            fill.fillAmount = 0f;
            fillObject.GetComponent<RectTransform>().sizeDelta = new Vector2(96f, 96f);
            var view = ringRoot.gameObject.AddComponent<ProgressRingView>();
            view.Configure(fill);
            return view;
        }

        private static Button CreateAddButton(Transform parent)
        {
            var buttonRoot = CreatePanel(parent, "Add Task Affordance", new Vector2(180f, 58f), new Color(0.24f, 0.78f, 0.62f, 1f));
            buttonRoot.anchoredPosition = new Vector2(250f, -250f);
            var button = buttonRoot.gameObject.AddComponent<Button>();
            var label = CreateText(buttonRoot, "Add task", 22, FontStyles.Bold, Vector2.zero, new Vector2(150f, 42f));
            label.color = Color.black;
            return button;
        }

        private static TMP_Text CreateText(Transform parent, string text, int size, FontStyles style, Vector2 position, Vector2 bounds)
        {
            var obj = new GameObject(text, typeof(RectTransform), typeof(TextMeshProUGUI));
            obj.transform.SetParent(parent, false);
            var rect = obj.GetComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = bounds;
            var label = obj.GetComponent<TMP_Text>();
            label.text = text;
            label.fontSize = size;
            label.fontStyle = style;
            label.color = Color.white;
            label.alignment = TextAlignmentOptions.Center;
            return label;
        }
    }
}
