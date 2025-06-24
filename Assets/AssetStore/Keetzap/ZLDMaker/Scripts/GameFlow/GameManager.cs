using System;
using System.Collections;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    public class GameManager : MonoBehaviour
    {
        public Action AddingHeart;
        public Action LifesHandler;
        public Action GameOverHandler;
        public Action<int> GemsHandler;
        public Action<GD_Key, int> KeysHandler;

        [SerializeField] private GameData gameData;
        [SerializeField] private RespawnLevel respawnLevel;
        [SerializeField] private float timeToRespawn = 1;

        public GameData GameData => gameData;
        public float TimeToRespawn => timeToRespawn;

        public static GameManager Instance;

        private void Awake()
        {
            Instance = this;
            //GameData.playerStats.InitializeCollectables();

            DontDestroyOnLoad(gameObject);
        }

        public void AddCollectable(GD_Collectable collectable)
        {
            switch (collectable.typeOfCollectable)
            {
                case TypeOfCollectable.Heart: AddHeart(); break;
                case TypeOfCollectable.Life: SetLife(collectable.amount); break;
                case TypeOfCollectable.Gem: SetGems(collectable.amount); break;
                case TypeOfCollectable.Key: SetKeyValue((GD_Key)collectable, 1); break;
            }
        }

        private void AddHeart()
        {
            GameData.playerStats.AddingHeart();
            AddingHeart?.Invoke();
        }

        public void SetLife(int amount)
        {
            GameData.playerStats.SetLifes(amount);
            LifesHandler?.Invoke();
        }

        public void GameOver()
        {
            if (GameData.playerStats.GetCurrentLifes() <= 0)
            {
                GameOverHandler?.Invoke();
                PlayerController.Instance.PausePlayer();
                PlayerController.Instance.PauseInteractions();
            }
        }
        private void SetGems(int amount)
        {
            GameData.playerStats.Gems += amount;
            GemsHandler?.Invoke(amount);
        }

        public void SetKeyValue(GD_Key key, int value)
        {
            GameData.playerStats.SetKeyAmount(key.typeOfkey, value);
            KeysHandler?.Invoke(key, value);
        }

        public void SetKeyValue(TypeOfKey key, int value)
        {
            GD_Key collectedKey = GameData.keys.Find(k => k.typeOfkey == key);
            SetKeyValue(collectedKey, value);
        }

        public void RetryGame()
        {
            GameData.playerStats.InitializeLifes();
            StartCoroutine(SpawnPlayer());
        }

        private IEnumerator SpawnPlayer()
        {
            yield return new WaitForSeconds(timeToRespawn);
            Transform initialSpawnPoint = respawnLevel.GetCurrentRespawnPoint;
            PlayerController.Instance.SetPlayerToDesirePosition(initialSpawnPoint.position, initialSpawnPoint.rotation);
            LifesHandler?.Invoke();

            yield return new WaitForSeconds(CanvasController.Instance.FadeTime);
            PlayerController.Instance.ResumeInteractions();
        }
    }
}
