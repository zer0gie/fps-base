using System;
using UnityEngine;
using Zenject;

namespace Code.Player
{
    public class PlayerController : MonoBehaviour, IDisposable
    {
        [SerializeField] private GameObject cameraTarget;
        [SerializeField] private GameObject activeWeaponSlot;

        private InputManager _inputManager;
    
        private CharacterController _chController;

        private float _moveSpeed = 5f;

        private readonly float _gravity = -13f;
        private readonly float _smoothTime = 0.05f;

        private float _mouseSensitivity = 0.05f;
        private float _xRotation;
        private float _mouseX;
        private float _mouseY;
        
        private float _recoilReturnSpeed = 20f;
        private float _currentRecoil;

        private Vector3 _fallVelocity;
        private Vector3 _currentVelocity;
        private Vector3 _smoothDumpVelocity;


        [Inject]
        public void Construct(InputManager inputManager)
        {
            _chController = gameObject.GetComponent<CharacterController>();
            _inputManager = inputManager;
        }
        private void Start()
        {
            _inputManager.EnableFPSControl();
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            HandleGravity();
            HandleMovement();
        }
        private void LateUpdate()
        {
            CameraRotation();
        }

        public void ApplyRecoil(float recoilAmount)
        {
            _currentRecoil += recoilAmount;
        }
        private void HandleMovement()
        {
            if (_chController.isGrounded)
            {
                EnableMovement(_moveSpeed);
            }
            else
            {
                EnableMovement(_moveSpeed * 0.5f);
            }
        }
        private void CameraRotation()
        {
            var inputLook = _inputManager.GetLookDelta();

            _mouseX = inputLook.x * _mouseSensitivity;
            _mouseY = inputLook.y * _mouseSensitivity;
            
            _currentRecoil = Mathf.Lerp(_currentRecoil, 0f, _recoilReturnSpeed * Time.deltaTime);
            
            _xRotation -= _mouseY;
            _xRotation -= _currentRecoil;
            _xRotation = Mathf.Clamp(_xRotation, -89f, 89f);

            cameraTarget.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

            transform.Rotate(Vector3.up * _mouseX);
        }
        private void HandleGravity()
        {
            if (_chController.isGrounded && _fallVelocity.y < 0)
            {
                _fallVelocity.y = -2f;
            }

            _fallVelocity.y += _gravity * Time.deltaTime;

            _chController.Move(_fallVelocity * Time.deltaTime);
        }
        private void EnableMovement(float moveSpeed)
        {
            var inputVector = _inputManager.GetMovementVectorNormalized();

            Vector3 moveVector = new(inputVector.x, 0f, inputVector.y);

            moveVector = transform.TransformDirection(moveVector);

            var targetVelocity = moveVector * moveSpeed;

            _currentVelocity = Vector3.SmoothDamp(_currentVelocity, targetVelocity, ref _smoothDumpVelocity, _smoothTime);

            _chController.Move(_currentVelocity * Time.deltaTime);
        }
        public Transform GetWeaponSlotTransform()
        {
            return activeWeaponSlot.transform;
        }
        public void Dispose()
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}