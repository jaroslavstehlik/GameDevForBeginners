using UnityEngine;

namespace GameDevForBeginners
{
    public class GroundedBehaviour3D : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private CollisionState3D _collisionState;
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
        private float MIN_RAMP_DISTANCE = 0.25f;

        void OnEnable()
        {
            
        }

        void FixedUpdate()
        {            
            PlayerInput playerInput = _inputController.playerInput;
            CollisionStateInfo groundStateInfo = _collisionState.GetGroundStateInfo(_maxSlopeAngle.value.count);
            CollisionInfo groundCollisionInfo = groundStateInfo.collisionInfo;

            if (groundCollisionInfo == null || groundCollisionInfo.rampDistance > MIN_RAMP_DISTANCE)
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

            Vector3 localGroundNormal = _rigidbody.transform.InverseTransformDirection(groundCollisionInfo.normal);
            Vector3 playerMove = ProjectVelocityOnNormal(playerInputDirection, playerInputMagnitude, groundStateInfo.up, localGroundNormal);
            
            // put player closer to the ramp
            float rampMagnetVelocity = (groundCollisionInfo.projectedRampDistance - 0.02f) / Time.fixedDeltaTime;
            Vector3 gravityDirection = Physics.gravity.normalized;
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
            // replace with angular velocity
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
