using System;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    public class CharacterControllerTest: MonoBehaviour
    {
        private readonly string RUN = "Run";
        private readonly string IDLE = "Idle";
        private readonly string SWORD = "SwordAttack";
        private readonly string SHIELD = "Shield";
        //private readonly string _pick = "Pick";
        private readonly string PUSH = "Push";
        private readonly string BOMB = "Bomb";
        private readonly string KEY = "Key";

        Rigidbody Rigidbody => GetComponent<Rigidbody>();
        //Animator Animator => GetComponentInChildren<Animator>();
        //RenderVisibility RenderVisibility => GetComponent<RenderVisibility>();
        //CharacterInventory CharacterInventory => GetComponent<CharacterInventory>();

        [SerializeField] private float rotataionSpeed = 10.0f;
        [SerializeField] private float playerSpeed = 200.0f;
        [SerializeField] private float pushSpeed = 50.0f;
        [SerializeField] private Transform bomb;
        [SerializeField] private Vector3 bombForce = new(0, 20, 50);

        private Vector3 _targetRotation;
        private float _rotateFactor = 45;
        private float _speed;
        private GameObject _bomb;
        private bool _isLoaded;
        private float _factorForceMultiplier = 10f;
        private bool _isColliding;

        private void Start()
        {
            _speed = playerSpeed;
        }

        //private void Update()
        //{
        //    GetInput();
        //}

        void FixedUpdate()
        {
            Vector3 input = new(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            Vector3 inputRaw = new (Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            
            if (input.sqrMagnitude > 1f)
                input.Normalize();

            if (inputRaw.sqrMagnitude > 1f)
                inputRaw.Normalize();

            if (inputRaw != Vector3.zero)
            {
                _targetRotation = Quaternion.LookRotation(input).eulerAngles;
                //Animator.SetBool(RUN, true);
                //Animator.SetBool(IDLE, false);
            }
            else
            {
                //Animator.SetBool(RUN, false);
                //Animator.SetBool(IDLE, true);
            }

            Quaternion eulerRotation = Quaternion.Euler(_targetRotation.x, Mathf.Round(_targetRotation.y / _rotateFactor) * _rotateFactor, _targetRotation.z);
            Rigidbody.rotation = Quaternion.Slerp(transform.rotation, eulerRotation, Time.deltaTime * rotataionSpeed);
            Rigidbody.linearVelocity = input * _speed * Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            //if (other.CompareTag(BOMB) && !_isLoaded)
            //{
            //    Animator.SetBool(BOMB, true);
            //    RenderVisibility.SetVisibility(false);
            //    _bomb = other.gameObject;
            //    _bomb.transform.position = bomb.transform.position;
            //    _bomb.transform.parent = bomb.transform;

            //    _isLoaded = true;
            //}

            //if (other.CompareTag(KEY))
            //{
            //    var typeItem = other.GetComponent<TypeItem>();
            //    var key = CharacterInventory.items.keys.Find(k => k.typeOfKey == typeItem.key.typeOfKey);
            //    key.amount += typeItem.amount;

            //    Destroy(other.gameObject);
            //}
        }

        private void OnTriggerStay(Collider other)
        {
            //if (_isLoaded) return;

            //bool value = (other.CompareTag("Wall") || other.CompareTag("Draggable")) && Physics.Raycast(transform.position, transform.forward * 1, out _, 1.0f);

            //SetRigidbody(other, false);
            //Animator.SetFloat(PUSH, value ? 1 : 0);
            //_speed = value ? pushSpeed : playerSpeed;
            //RenderVisibility.SetVisibility(!value);
        }

        private void OnTriggerExit(Collider other)
        {
            //if (_isLoaded) return;

            //SetRigidbody(other, true);
        }

        //private void SetRigidbody(Collider other, bool state)
        //{
        //    if (other.attachedRigidbody != null)
        //        other.attachedRigidbody.isKinematic = state;
        //}

        //private void GetInput()
        //{
        //    if (Input.GetMouseButtonDown(0))
        //        Animator.SetTrigger(SWORD);

        //    Animator.SetBool(SHIELD, Input.GetMouseButton(1));

        //    if (Input.GetKeyDown(KeyCode.Space) && _isLoaded)
        //    {
        //        _isLoaded = false;

        //        RenderVisibility.SetVisibility(true);
        //        Animator.SetBool(BOMB, false);

        //        Rigidbody rigidbody = _bomb.AddComponent<Rigidbody>();
        //        rigidbody.AddForce((Vector3.up * bombForce.y + transform.forward * bombForce.z) * _factorForceMultiplier);
        //        _bomb.transform.parent = null;
        //    }
        //}

        // Getting item. Passem per sobre i si l'objecte �s nou, surt l'animaci�
    }
}
