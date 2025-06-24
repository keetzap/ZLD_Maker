using System;
using System.Collections.Generic;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    public class TriggerByProximity : Trigger
    {
        new public static class Fields
        {
            public static string TypeOfTriggerBehaviour => nameof(typeOfTriggerBehaviour);
            public static string OneSingleUse => nameof(oneSingleUse);
            public static string CanBeEnabledByEnemies => nameof(canBeEnabledByEnemies);
        }

        public enum TypeOfTriggerBehaviour
        {
            TriggerOnExit,
            TriggerOnEnter,
            AlwaysTrigger
        }

        [SerializeField] private TypeOfTriggerBehaviour typeOfTriggerBehaviour;
        [SerializeField] private bool oneSingleUse;
        [SerializeField] private bool canBeEnabledByEnemies;

        private Coroutine _startCountdown;
        private List<Collider> _colliders = new();

        public void OnTriggerEnter(Collider other)
        {
            if (_hasBeenUsed || typeOfTriggerBehaviour == TypeOfTriggerBehaviour.TriggerOnExit) return;

            if (other.CompareTag(StringsData.PLAYER) || other.GetComponent<IDraggable>() != null || (other.CompareTag(StringsData.ENEMY) && canBeEnabledByEnemies))
            {
                _colliders.Add(other);

                if (_colliders.Count == 1)
                {
                    if (_startCountdown != null)
                    {
                        StopCoroutine(_startCountdown);
                    }

                    OnTrigger(other);
                }
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (_hasBeenUsed || typeOfTriggerBehaviour == TypeOfTriggerBehaviour.TriggerOnEnter) return;

            _colliders.Remove(other);

            if (_colliders.Count == 0 || (other.CompareTag(StringsData.ENEMY) && canBeEnabledByEnemies))
            {
                OnTrigger(other);
            }
        }

        private void OnTrigger(Collider other)
        {
            OnTrigger(Collider, other);
            
            if (oneSingleUse)
            {
                _hasBeenUsed = true;
            }
        }
    }   
}
