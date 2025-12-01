using UnityEngine;

namespace GameDevForBeginners
{
    [System.Serializable]
    public class MovementSettings
    {
        public float moveSpeed = 0.2f;
        public float maxSlopeAngle = 60f;
        public float crouchMultiplier = 0.5f;
        public float sprintMultiplier = 2f;
        public float jumpHeight = 2f;
        public float jumpSpeed = 0.2f;
        public float fallSpeed = 0.2f;
        public bool useMovingPlatforms = true;
    }
    
    // TODO: Unable to jump when directly touching stairs from side
    // TODO: Support moving platforms
    
    [AddComponentMenu("GMD/Character/CharacterControllerBasic")]
    public class CharacterControllerBasic : MonoBehaviour
    {
        [SerializeField] private InputController _inputController;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private CapsuleCollider _collider;
        [SerializeField] private SphereCast _groundDetector;
        [SerializeField] private SphereCast _ceilingDetector;
        [SerializeField] private Rigidbody _rigidbody;
        
        public LayerMask environmentMask = int.MaxValue;
        public bool visualiseDebug = true;
        public bool showGroundHit = true;
        public bool showCeilingHit = true;
        
        private CollisionState _collisionState = new CollisionState();
        public MovementSettings movementSettings = new MovementSettings();
        private MovementStateMachine _movementStateMachine;
        private PlayerInput _playerInput = new PlayerInput();
        
        private void Awake()
        {
            _movementStateMachine = new MovementStateMachine(_collisionState, _playerInput, movementSettings);
        }

        private void OnEnable()
        {
            _inputController.onPlayerInputChanged += OnInputChanged;
        }

        private void OnDisable()
        {
            _inputController.onPlayerInputChanged -= OnInputChanged;
        }

        void OnInputChanged(PlayerInput playerInput)
        {
            _playerInput = playerInput;
            _movementStateMachine.UpdatePlayerInput(_playerInput);
        }
        
        void FixedUpdate()
        {
            float cameraYaw = _cameraTransform != null ? _cameraTransform.rotation.eulerAngles.y : _rigidbody.rotation.eulerAngles.y;
            _rigidbody.rotation = Quaternion.Euler(0f, cameraYaw, 0f);

            MovementStateData movementStateData = new MovementStateData()
            {
                velocity = Vector3.zero,
            };
            
            movementStateData.SetPositionAndRotation(_rigidbody);
            movementStateData.SetJumpDirection(-Physics.gravity);
            
            _collisionState.Update(_rigidbody, _rigidbody.transform, _groundDetector, _ceilingDetector, environmentMask, movementSettings.maxSlopeAngle);
            _movementStateMachine.Update(movementStateData);
            
            _rigidbody.linearVelocity = movementStateData.velocity;
            _playerInput.jump.Reset();
        }

        private void OnDrawGizmos()
        {
            if (!visualiseDebug)
                return;

            if (_collisionState != null)
            {
                if (showGroundHit)
                {
                    _collisionState.groundSphereCastInfo.DebugDrawHits();
                    _collisionState.groundSphereCastInfo.DebugDrawNormals();
                }

                if (showCeilingHit)
                {
                    _collisionState.ceilingSphereCastInfo.DebugDrawHits();
                    _collisionState.ceilingSphereCastInfo.DebugDrawNormals();
                }

            }
        }
    }
}