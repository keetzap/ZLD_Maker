using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    public sealed class CameraTriggerHorizontalDolly : CameraTriggerDolly
    {
        private Vector3 _leftWaypoint;
        private Vector3 _rightWaypoint;

        protected override void Awake()
        {
            base.Awake();

            InitializeSmoothPath();
            CreateSoothPathWaypoints();

            Vector3 camPosition = virtualCamera.transform.localPosition;
            Vector3 pathPosition = new(-camPosition.z, camPosition.y, 0);

            AssignTrackedDollyComponent(pathPosition, _leftWaypoint, _rightWaypoint);
        }

        private void CreateSoothPathWaypoints()
        {
            float xPos = (singleRoomSize.x / 2) * ((_collider.size.x / singleRoomSize.x) - 1);

            _leftWaypoint = new Vector3(-xPos, 0, 0);
            _rightWaypoint = new Vector3(xPos, 0, 0);
        }
    }
}