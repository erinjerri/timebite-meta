using UnityEngine;

namespace TimeBite.VRStart.Interaction
{
    public sealed class HandsOnlyInteractionRig : MonoBehaviour
    {
        [SerializeField] private GameObject leftHandPokeInteractor;
        [SerializeField] private GameObject rightHandPokeInteractor;
        [SerializeField] private GameObject leftHandFarRayInteractor;
        [SerializeField] private GameObject rightHandFarRayInteractor;
        [SerializeField] private GameObject controllerRoot;

        private void Awake()
        {
            SetActive(leftHandPokeInteractor, true);
            SetActive(rightHandPokeInteractor, true);
            SetActive(leftHandFarRayInteractor, true);
            SetActive(rightHandFarRayInteractor, true);
            SetActive(controllerRoot, false);
        }

        private static void SetActive(GameObject target, bool active)
        {
            if (target != null)
            {
                target.SetActive(active);
            }
        }
    }
}
