using System;
using System.Collections.Generic;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    public class RespawnSystem : MonoBehaviour
    {
        public static class Fields
        {
            public static string CurrentRespawnPoint => nameof(currentRespawnPoint);
            public static string RespawningPoints => nameof(respawningPoints);
        }

        [Serializable]
        public struct RespawningPoint
        {
            public RespawnPoint respawnPoint;
        }

        [SerializeField] private RespawnPoint currentRespawnPoint;
        [SerializeField] private List<RespawningPoint> respawningPoints = new();
        
        public Transform GetCurrentRespawnPoint
        {
            get => currentRespawnPoint.transform;
        }

        private void Awake()
        {
            foreach (var respawnLevelPoint in respawningPoints) 
            {
                respawnLevelPoint.respawnPoint.OnDropByEvent += SetCurrentRespawnPoint;
            }

            #if UNITY_EDITOR
            if (currentRespawnPoint == null && respawningPoints.Count > 0)
            {
                currentRespawnPoint = respawningPoints[0].respawnPoint;
            }
            #endif        
        }

        private void SetCurrentRespawnPoint(RespawnPoint respawnLevelPoint)
        {
            currentRespawnPoint = respawnLevelPoint;
        }

        public void AddRespawnPoints()
        {
            RemoveRespawnPoints();

            RespawnPoint[] points = GetComponentsInChildren<RespawnPoint>();

            foreach (var point in points)
            {
                RespawningPoint respawningPoint = new()
                {
                    respawnPoint = point
                };

                respawningPoints.Add(respawningPoint);
            }
        }

        public void RemoveRespawnPoints() => respawningPoints.Clear();
    }
}
