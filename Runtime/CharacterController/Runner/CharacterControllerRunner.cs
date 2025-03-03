using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chc
{
    struct CollisionInfo
    {
        public Ray ray;
        public bool hit;
        public RaycastHit raycastHit;
    }
    
    public class CharacterControllerRunner : MonoBehaviour
    {
        [SerializeField] private PlayerInputController _playerInputController;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private CapsuleCollider _collider;
        [SerializeField] private Rigidbody _rigidbody;
        public LayerMask environmentMask = int.MaxValue;
        public float moveSpeed = 5f;
        public float sprintMultiplier = 2f;
        public float crouchMultiplier = 0.5f;

        private Queue<PlayerInput> _playerInputQueue = new Queue<PlayerInput>();
        private PlayerInput _playerInput;
        private CollisionInfo _collisionInfo;
        private Vector3 _up = Vector3.up;
        private Ray _groundRay;
        private float _groundDistance = 0f;

        private Ray _groundRayFront;
        private RaycastHit _frontRaycastHit;
        private Ray _groundRayBack;
        private RaycastHit _backRaycastHit;
        
        private Ray _groundRayLeft;
        private RaycastHit _leftRaycastHit;
        private Ray _groundRayRight;
        private RaycastHit _rightRaycastHit;
        private Ray _groundRayCenter;
        private RaycastHit _centerRaycastHit;

        private void OnEnable()
        {
            _playerInputController.onPlayerInputChanged += OnPlayerInputChanged;
        }

        private void OnDisable()
        {
            _playerInputController.onPlayerInputChanged -= OnPlayerInputChanged;
        }

        void OnPlayerInputChanged(PlayerInput playerInput)
        {
            _playerInput.move = playerInput.move;
            _playerInput.sprint = playerInput.sprint;
            _playerInput.crouch = playerInput.crouch;
            
            if(playerInput.jump)
                _playerInput.jump = _playerInput.jump;
        }

        void FixedUpdate()
        {
            Vector3 position = _rigidbody.position;
            Quaternion rotation = _rigidbody.rotation;
            
            float groundOffset = 0.1f;
            float rayCastDistance = _collider.radius * 10f;
           
            Vector3 upOffset = rotation * Vector3.up;
            float spread = _collider.radius;
            _groundRayFront = new Ray(position + upOffset + rotation * Vector3.forward * spread, rotation * Vector3.down);
            _groundRayBack = new Ray(position + upOffset + rotation * -Vector3.forward * spread, rotation * Vector3.down);
            _groundRayLeft = new Ray(position + upOffset + rotation * Vector3.right * spread, rotation * Vector3.down);
            _groundRayRight = new Ray(position + upOffset + rotation * -Vector3.right * spread, rotation * Vector3.down);
            _groundRayCenter = new Ray(position, rotation * Vector3.down);
            
            bool front = Physics.Raycast(_groundRayFront, out _frontRaycastHit, rayCastDistance, environmentMask,
                QueryTriggerInteraction.Ignore);
            
            bool back = Physics.Raycast(_groundRayBack, out _backRaycastHit, rayCastDistance, environmentMask,
                QueryTriggerInteraction.Ignore);

            bool left = Physics.Raycast(_groundRayLeft, out _leftRaycastHit, rayCastDistance, environmentMask,
                QueryTriggerInteraction.Ignore);
            
            bool right = Physics.Raycast(_groundRayRight, out _rightRaycastHit, rayCastDistance, environmentMask,
                QueryTriggerInteraction.Ignore);
            
            bool center = Physics.Raycast(_groundRayCenter, out _centerRaycastHit, rayCastDistance, environmentMask,
                QueryTriggerInteraction.Ignore);

            bool isGrounded = front || back || left || right;
            bool isStable = front && back && left && right;
            
            Vector3 fakeForward = (_frontRaycastHit.point - _backRaycastHit.point).normalized;
            Vector3 fakeRight = (_leftRaycastHit.point - _rightRaycastHit.point).normalized;
            Vector3 fakeUp = Vector3.Cross(fakeForward, fakeRight);

            if (isStable)
            {
                _up = fakeUp;
                _groundDistance = center ? _centerRaycastHit.distance : 0f;
            }
            
            Vector3 forward;
            if (_cameraTransform != null)
            {
                forward = Vector3.ProjectOnPlane(_cameraTransform.forward, _up).normalized;
            }
            else
            {
                forward = Vector3.ProjectOnPlane(_rigidbody.rotation * Vector3.forward, _up).normalized;
            }
            
            float playerSpeed = moveSpeed;
            if (_playerInput.crouch)
            {
                playerSpeed *= crouchMultiplier;
            }
            else if (_playerInput.sprint)
            {
                playerSpeed *= sprintMultiplier;
            }

            Vector3 playerInputDirection = new Vector3(_playerInput.move.x, 0f, _playerInput.move.y).normalized;
            float playerInputMagnitude = Mathf.Clamp(playerInputDirection.magnitude, 0f, 1f) * playerSpeed;
            
            Vector3 playerLocalMove = (playerInputDirection * playerInputMagnitude);
            Quaternion playerWorldRotation = Quaternion.LookRotation(forward, _up);
            Vector3 futurePlayerVelocity = playerWorldRotation * playerLocalMove;
            if (isGrounded)
            {
                futurePlayerVelocity += _up * -_groundDistance;
            }
            
            _rigidbody.rotation = playerWorldRotation;
            _rigidbody.velocity = futurePlayerVelocity;
        }
    }
}