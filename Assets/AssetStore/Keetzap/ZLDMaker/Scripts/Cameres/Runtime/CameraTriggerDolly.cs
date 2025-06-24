using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Splines;

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

        protected SplineContainer spline;
        
        protected override void Start()
        {
            base.Start();

            virtualCamera.Follow = PlayerController.Instance.gameObject.transform;
        }

        protected void InitializeSmoothPath()
        {
            spline = dollyTrack.GetComponent<SplineContainer>();
        }

        protected void AssignTrackedDollyComponent(Vector3 pathOffset, Vector3 waypointA, Vector3 waypointB)
        {
            CinemachineSplineDolly cinemachineSplineDolly = virtualCamera.GetComponent<CinemachineSplineDolly>();
            cinemachineSplineDolly.Spline = spline;
            cinemachineSplineDolly.CameraPosition = startingPoint;
            cinemachineSplineDolly.SplineOffset = pathOffset;
            cinemachineSplineDolly.AutomaticDolly.Enabled = true;

            spline.Splines[0].Add(new BezierKnot(waypointA));
            spline.Splines[0].Add(new BezierKnot(waypointB));
        }
    }
}