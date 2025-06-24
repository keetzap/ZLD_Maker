using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    public class HandController : MonoBehaviour
    {
        public Weapon DefaultWeapon { get; set; }

        public List<Weapon> weapons;

        private void Awake()
        {
            for (int w = 0; w < transform.childCount; w++)
            {
                weapons.Add(transform.GetChild(w).GetComponent<Weapon>());
            }
        }
    }
}
