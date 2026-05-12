using Keetzap.Core;
using Keetzap.Utils;
using System.Collections;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    public class SimpleDropper : MonoBehaviour, IDropable
    {
        public static class Fields
        {
            public static string Drop => nameof(drop);
            public static string ItAlwaysDrops => nameof(itAlwaysDrops);
            public static string ChanceOfDropping => nameof(chanceOfDropping);
            public static string Delay => nameof(delay);
            public static string SpawnEffect => nameof(spawnEffect);
        }

        [SerializeField] private GameObject drop;
        [SerializeField] private bool itAlwaysDrops = true;
        [SerializeField] private int chanceOfDropping = 30;
        [SerializeField] private float delay;
        [SerializeField] private SpawnObjectPosition spawnEffect;

        private IHitable _hitable;
        private IInteractable _interactable;

        private void Awake()
        {
            if (TryGetComponent<IHitable>(out IHitable hitable))
            {
                _hitable = hitable;  
                _hitable.OnAttackedEvent += OnDrop;
                _hitable.OnDeadEvent += OnDead;
            }
            else if (TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                _interactable = interactable;
                _interactable.OnInteractEvent += OnInteract; 
            }
        }

        public void OnDrop(int damage, float power)
        {
            OnDrop();
        }

        public void OnDead(int damage, float power)
        {
            OnDrop();
        }

        public void OnInteract()
        {
            OnDrop();
        }

        public void OnDrop()
        {
            if (itAlwaysDrops || (Random.Range(0, 100) < chanceOfDropping))
            {
                Vector3 position = spawnEffect.anchorType == TypeOfAnchor.Transform ? spawnEffect.anchorTransform.position : transform.position + spawnEffect.anchorOffset;
                StartCoroutine(DropObject(position));
            }
        }

        private IEnumerator DropObject(Vector3 position)
        {
            yield return new WaitForSeconds(delay);
            _ = (GameObject)Instantiate(drop, position, Quaternion.identity);
        }

        private void OnDestroy()
        {
            if (_hitable !=null)
            {
                _hitable.OnAttackedEvent -= OnDrop;
                _hitable.OnDeadEvent -= OnDead;
            }
            else if (_interactable != null)
            {
                _interactable.OnInteractEvent -= OnInteract;
            }
        }
    }
}