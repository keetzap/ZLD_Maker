using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    public sealed class CameraTriggerVerticalDolly : CameraTriggerDolly
    {
        private Vector3 _downWaypoint;
        private Vector3 _upWaypoint;

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

            _downWaypoint = new Vector3(0, 0, -zPos);
            _upWaypoint = new Vector3(0, 0, zPos);
        }
    }
}