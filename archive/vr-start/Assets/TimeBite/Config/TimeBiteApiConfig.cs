using UnityEngine;

namespace TimeBite.VRStart.Config
{
    [CreateAssetMenu(fileName = "TimeBiteApiConfig", menuName = "TimeBite/API Config")]
    public sealed class TimeBiteApiConfig : ScriptableObject
    {
        [SerializeField] private string baseUrl = "https://example.invalid";
        [SerializeField] private bool useMockDataInEditor = true;
        [SerializeField] private int requestTimeoutSeconds = 10;

        public string BaseUrl => baseUrl.TrimEnd('/');
        public bool UseMockDataInEditor => useMockDataInEditor;
        public int RequestTimeoutSeconds => Mathf.Max(1, requestTimeoutSeconds);
    }
}
