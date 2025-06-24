using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using Unity.IO.LowLevel.Unsafe;

namespace Keetzap.ZeldaMaker
{
    public class CanvasController : MonoBehaviour
    {
        public Action CloseMessageEvent;

        public static CanvasController Instance;

        [SerializeField] private InputActionAsset playerControls;
        [SerializeField] private HeartsController heartsController;
        [SerializeField] private ConsumableController consumableController;
        [SerializeField] private TMP_Text TXT_GemsAmount;
        [SerializeField] private GameObject infoMessage;
        [SerializeField] private GameObject actionMessage;
        [SerializeField] private GameObject gameOverCanvas;
        [SerializeField] private GameObject pauseMenuCanvas;
        [SerializeField] private GameObject blackBackground;
        [SerializeField] private float fadeTime = 0.5f;

        public float FadeTime => fadeTime;

        private enum FadeState
        {
            None,
            In,
            Wait,
            Out
        }

        private GameData _gameData;
        private GD_PlayerStats _playerStats;
        //private InputAction _interact;
        private Dictionary<TypeOfMessage, GameObject> _messages = new();
        private Dictionary<InputAction, Action<InputAction.CallbackContext>> _actions;
        private FadeState _fadeState;
        private Image _blackBackground;
        private Color _color;
        private Coroutine _waitingTime;
        private bool _menuPauseState;

        private void Awake()
        {
            Instance = this;
            _gameData = GameManager.Instance.GameData;
            _playerStats = _gameData.playerStats;
            _blackBackground = blackBackground.GetComponent<Image>();
            _color = _blackBackground.color;
            _fadeState = FadeState.None;

            AddListeners();
            InitializePlayerInput();
            InitializeLifesMaxCapacity();
            InitializeKeys();
            OnGemsHandlerEvent(0);
            InitializeMessages();
        }

        private void AddListeners()
        {
            GameManager.Instance.AddingHeart += OnAddingHeart;
            GameManager.Instance.LifesHandler += OnLifesHandlerEvent;
            GameManager.Instance.GemsHandler += OnGemsHandlerEvent;
            GameManager.Instance.KeysHandler += OnKeyHandlerEvent;
            GameManager.Instance.GameOverHandler += OnGameOverHandlerEvent;
            
            gameOverCanvas.GetComponent<CanvasGameOver>().RetryEvent += OnRetryHandlerEvent;
            pauseMenuCanvas.GetComponent<CanvasPause>().ResumeEvent += OnPauseMenuHandlerEvent;
        }

        private void InitializePlayerInput()
        {
            InputActionMap gamePlayMap = playerControls.FindActionMap(StringsData.UI);

            _actions = new Dictionary<InputAction, Action<InputAction.CallbackContext>>
            {
                { gamePlayMap.FindAction(StringsData.INTERACT), Interact },
                { gamePlayMap.FindAction(StringsData.PAUSE), Pause }

            };

            foreach (KeyValuePair<InputAction, Action<InputAction.CallbackContext>> action in _actions)
            {
                action.Key.performed += action.Value;
                action.Key.canceled += action.Value;
            }
        }

        private void InitializeLifesMaxCapacity()
        {
            for (int h = 0; h < _playerStats.LifesMaxCapacity; h++)
            {
                heartsController.AddHeart();
            }

            OnLifesHandlerEvent();
        }

        private void OnAddingHeart()
        {
            heartsController.AddHeart();
            heartsController.RecoverAllLifes();
        }

        private void OnLifesHandlerEvent()
        {
            for (int h = 0; h < _playerStats.LifesMaxCapacity; h++)
            {
                heartsController.SetLifes(h, h < _playerStats.GetCurrentLifes());
            }
        }

        private void OnGemsHandlerEvent(int gems)
        {
            TXT_GemsAmount.text = _playerStats.Gems.ToString();
        }

        private void InitializeKeys()
        {
            LoadKey(_playerStats.SilverKey, TypeOfKey.SilverKey);
            LoadKey(_playerStats.GoldenKey, TypeOfKey.GoldenKey);
            LoadKey(_playerStats.BossKey, TypeOfKey.BossKey);
        }

        private void LoadKey(int counter, TypeOfKey typeOfKey)
        {
            for (int k = 0; k < counter; k++)
            {
                GD_Key key = _gameData.keys.Find(key => key.typeOfkey == typeOfKey);
                consumableController.CreateConsumable(key);
            }
        }

        private void OnKeyHandlerEvent(GD_Key key, int value)
        {
            consumableController.SetKey(key, value);
        }

        private void InitializeMessages()
        {
            _messages.Add(TypeOfMessage.Hint, actionMessage);
            _messages.Add(TypeOfMessage.Information, infoMessage);

            HideCanvases();
        }

        public void DisplayMessage(TypeOfMessage typeOfMessage, string message)
        {
            var canvas = _messages[typeOfMessage];

            SetInfoMessage(canvas, message);
            SetCanvasVisibility(canvas, true);
        }

        public void HideMessage(TypeOfMessage typeOfMessage)
        {
            SetCanvasVisibility(_messages[typeOfMessage], false);

            if (typeOfMessage != TypeOfMessage.Hint)
            {
                CloseMessageEvent?.Invoke();
            }
        }

        public void OnGameOverHandlerEvent()
        {
            gameOverCanvas.SetActive(true);
        }

        public void OnPauseMenuHandlerEvent()
        {
            _menuPauseState = !_menuPauseState;

            pauseMenuCanvas.SetActive(_menuPauseState);
            Time.timeScale = _menuPauseState ? 0 : 1;
        }

        // This function in necessary because it's called by the buttons on the UI
        public void HideCanvas(GameObject canvas)
        {
            SetCanvasVisibility(canvas, false);
            CloseMessageEvent?.Invoke();
        }

        private void SetInfoMessage(GameObject canvas, string message)
        {
            canvas.GetComponent<InfoMessage>().PopulateMessage(message);
        }

        private void SetCanvasVisibility(GameObject canvas, bool state)
        {
            canvas.SetActive(state);
        }

        private void HideCanvases()
        {
            SetCanvasVisibility(infoMessage, false);
            SetCanvasVisibility(actionMessage, false);
        }

        private void OnRetryHandlerEvent()
        {
            _fadeState = FadeState.In;
            blackBackground.SetActive(true);
        }

        private void Update()
        {
            switch (_fadeState)
            {
                case FadeState.In:      FadeIn();       break;
                case FadeState.Wait:    WaitingTime();  break;
                case FadeState.Out:     FadeOut();      break;
            }
        }

        private void FadeIn()
        {
            _color.a += Time.deltaTime / fadeTime;

            if (_color.a > 1)
            {
                _color.a = 1;
                _fadeState = FadeState.Wait;
            }

            _blackBackground.color = _color;
        }

        private void WaitingTime()
        {
            if (_waitingTime == null)
            {
                _waitingTime = StartCoroutine(WaitUntilPlayerGetsRespawn());
            }
        }

        private void FadeOut()
        {
            _color.a -= Time.deltaTime / fadeTime;

            if (_color.a < 0)
            {
                _color.a = 0;
                _fadeState = FadeState.None;
                blackBackground.SetActive(false);
            }

            _blackBackground.color = _color;
        }

        private IEnumerator WaitUntilPlayerGetsRespawn()
        {
            gameOverCanvas.SetActive(false);

            yield return new WaitForSeconds(GameManager.Instance.TimeToRespawn);
            _fadeState = FadeState.Out;
            _waitingTime = null;
        }

        private void OnEnable()
        {
            foreach (KeyValuePair<InputAction, Action<InputAction.CallbackContext>> action in _actions)
                action.Key.Enable();
        }

        private void OnDisable()
        {
            foreach (KeyValuePair<InputAction, Action<InputAction.CallbackContext>> action in _actions)
                action.Key.Disable();
        }

        private void OnDestroy()
        {
            GameManager.Instance.AddingHeart -= OnAddingHeart;
            GameManager.Instance.LifesHandler -= OnLifesHandlerEvent;
            GameManager.Instance.GemsHandler -= OnGemsHandlerEvent;
            GameManager.Instance.KeysHandler -= OnKeyHandlerEvent;
            GameManager.Instance.GameOverHandler -= OnGameOverHandlerEvent;
        }

        private void Interact(InputAction.CallbackContext context)
        {
            if (context.ReadValueAsButton())
            {
                HideCanvases();
            }
        }

        private void Pause(InputAction.CallbackContext context)
        {
            if (context.ReadValueAsButton())
            {
                OnPauseMenuHandlerEvent();
            }
        }
    }
}
