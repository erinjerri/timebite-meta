using UnityEngine;
using UnityEngine.UI;

namespace TimeBite.VRStart.UI
{
    public sealed class ProgressRingView : MonoBehaviour
    {
        [SerializeField] private Image ringFill;

        public void Configure(Image fill)
        {
            ringFill = fill;
        }

        public void SetProgress(float progress)
        {
            if (ringFill != null)
            {
                ringFill.fillAmount = Mathf.Clamp01(progress);
            }
        }
    }
}
