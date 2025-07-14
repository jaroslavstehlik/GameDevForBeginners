using GameDevForBeginners;
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
        [Range(0f, 1f)]
        public float magnetism = 0.1f;
        [Range(0f, 1f)]
        public float stickiness = 0.1f;
        public float levitationHeight = 0.1f;
        
        private PlayerInput _playerInput;
        private CollisionInfo _collisionInfo;
        private Vector3 _up = Vector3.up;
        private Ray _groundRay;
        private Vector3 _groundDirection;

        private Ray _groundRayFront;
        private RaycastHit _frontRaycastHit;
        private Ray _groundRayBack;
        private RaycastHit _backRaycastHit;
        
        private Ray _groundRayLeft;
        private RaycastHit _leftRaycastHit;
        private Ray _groundRayRight;
        private RaycastHit _rightRaycastHit;

        private Vector3 _groundPosition;

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
            float playerRadius = _collider.radius;
            
            //Draw.Plane("myPlane", ConsumeType.FixedUpdate, position, rotation, Vector3.one);
            
            Vector3 playerUp = rotation * Vector3.up;
            Vector3 playerDown = -playerUp;
            Vector3 playerForward = rotation * Vector3.forward;
            Vector3 playerBack = -playerForward;
            Vector3 playerRight = rotation * Vector3.right;
            Vector3 playerLeft = -playerRight;
            
            float rayCastDistance = playerRadius * 10f;
           
            Vector3 upOffset = playerUp;
            float spread = playerRadius;
            _groundRayFront = new Ray(position + upOffset + playerForward * spread, playerDown);
            _groundRayBack = new Ray(position + upOffset + playerBack * spread, playerDown);
            _groundRayLeft = new Ray(position + upOffset + playerLeft * spread, playerDown);
            _groundRayRight = new Ray(position + upOffset + playerRight * spread, playerDown);
            
            bool front = Physics.Raycast(_groundRayFront, out _frontRaycastHit, rayCastDistance, environmentMask,
                QueryTriggerInteraction.Ignore);
            
            bool back = Physics.Raycast(_groundRayBack, out _backRaycastHit, rayCastDistance, environmentMask,
                QueryTriggerInteraction.Ignore);

            bool left = Physics.Raycast(_groundRayLeft, out _leftRaycastHit, rayCastDistance, environmentMask,
                QueryTriggerInteraction.Ignore);
            
            bool right = Physics.Raycast(_groundRayRight, out _rightRaycastHit, rayCastDistance, environmentMask,
                QueryTriggerInteraction.Ignore);

            bool ground = FindPlayerGround(position, playerUp, playerRadius, rayCastDistance, environmentMask,
                out _groundPosition);
            
            bool isGrounded = front || back || left || right || ground;
            bool isStable = front && back && left && right;
            
            Vector3 fakeForward = (_frontRaycastHit.point - _backRaycastHit.point).normalized;
            Vector3 fakeRight = (_rightRaycastHit.point- _leftRaycastHit.point).normalized;
            Vector3 fakeUp = Vector3.Cross(fakeForward, fakeRight);

            if (isStable)
            {
                _up = Vector3.Lerp(_up, fakeUp, stickiness);
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
                Vector3 groundLevitation = (_groundPosition + playerUp * levitationHeight);
                Vector3 groundDirection = groundLevitation - position;
                futurePlayerVelocity += (groundDirection / Time.fixedDeltaTime) * magnetism;
            }
            
            _rigidbody.rotation = playerWorldRotation;
            _rigidbody.linearVelocity = futurePlayerVelocity;
        }
        
        static bool FindPlayerGround(Vector3 position, Vector3 up, float radius, float maxDistance, int layerMask, out Vector3 groundPosition)
        {
            Ray ray = new Ray(position + up, -up);
            if (!Physics.SphereCast(ray, radius, out RaycastHit raycastHit, maxDistance, layerMask,
                    QueryTriggerInteraction.Ignore))
            {
                groundPosition = Vector3.zero;
                return false;
            }

            Vector3 localPoint = raycastHit.point - ray.origin;
            Vector3 closestPoint = ray.origin + Vector3.Project(localPoint, ray.direction);
            groundPosition = closestPoint;
            return true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(_rigidbody.position, 0.1f);
            Gizmos.DrawWireSphere(_groundPosition, 0.05f);
        }
    }
}