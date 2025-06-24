using Cinemachine;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    public sealed class CameraTriggerVerticalDolly : CameraTriggerDolly
    {
        private CinemachineSmoothPath.Waypoint _downWaypoint = new();
        private CinemachineSmoothPath.Waypoint _upWaypoint = new();

        protected override void Awake()
        {
            base.Awake();

            InitializeSmoothPath();
            CreateSoothPathWaypoints();

            Vector3 pathPosition = virtualCamera.transform.localPosition;
            AssignTrackedDollyComponent(pathPosition, _downWaypoint, _upWaypoint);
        }

        private void CreateSoothPathWaypoints()
        {
            float zPos = (singleRoomSize.z / 2) * ((_collider.size.z / singleRoomSize.z) - 1);

            _downWaypoint.position = new Vector3(0, 0, -zPos);
            _downWaypoint.roll = 0;

            _upWaypoint.position = new Vector3(0, 0, zPos);
            _upWaypoint.roll = 0;
        }
    }
}