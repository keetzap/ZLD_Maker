using System.Collections;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    [RequireComponent(typeof(BoxCollider))]
    public class RespawnArea : RespawnSystem
    {
        new public static class Fields
        {
            public static string LifeCost => nameof(lifeCost);
            public static string TimeToRespawn => nameof(timeToRespawn);
        }

        [SerializeField] private int lifeCost = -1;
        [SerializeField] private float timeToRespawn = 1;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(StringsData.PLAYER))
            {
                GameManager.Instance.SetLife(lifeCost);
                PlayerController.Instance.PausePlayer();
                StartCoroutine(SpawnPlayer(other));
            }
        }

        private IEnumerator SpawnPlayer(Collider other)
        {
            yield return new WaitForSeconds(timeToRespawn);

            Transform spawnPoint = GetCurrentRespawnPoint;

            if (GameManager.Instance.GameData.playerStats.GetCurrentLifes() > 0)
            {
                other.GetComponent<PlayerController>().SetPlayerToDesirePosition(spawnPoint.position, spawnPoint.rotation);
            }
            else
            {
                other.GetComponent<PlayerController>().SetPlayerToDesirePosition(spawnPoint.position, spawnPoint.rotation, false);
                GameManager.Instance.GameOver();
            }
        }
    }   
}
