using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//using System.Linq;

namespace Keetzap.ZeldaMaker
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        private readonly int idle = Animator.StringToHash(StringsData.IDLE);
        private readonly int locomotion = Animator.StringToHash(StringsData.LOCOMOTION);
        private readonly int shield = Animator.StringToHash(StringsData.SHIELD);
        private readonly int sword = Animator.StringToHash(StringsData.SWORD);
        private readonly int push = Animator.StringToHash(StringsData.PUSH);
        //CharacterInventory CharacterInventory => GetComponent<CharacterInventory>();

        public GD_PlayerStats playerStats;

        [SerializeField] private InputActionAsset playerControls;
        //[SerializeField] private WeaponsManager weaponsManager;

        [SerializeField] private bool useSmoothRotation;
        [SerializeField] private float rotationSpeed = 0.1f;
        [SerializeField] private float playerSpeed = 2.5f;
        [SerializeField] private float gravity = Physics.gravity.y;
        [SerializeField] private float pushSpeed = 1.5f;
        [SerializeField] private GameObject mainCharacterModel;
        //[SerializeField] private Transform headPosition;
        //[SerializeField] private Vector3 bombForce = new(0, 20, 50);

        private WeaponsManager _weaponsManager;
        private CharacterController _characterController;
        private Animator _animator;
        
        private Vector3 _direction;
        private Vector3 _lastKnownDirection;
        private Vector3 _targetRotation;
        private Vector3 _targetDesirePosition;
        private Quaternion _targetDesireRotation;
        private float _speed;
        private float _repositioningSpeed;
        private bool _characterIsMoving;
        private float _groundedGravity = -0.05f;

        private struct CCDefaultValues
        {
            public float StepOffset;
            public float SlopeLimit;
            public float Radius;
        }

        private CCDefaultValues _ccDefaultValues;
        private float _raycastDistance = 10;
        private RaycastHit _deathHit;

        //private GameObject _bomb;
        //private bool _isLoaded;
        //private float _factorForceMultiplier = 10f;
        public bool IsInteracting { get; set; }
        
        public Animator MainCharacterAnimator => mainCharacterModel.GetComponent<Animator>();
        public Vector3 Position => transform.position;
        public float PushSpeed => pushSpeed;

        private Dictionary<InputAction, Action<InputAction.CallbackContext>> _actions;

        public static PlayerController Instance;

        public enum PlayerState
        {
            ControlledByPlayer,
            ControlledByComputer,
            PlayerIsDead,
            Paused,
            Unknown
        }

        private PlayerState _state = PlayerState.ControlledByPlayer;
        private PlayerState _currentState = PlayerState.Unknown;    

        public void Awake()
        {
            Instance = this;

            _weaponsManager = GetComponentInChildren<WeaponsManager>();
            _characterController = GetComponent<CharacterController>();
            _animator = GetComponentInChildren<Animator>();
            _state = PlayerState.ControlledByPlayer;

            _ccDefaultValues = new CCDefaultValues()
            {
                StepOffset = _characterController.stepOffset,
                SlopeLimit = _characterController.slopeLimit,
                Radius = _characterController.radius
            };

            AddInputPlayerController();
        }

        #region INPUT_PLAYER_CONTROLLER
        private void AddInputPlayerController()
        {
            InputActionMap gamePlayMap = playerControls.FindActionMap(StringsData.GAMEPLAY);

            _actions = new Dictionary<InputAction, Action<InputAction.CallbackContext>>
            {
                { gamePlayMap.FindAction(StringsData.LOCOMOTION), Locomotion },
                { gamePlayMap.FindAction(StringsData.SWORD), Sword },
                { gamePlayMap.FindAction(StringsData.SHIELD), Shield },
                { gamePlayMap.FindAction(StringsData.INTERACT), Interact }//,
                //{ gamePlayMap.FindAction(StringsData.BOMB), Bomb },
            };

            foreach (KeyValuePair<InputAction, Action<InputAction.CallbackContext>> action in _actions)
            {
                action.Key.performed += action.Value;
                action.Key.canceled += action.Value;
            }
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
        #endregion

        private void Start()
        {
            _speed = playerSpeed;
        }

        private void Update()
        {
            switch (_state)
            {
                case PlayerState.ControlledByPlayer:
                    {
                        MovementControlledByPlayer();
                        break;
                    }
                case PlayerState.ControlledByComputer: 
                    {
                        MoveToDesirePosition();
                        break; 
                    }
                case PlayerState.PlayerIsDead:
                    {
                        PlayerIsDying();
                        break;
                    }
                case PlayerState.Paused:
                    {
                        break;
                    }
            }

            if (_currentState != _state)
            {
                _currentState = _state;
                Debug.Log("<color=cyan>State</color>: " + _state);
            }
        }

        private void FixedUpdate()
        {
            if (_state == PlayerState.PlayerIsDead || _state == PlayerState.Paused) return;

            var ssss = Physics.Raycast(transform.position, Vector3.down * _raycastDistance, out _deathHit, _raycastDistance);

            if (_deathHit.collider == null) return;

            if (_deathHit.collider.gameObject.layer == LayerMask.NameToLayer("RespawnArea"))
            {
                _state = PlayerState.PlayerIsDead;
                _lastKnownDirection = _direction;
                _characterController.stepOffset = 0;
                _characterController.slopeLimit = 0;
                _characterController.radius = 0.0f;
            }
            
            //Debug.DrawRay(transform.position, Vector3.down * _raycastDistance, Color.cyan, 0.05f);// * (_characterController.height / 2 + _raycastDistance), groundedInFront ? Color.green : Color.red);
        }

        public void PausePlayer()
        {
            _state = PlayerState.Paused;
        }

        private void MovementControlledByPlayer()
        {
            _characterController.Move(_direction * _speed * Time.deltaTime);
            _targetRotation = new(_direction.x, 0, _direction.z);

            if (_characterIsMoving)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_targetRotation);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, useSmoothRotation ? rotationSpeed * Time.deltaTime : Mathf.Infinity * Time.deltaTime);
            }

            _direction.y += (_characterController.isGrounded ? _groundedGravity : gravity) * Time.deltaTime;
        }

        //private void ApplyGravity()
        //{
        //    _direction.y += (_characterController.isGrounded ? _groundedGravity : gravity) * Time.deltaTime;
        //}

        public void PlayerIsDying()
        {
            _lastKnownDirection.y += gravity * Time.deltaTime;
            _characterController.Move(_lastKnownDirection * _speed * Time.deltaTime);
        }

        private void MoveToDesirePosition()
        {
            transform.position = Vector3.Lerp(transform.position, _targetDesirePosition, _repositioningSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, _targetDesireRotation, _repositioningSpeed * Time.deltaTime);

            var thresholdDistance = 0.01f;
            float dist = Vector3.Distance(transform.position, _targetDesirePosition);

            if (Vector3.Distance(transform.position, _targetDesirePosition) < thresholdDistance)
            {
                SetPlayerToDesirePosition(_targetDesirePosition, _targetDesireRotation);
            }
        }

        public void MoveToTargetPosition(Transform targetTransform, float time)
        {
            _state = PlayerState.ControlledByComputer;
            _targetDesirePosition = targetTransform.position;
            _targetDesireRotation = targetTransform.rotation;
            _repositioningSpeed = time;
        }

        public void MoveToTargetPosition(Vector3 targetPosition, float time)
        {
            _state = PlayerState.ControlledByComputer;
            _targetDesirePosition = targetPosition;
            _targetDesireRotation = transform.rotation;
            _repositioningSpeed = time;
        }

        public void SetPlayerToDesirePosition(Vector3 position, Quaternion rotation, bool returnControlToPlayer = true)
        {
            transform.SetPositionAndRotation(position, rotation);
            StartCoroutine(EndOfFixedFrame());

            if (returnControlToPlayer)
            {
                StartCoroutine(ReturnControlToPlayer());
            }
        }

        IEnumerator ReturnControlToPlayer()
        {
            yield return new WaitForFixedUpdate();

            ResetCharacterControllerValues();
            _state = PlayerState.ControlledByPlayer;
        }

        private void ResetCharacterControllerValues()
        {
            _characterController.stepOffset = _ccDefaultValues.StepOffset;
            _characterController.slopeLimit = _ccDefaultValues.SlopeLimit;
            _characterController.radius = _ccDefaultValues.Radius;
        }

        private void OnTriggerEnter(Collider collider)
        {
            //if (other.CompareTag(StringsData.BOMB))
            //{
            //    LoadBomb(other);
            //}
            //else if (other.CompareTag(StringsData.TRAP))
            //{
            //    playerStats.ManageLives(-1);
            //}
            //else

            if (collider.TryGetComponent<ICollectable>(out ICollectable collectable))
            {
                collectable.OnCollected();
            }
        }

        private void OnTriggerStay(Collider collider)
        {
            CheckIfDraggable(collider);
            CheckIfInteracting(collider);
        }

        private void CheckIfDraggable(Collider collider)
        {
            var closenessThreshold = 1f;
            Vector3 centerCharacter = transform.position + _characterController.center;
            Vector3 fwd = transform.TransformDirection(Vector3.forward);

            bool closeness = Physics.Raycast(centerCharacter, fwd, closenessThreshold, 1 << LayerMask.NameToLayer(Layers.Draggable));
            Debug.DrawRay(centerCharacter, fwd * closenessThreshold, Color.red);
            bool draggable = collider.TryGetComponent<IDraggable>(out IDraggable draggableProp);

            if (!draggable) return;

            if (closeness)
            {
                if (_characterIsMoving && (!draggableProp.OnDragged() || draggableProp.OnPushed()))
                {
                    SetPushProperties(true);
                    draggableProp.OnStartDragging(this);
                }
                else
                {
                    DisablePushProperties(draggableProp);
                }
            }
            else
            {
                DisablePushProperties(draggableProp);
            }
        }

        private void DisablePushProperties(IDraggable draggableProp)
        {
            SetPushProperties(false);
            draggableProp.OnStopBehaviour();
        }

        private void SetPushProperties(bool state)
        {
            _animator.SetFloat(push, state ? 1 : 0);
            _weaponsManager.SetVisibility(!state);
            _speed = state ? pushSpeed : playerSpeed;

        }

        private void OnTriggerExit(Collider collider)
        {
            if (collider.TryGetComponent<IDraggable>(out IDraggable draggableProp))
            {
                DisablePushProperties(draggableProp);
            }
        }

        private void CheckIfInteracting(Collider collider)
        {
            if (IsInteracting && collider.TryGetComponent<IInteractable>(out IInteractable interactable))
            { 
                interactable.OnInteract();
            }
        }

        //private void LoadBomb(Collider other)
        //{
        //    if (_isLoaded) return;

        //    Animator.SetBool(StringsData.BOMB, true);
        //    WeaponsManager.SetVisibility(false);
        //    _bomb = other.gameObject;
        //    _bomb.transform.position = headPosition.transform.position;
        //    _bomb.transform.parent = headPosition.transform;

        //    _isLoaded = true;
        //}

        public void OnAttackEvent(bool state)
        {
            _weaponsManager.EnablingColliders(state);
        }

        public void PauseInteractions()
        {
            playerControls.Disable();
        }

        public void ResumeInteractions()
        {
            playerControls.Enable();
        }

        public void ShowWeapons()
        {
            _weaponsManager.SetVisibility(true);
        }

        public void HideWeapons()
        {
            _weaponsManager.SetVisibility(false);
        }

        public float GetCharacterControllerRadius()
        {
            return _characterController.radius;
        }

        #region CALLBACKS

        //public void PickItem()
        //{
        //    _animator.SetTrigger(StringsData.PICK);
        //}

        private void Locomotion(InputAction.CallbackContext context) 
        {
             var moveInput = context.ReadValue<Vector2>();
            _direction = new(moveInput.x, 0, moveInput.y);
            _characterIsMoving = _direction.magnitude > 0;

            bool isIdle = _animator.GetBool(idle);
            bool isLocomotion = _animator.GetBool(locomotion);

            if (_characterIsMoving && !isLocomotion)
            {
                ResetLocomotionParameters(true, false);
            }
            else if (!_characterIsMoving && !isIdle)
            {
                ResetLocomotionParameters(false, true);
            }
        }

        private void ResetLocomotionParameters(bool locomotionState, bool idleState)
        {
            _animator.SetBool(locomotion, locomotionState);
            _animator.SetBool(idle, idleState);
        }

        private void Sword(InputAction.CallbackContext context)
        {
            if (context.ReadValueAsButton())
            {
                _animator.SetTrigger(sword);
            }
        }

        private void Shield(InputAction.CallbackContext context)
        {
            _animator.SetBool(shield, context.ReadValueAsButton());
        }

        private void Interact(InputAction.CallbackContext context)
        {
            if (context.ReadValueAsButton())
            {
                IsInteracting = true;
                StartCoroutine(EndOfFixedFrame());
            }
        }

        //private void Bomb(InputAction.CallbackContext context)
        //{
        //    if (context.ReadValueAsButton() && _isLoaded)
        //    {
        //        _isLoaded = false;

        //        WeaponsManager.SetVisibility(true);
        //        Animator.SetBool(StringsData.BOMB, false);

        //        Rigidbody rigidbody = _bomb.AddComponent<Rigidbody>();
        //        rigidbody.AddForce((Vector3.up * bombForce.y + transform.forward * bombForce.z) * _factorForceMultiplier);
        //        _bomb.transform.parent = null;
        //    }
        //}

        #endregion

        IEnumerator EndOfFixedFrame()
        {
            yield return new WaitForFixedUpdate();
            IsInteracting = false;
            //PlayerIsPaused = false;
        }
    }
}
