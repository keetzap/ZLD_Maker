using UnityEngine;
//using Cinemachine;
/*
namespace Keetzap.ZeldaMaker
{
    public class CameraTriggerRoomBase : MonoBehaviour
    {
        public static class Fields
        {
            public static string VirtualCamera => nameof(virtualCamera);
            public static string BlackLid => nameof(blackLid);
            public static string BlackFrame => nameof(blackFrame);
            public static string FadeThreshold => nameof(fadeThreshold);
            public static string FrameDistance => nameof(frameDistance);
            public static string OffsetFrame => nameof(offsetFrame);
            public static string UseCustomBlackFrame => nameof(useCustomBlackFrame);
            public static string CustomBlackFrame => nameof(customBlackFrame);
            public static string EnteringFadeDuration => nameof(enteringFadeDuration);
            public static string ExitingFadeDuration => nameof(exitingFadeDuration);
        }

        protected readonly Vector3 singleRoomSize = new(14, 2.1f, 10);

        [SerializeField] protected CinemachineVirtualCamera virtualCamera;
        [SerializeField] private GameObject blackLid;
        [SerializeField] private GameObject blackFrame;
        [SerializeField] private float fadeThreshold = 0.5f;
        [SerializeField] private float frameDistance = 1;
        [SerializeField] private float offsetFrame = 0.05f;
        [SerializeField] private bool useCustomBlackFrame;
        [SerializeField] private GameObject customBlackFrame;

        [SerializeField] private float enteringFadeDuration = 1;
        [SerializeField] private float exitingFadeDuration = 0.5f;

        private GameObject _blackFrame;
        private Mesh _mesh;
        private Color[] _colors;

        private bool _isEntering;
        private bool _isExiting;
        private float _fade = 1;
        private float _delta;

        private Vector3 _bottomLeft;
        private Vector3 _upLeft;
        private Vector3 _bottomRight;
        private Vector3 _upRight;

        protected BoxCollider _collider;

        protected virtual void Awake()
        {
            virtualCamera.gameObject.SetActive(false);
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;

            _fade = 1;

            SetVertices();
            CreateLid();

            _blackFrame = useCustomBlackFrame ? customBlackFrame : CreateFrame();
            _blackFrame.SetActive(true);
        }

        protected virtual void Start() {}

        private void SetVertices()
        {
            Vector3 size = _collider.size / 2 + _collider.center;

            _upLeft = new Vector3(-size.x, size.y, size.z);
            _upRight = size;
            _bottomRight = new Vector3(size.x, size.y, -size.z);
            _bottomLeft = new Vector3(-size.x, size.y, -size.z);
        }

        private void CreateLid()
        {
            Vector3[] vertices = new Vector3[4];
            int[] tris = new int[6];

            _colors = new Color[4];

            vertices[0] = _upLeft;
            vertices[1] = _upRight;
            vertices[2] = _bottomRight;
            vertices[3] = _bottomLeft;

            int[] trisOrder = new int[6] { 0, 1, 2, 2, 3, 0 };

            for (int t = 0; t < tris.Length; t++)
            {
                tris[t] = trisOrder[t];
            }

            for (int i = 0; i < 4; i++)
            {
                _colors[i] = Color.black;
            }

            _mesh = new();

            _mesh.vertices = vertices;
            _mesh.colors = _colors;
            _mesh.uv = new Vector2[4];
            _mesh.triangles = tris;

            blackLid.GetComponent<MeshFilter>().mesh = _mesh;
        }

        private GameObject CreateFrame()
        {
            Vector3[] vertices = new Vector3[12];
            Color[] colors = new Color[12];
            int[] tris = new int[48];
            Vector3 offset = new Vector3(0, offsetFrame, 0);

            _upLeft += offset;
            _upRight += offset;
            _bottomRight += offset;
            _bottomLeft += offset;

            vertices[0] = _upLeft;
            vertices[1] = _upRight;
            vertices[2] = _bottomRight;
            vertices[3] = _bottomLeft;

            vertices[4] = _upLeft + new Vector3(fadeThreshold, 0, -fadeThreshold);
            vertices[5] = _upRight + new Vector3(-fadeThreshold, 0, -fadeThreshold);
            vertices[6] = _bottomRight + new Vector3(-fadeThreshold, 0, fadeThreshold);
            vertices[7] = _bottomLeft + new Vector3(fadeThreshold, 0, fadeThreshold);

            vertices[8] = _upLeft + new Vector3(frameDistance, 0, -frameDistance);
            vertices[9] = _upRight + new Vector3(-frameDistance, 0, -frameDistance);
            vertices[10] = _bottomRight + new Vector3(-frameDistance, 0, frameDistance);
            vertices[11] = _bottomLeft + new Vector3(frameDistance, 0, frameDistance);

            int[] trisOrder = new int[48] { 0, 1, 4, 4, 1, 5, 4, 5, 8, 8, 5, 9, 1, 2, 5, 5, 2, 6, 5, 6, 10, 10, 9, 5, 2, 3, 7, 7, 6, 2, 7, 11, 6, 6, 11, 10, 3, 0, 7, 7, 0, 4, 7, 4 , 8, 7, 8, 11};

            for (int t = 0; t < tris.Length; t++)
            {
                tris[t] = trisOrder[t];
            }

            for (int i = 0; i < 8; i++)
            {
                colors[i] = Color.black;
            }

            for (int i = 8; i < 12; i++)
            {
                colors[i] = new Color(0, 0, 0, 0);
            }

            Mesh mesh = new();

            mesh.vertices = vertices;
            mesh.colors = colors;
            mesh.uv = new Vector2[12];
            mesh.triangles = tris;

            blackFrame.GetComponent<MeshFilter>().mesh = mesh;

            return blackFrame;
        }

        private void Update()
        {
            if (_isEntering)
            {
                if (_fade > 0)
                {
                    _fade -= _delta;
                    ApplyingFade();
                }
                else
                {
                    _isEntering = false;
                    blackLid.SetActive(false);
                    PlayerController.Instance.ResumeInteractions();
                }
            }

            if (_isExiting)
            {
                blackLid.SetActive(true);

                if (_fade < 1)
                {
                    _fade += _delta;
                    ApplyingFade();
                }
                else
                {
                    _isExiting = false;
                }
            }
        }

        private void ApplyingFade()
        {
            for (int i = 0; i < 4; i++)
            {
                _colors[i] = new Color(0, 0, 0, _fade);
            }

            _mesh.colors = _colors;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(StringsData.PLAYER))
            {
                virtualCamera.gameObject.SetActive(true);
                _delta = 1 / enteringFadeDuration * Time.deltaTime;
                _isEntering = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(StringsData.PLAYER))
            {
                virtualCamera.gameObject.SetActive(false);
                _delta = 1 / exitingFadeDuration * Time.deltaTime;
                _isExiting = true;
            }
        }
    }
}*/