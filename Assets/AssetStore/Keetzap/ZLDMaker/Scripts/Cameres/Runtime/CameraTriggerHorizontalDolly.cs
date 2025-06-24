using UnityEngine;
/*
using Cinemachine;

namespace Keetzap.ZeldaMaker
{
    public sealed class CameraTriggerHorizontalDolly : CameraTriggerDolly
    {
        private CinemachineSmoothPath.Waypoint _leftWaypoint = new();
        private CinemachineSmoothPath.Waypoint _rightWaypoint = new();

        protected override void Awake()
        {
            base.Awake();

            InitializeSmoothPath();
            CreateSoothPathWaypoints();

            Vector3 camPosition = virtualCamera.transform.localPosition;
            Vector3 pathPosition = new Vector3(-camPosition.z, camPosition.y, 0);

            AssignTrackedDollyComponent(pathPosition, _leftWaypoint, _rightWaypoint);
        }

        private void CreateSoothPathWaypoints()
        {
            float xPos = (singleRoomSize.x / 2) * ((_collider.size.x / singleRoomSize.x) - 1);

            _leftWaypoint.position = new Vector3(-xPos, 0, 0);
            _leftWaypoint.roll = 0;

            _rightWaypoint.position = new Vector3(xPos, 0, 0);
            _rightWaypoint.roll = 0;
        }
    }
}*/