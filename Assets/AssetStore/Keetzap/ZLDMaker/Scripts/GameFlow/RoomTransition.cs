using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    [RequireComponent(typeof(BoxCollider))]
    public class RoomTransition : MonoBehaviour
    {
        public static class FieldNames
        {
            public static string SpawnPoints => nameof(spawnPoints);
            public static string TransitionSpeed => nameof(transitionSpeed);
            public static string ShowGizmos => nameof(showGizmos);
            public static string DetectionColor => nameof(detectionColor);
            public static string Opacity => nameof(opacity);
            public static string SpawnColor => nameof(spawnColor);
            public static string SpawnRadius => nameof(spawnRadius);
        }

        [SerializeField] private float spawnPoints = 2;
        [SerializeField] private float transitionSpeed = 10;

        [SerializeField] private bool showGizmos = true;
        [SerializeField] private Color detectionColor = Color.blue;
        [SerializeField] private float opacity = 0.7f;
        [SerializeField] private Color spawnColor = new Color(0, 0.35f, 1);
        [SerializeField] private float spawnRadius = 0.2f;

        public BoxCollider Collider => GetComponent<BoxCollider>();

        private Vector3 _destination;
        private Transform _pointA, _pointB;

        public void Awake()
        {
            Collider.isTrigger = true;
            _pointA = CreateTransforms("PointA", 1);
            _pointB = CreateTransforms("PointB", -1);
        }

        private Transform CreateTransforms(string point, int mult)
        {
            var p = new GameObject(point).transform;
            p.transform.parent = transform;
            p.transform.localPosition += new Vector3(spawnPoints * mult, 0, 0);
            p.transform.position += transform.position;
            p.transform.rotation *= transform.rotation;

            return p;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(StringsData.PLAYER))
            {
                var characterPosition = PlayerController.Instance.Position;
                PlayerController.Instance.PauseInteractions();

                float distSpawnA = Vector3.Distance(characterPosition, _pointA.position);
                float distSpawnB = Vector3.Distance(characterPosition, _pointB.position);

                _destination = distSpawnA > distSpawnB ? _pointA.position : _pointB.position;
                _destination.y -= transform.position.y - characterPosition.y;

                PlayerController.Instance.MoveToTargetPosition(_destination, transitionSpeed);
            }
        }

        public Vector3 GetLocalPosition(float spawnPoint)
        {
            Vector3 v = new Vector3(spawnPoint, 0, 0);
            return Quaternion.AngleAxis(transform.localEulerAngles.y, Vector3.up) * v;
        }

        private void OnDrawGizmos()
        {
            if (!showGizmos) return;

            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Vector3 center = Collider.center;
            Vector3 size = Collider.size;

            Gizmos.color = new(detectionColor.r, detectionColor.g, detectionColor.b, opacity);
            Gizmos.DrawCube(center, size);

            Gizmos.color = detectionColor;
            Gizmos.DrawWireCube(center, size);
            
            DrawSpawn(spawnPoints);
            DrawSpawn(-spawnPoints);

            Gizmos.matrix = Matrix4x4.identity;
        }

        private void DrawSpawn(float pos)
        {
            var _pos = new Vector3(pos, 0, 0);
            Gizmos.color = new(spawnColor.r, spawnColor.g, spawnColor.b, spawnColor.a);
            Gizmos.DrawSphere(_pos, spawnRadius);
            Gizmos.color = spawnColor;
            Gizmos.DrawLine(_pos, Collider.center);
        }
    }   
}
