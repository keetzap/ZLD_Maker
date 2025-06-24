using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace Keetzap.ZeldaMaker
{
    public class CanvasGameOver : CanvasBase
    {
        public Action RetryEvent;

        [SerializeField] private Button btnRetry;
        [SerializeField] private Button btnQuit;

        void Awake()
        {
            btnRetry.onClick.AddListener(Retry);
            btnQuit.onClick.AddListener(Quit);
        }

        public override void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(btnRetry.gameObject);
        }

        private void Retry()
        {
            GameManager.Instance.RetryGame();
            RetryEvent?.Invoke();
        }

        private void Quit()
        {
            Debug.Log("Quit");
            Application.Quit();
        }
    }
}
