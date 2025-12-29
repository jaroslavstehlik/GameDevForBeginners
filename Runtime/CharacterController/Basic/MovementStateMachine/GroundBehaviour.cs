using UnityEngine;

namespace GameDevForBeginners
{
    public class GroundedBehaviour : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private CollisionState _collisionState;
        [SerializeField] private InputController _inputController;

        [Header("State")]
        [SerializedInterface(new [] {typeof(State), typeof(StateBehaviour)}, true)]
        [SerializeField] private SerializedInterface<IState> _movementState = new SerializedInterface<IState>{};
        
        [Header("State Options")]
        [SerializeField] private Option _fallingState;
        [SerializeField] private Option _jumpState;
        [SerializeField] private Option _groundState;
    
        [Header("Variables")]
        [SerializedInterface(new [] {typeof(Counter), typeof(CounterBehaviour)}, true)]
        [SerializeField] private SerializedInterface<ICountable> _moveSpeed = new SerializedInterface<ICountable>{};

        [SerializedInterface(new [] {typeof(Counter), typeof(CounterBehaviour)}, true)]
        [SerializeField] private SerializedInterface<ICountable> _crouchMultiplier = new SerializedInterface<ICountable>{};

        [SerializedInterface(new [] {typeof(Counter), typeof(CounterBehaviour)}, true)]
        [SerializeField] private SerializedInterface<ICountable> _sprintMultiplier = new SerializedInterface<ICountable>{};

        [SerializedInterface(new [] {typeof(Counter), typeof(CounterBehaviour)}, true)]
        [SerializeField] private SerializedInterface<ICountable> _maxSlopeAngle = new SerializedInterface<ICountable>{};
        
        public bool useMovingPlatforms = true;

        void OnEnable()
        {
            
        }

        void FixedUpdate()
        {            
            PlayerInput playerInput = _inputController.playerInput;

            GroundStateInfo groundStateInfo = _collisionState.GetGroundStateInfo(_rigidbody, _maxSlopeAngle.value.count);
            GroundCollisionInfo groundCollisionInfo = groundStateInfo.groundCollisionInfo;

            if (groundCollisionInfo == null || !groundCollisionInfo.isGrounded || groundCollisionInfo.isTooSteep)
            {
                _movementState.value.activeOption = _fallingState;
                return;
            }
            
            if (playerInput.jump.Take())
            {
                _movementState.value.activeOption = _jumpState;
                return;
            }

            float playerSpeed = _moveSpeed.value.count;

            if (playerInput.crouch.isPressed)
            {
                playerSpeed *= _crouchMultiplier.value.count;
            }
            else if (playerInput.sprint.isPressed)
            {
                playerSpeed *= _sprintMultiplier.value.count;
            }

            Vector3 playerInputDirection = new Vector3(playerInput.move.x, 0f, playerInput.move.y).normalized;
            float playerInputMagnitude = Mathf.Clamp(playerInputDirection.magnitude, 0f, 1f) * playerSpeed;

            Vector3 playerMove = ProjectVelocityOnNormal(playerInputDirection, playerInputMagnitude, groundStateInfo.up,
                groundCollisionInfo.localGroundNormal);

            if (groundCollisionInfo.isTooSteep)
            {
                float rampPLayerLocalDotProduct = Vector3.Dot(playerInputDirection, groundCollisionInfo.localGroundNormal);
                if (rampPLayerLocalDotProduct < 0f)
                {
                    playerMove *= 0f;
                }
            }
            
            Vector3 gravityDirection = Physics.gravity.normalized;
            // put player closer to the ramp
            float rampMagnetVelocity = groundCollisionInfo.rampDistance / Time.fixedDeltaTime;
            Vector3 velocity = _rigidbody.rotation * playerMove + gravityDirection * rampMagnetVelocity;

            // Apply moving platforms
            if (useMovingPlatforms)
            {
                Rigidbody groundRigidbody = groundStateInfo.sphereCastInfo.GetRigidbody();
                if (groundRigidbody != null)
                {
                    velocity += groundRigidbody.GetPointVelocity(_rigidbody.position);
                }
            }

            _rigidbody.linearVelocity = velocity;

            float cameraYaw = Camera.main.transform.rotation.eulerAngles.y;
            _rigidbody.rotation = Quaternion.Euler(0f, cameraYaw, 0f);                                 

            _movementState.value.activeOption = _groundState;
        }


        void OnDisable()
        {
            
        }

        static Vector3 ProjectVelocityOnNormal(Vector3 velocityDirection, float velocityMagnitude, Vector3 playerUp,
            Vector3 groundNormal)
        {
            Vector3 rotationAxis = Vector3.Normalize(Vector3.Cross(velocityDirection, playerUp));
            Vector3 tangent = Vector3.Normalize(Vector3.Cross(groundNormal, rotationAxis));
            return tangent * velocityMagnitude;
        }
    }
}
