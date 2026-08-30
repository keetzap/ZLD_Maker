using System.Collections;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    [RequireComponent(typeof(Listener))]
    public class SpikesTrap : MonoBehaviour
    {
        public static class Fields
        {
            public static string BoxCollider => nameof(boxCollider);
            public static string LifeCost => nameof(lifeCost);
            public static string AllowMovement => nameof(allowMovement);
            public static string SafePoint => nameof(safePoint);
            public static string TimeToRespawn => nameof(timeToRespawn);
            public static string JumpHeight => nameof(jumpHeight);
            public static string DamageInterval => nameof(damageInterval);
        }

        [SerializeField] private BoxCollider boxCollider;
        [SerializeField] private int lifeCost = -1;
        [SerializeField] private bool allowMovement = false;
        [SerializeField] private RespawnPoint safePoint;
        [SerializeField] private float timeToRespawn = 0.8f;
        [SerializeField] private float jumpHeight = 1.5f;
        [SerializeField] private float damageInterval = 1.0f;

        private Listener _listener;
        private bool _isRespawning;
        private float _lastDamageTime;

        private void Awake()
        {
            _listener = GetComponent<Listener>();

            boxCollider.isTrigger = true;

            if (boxCollider.gameObject != gameObject)
            {
                SpikesTrapTriggerForwarder forwarder = boxCollider.gameObject.GetComponent<SpikesTrapTriggerForwarder>();
                if (forwarder == null)
                {
                    forwarder = boxCollider.gameObject.AddComponent<SpikesTrapTriggerForwarder>();
                }
                forwarder.TargetTrap = this;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (boxCollider.gameObject == gameObject)
            {
                HandlePlayerCollision(other);
            }
        }

        public void HandlePlayerCollision(Collider other)
        {
            if (other.CompareTag(StringsData.PLAYER))
            {
                if (!boxCollider.bounds.Intersects(other.bounds)) return;
                if (_listener.GetCurrentState() != Listener.TypeOfState.Enabled.ToString()) return;

                if (allowMovement)
                {
                    if (Time.time - _lastDamageTime >= damageInterval)
                    {
                        _lastDamageTime = Time.time;
                        GameManager.Instance.SetLife(lifeCost);

                        if (GameManager.Instance.GameData.playerStats.GetCurrentLifes() <= 0)
                        {
                            GameManager.Instance.GameOver();
                        }
                    }
                }
                else
                {
                    if (_isRespawning) return;

                    _isRespawning = true;
                    GameManager.Instance.SetLife(lifeCost);
                    PlayerController.Instance.PausePlayer();
                    StartCoroutine(RespawnPlayerWithJump(other));
                }
            }
        }

        private IEnumerator RespawnPlayerWithJump(Collider other)
        {
            PlayerController player = other.GetComponent<PlayerController>();

            Vector3 startPos = player.transform.position;
            Vector3 targetPos = safePoint != null ? safePoint.transform.position : transform.position;
            Quaternion targetRot = safePoint != null ? safePoint.transform.rotation : player.transform.rotation;

            float elapsed = 0f;

            while (elapsed < timeToRespawn)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / timeToRespawn);

                Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
                currentPos.y = Mathf.Lerp(startPos.y, targetPos.y, t) + Mathf.Sin(t * Mathf.PI) * jumpHeight;

                player.transform.position = currentPos;
                yield return null;
            }

            if (GameManager.Instance.GameData.playerStats.GetCurrentLifes() > 0)
            {
                player.SetPlayerToDesirePosition(targetPos, targetRot, true);
            }
            else
            {
                player.SetPlayerToDesirePosition(targetPos, targetRot, false);
                GameManager.Instance.GameOver();
            }

            _isRespawning = false;
        }
    }

    public class SpikesTrapTriggerForwarder : MonoBehaviour
    {
        public SpikesTrap TargetTrap;

        private void OnTriggerEnter(Collider other) => TargetTrap.HandlePlayerCollision(other);
    }
}
