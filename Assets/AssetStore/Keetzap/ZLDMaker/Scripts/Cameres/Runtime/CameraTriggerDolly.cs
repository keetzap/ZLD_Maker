using UnityEngine;
//using Cinemachine;
/*
namespace Keetzap.ZeldaMaker
{
    public class CameraTriggerDolly : CameraTriggerRoomBase
    {
        new public static class Fields
        {
            public static string DollyTrack => nameof(dollyTrack);
            public static string StartingPoint => nameof(startingPoint);
        }

        [SerializeField] protected GameObject dollyTrack;
        [SerializeField] protected float startingPoint;

        protected CinemachineSmoothPath smoothPath;
        
        protected override void Start()
        {
            base.Start();

            virtualCamera.m_Follow = PlayerController.Instance.gameObject.transform;
        }

        protected void InitializeSmoothPath()
        {
            smoothPath = dollyTrack.GetComponent<CinemachineSmoothPath>();
            smoothPath.m_Waypoints = new CinemachineSmoothPath.Waypoint[2];
        }

        protected void AssignTrackedDollyComponent(Vector3 pathOffset, CinemachineSmoothPath.Waypoint waypointA, CinemachineSmoothPath.Waypoint waypointB)
        {
            CinemachineTrackedDolly cinemachineTrackedDolly = virtualCamera.AddCinemachineComponent<CinemachineTrackedDolly>();
            cinemachineTrackedDolly.m_Path = smoothPath;
            cinemachineTrackedDolly.m_PathPosition = startingPoint;
            cinemachineTrackedDolly.m_PathOffset = pathOffset;
            cinemachineTrackedDolly.m_AutoDolly.m_Enabled = true;

            smoothPath.m_Waypoints[0] = waypointA;
            smoothPath.m_Waypoints[1] = waypointB;
        }
    }
}*/