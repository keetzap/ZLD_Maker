using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    public class WeaponsManager : MonoBehaviour
    {
        public Transform leftHand;
        public Transform rightHand;
        public Transform leftHandAnchor;
        public Transform rightHandAnchor;

        private void Awake()
        {
            leftHand.transform.parent = leftHandAnchor.transform;
            rightHand.transform.parent = rightHandAnchor.transform;
        }

        public void SetVisibility(bool state)
        {
            leftHand.gameObject.SetActive(state);
            rightHand.gameObject.SetActive(state);
        }

        public void EnablingColliders(bool state)
        {
            if (leftHand.GetComponentInChildren<Weapon>() != null)
            {
                leftHand.GetComponentInChildren<Weapon>().EnableCollider(state);
            }

            if (rightHand.GetComponentInChildren<Weapon>() != null)
            {
                rightHand.GetComponentInChildren<Weapon>().EnableCollider(state);
            }
        }
    }
}
