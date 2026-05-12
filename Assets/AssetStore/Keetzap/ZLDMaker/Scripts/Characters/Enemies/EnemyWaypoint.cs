using System;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    [Serializable]
    public class EnemyWaypoint
    {
        public Vector3 position;
        public bool waitingTime;
        public Vector2 timeOnWaypoint = new(1, 2);

        public float TimeOnWaypoint
        {
            get
            {
                return waitingTime ? UnityEngine.Random.Range(timeOnWaypoint.x, timeOnWaypoint.y) : 0;
            }
        }
    }
}
