using System.Collections.Generic;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    [RequireComponent(typeof(Collider))]
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private GD_Weapon weapon;

        private Collider _collider;
        private List<IHitable> _destructibles = new();

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            EnableCollider(false);
        }

        public void EnableCollider(bool state)
        {
            _collider.enabled = state;

            if (state == false)
                _destructibles.Clear();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IHitable>(out IHitable destructible))
            {
                if (!_destructibles.Contains(destructible))
                {
                    _destructibles.Add(destructible);
                    destructible.OnAttacked(weapon.damage, weapon.power);
                }
            }
        }
    }
}
