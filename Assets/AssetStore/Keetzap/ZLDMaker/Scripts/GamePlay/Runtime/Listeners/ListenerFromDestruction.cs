using System.Collections.Generic;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    public class ListenerFromDestruction : Listener
    {
        public List<Destructible> destructibles = new();

        private List<Hitable> _destructibles = new();

        private void Awake()
        {
            foreach (var destructible in destructibles)
            {
                destructible.destructible.OnDestructionEvent += OnDestruction;
                _destructibles.Add(destructible.destructible);
            }
        }

        private void OnDestruction(Hitable destructible)
        {
            _destructibles.Remove(destructible);

            if (_destructibles.Count == 0)
                OnTrigger(null, null);
        }

        public override void OnTrigger(object sender, Collider other)
        {
            if (_state == TypeOfState.Enabled) return;

            base.OnTrigger(sender, other);
        }

        private void OnDestroy()
        {
            foreach (var destructible in destructibles)
            {
                destructible.destructible.OnDestructionEvent -= OnDestruction;
            }
        }
    }
}
