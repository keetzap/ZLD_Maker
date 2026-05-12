using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace Keetzap.ZeldaMaker
{
    public class CanvasPause : CanvasBase
    {
        public Action ResumeEvent;

        [SerializeField] private Button btnResume;
        [SerializeField] private Button btnMainMenu;

        void Awake()
        {
            btnResume.onClick.AddListener(Resume);
            btnMainMenu.onClick.AddListener(MainMenu);
        }

        public override void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(btnResume.gameObject);
            btnResume.GetComponent<GenericButtonInGameController>().OnSelect();
            btnMainMenu.GetComponent<GenericButtonInGameController>().OnDeselect();

            base.OnEnable();
        }

        private void Resume()
        {
            ResumeEvent?.Invoke();
        }

        private void MainMenu()
        {

        }
    }
}
