using UnityEngine;

namespace GameDevForBeginners
{
    /*
    Collision detection
    Cast rays on corners to detect if we are standing on a slope.


    */


    public class GroundedBehaviour2D : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private CollisionState2D _collisionState;
        [SerializeField] private InputController _inputController;

        [Header("State")]
        [SerializedInterface(typeof(IState), true)]
        [SerializeField] private SerializedInterface<IState> _movementState = new SerializedInterface<IState>{};
        
        [Header("State Options")]
        [SerializeField] private Option _fallingState;
        [SerializeField] private Option _jumpState;
        [SerializeField] private Option _groundState;
    
        [Header("Variables")]
        [SerializedInterface(typeof(ICountable), true)]
        [SerializeField] private SerializedInterface<ICountable> _moveSpeed = new SerializedInterface<ICountable>{};

        [SerializedInterface(typeof(ICountable), true)]
        [SerializeField] private SerializedInterface<ICountable> _crouchMultiplier = new SerializedInterface<ICountable>{};

        [SerializedInterface(typeof(ICountable), true)]
        [SerializeField] private SerializedInterface<ICountable> _sprintMultiplier = new SerializedInterface<ICountable>{};

        [SerializedInterface(typeof(ICountable), true)]
        [SerializeField] private SerializedInterface<ICountable> _maxSlopeAngle = new SerializedInterface<ICountable>{};
        
        public bool useMovingPlatforms = true;
        private CollisionStateInfo2D groundStateInfo;
        private float MIN_RAMP_DISTANCE = 0.25f;

        void OnEnable()
        {
            
        }

        void FixedUpdate()
        {
            PlayerInput playerInput = _inputController.playerInput;

            groundStateInfo = _collisionState.GetGroundStateInfo(_maxSlopeAngle.value.count);
            CollisionInfo2D groundCollisionInfo = groundStateInfo.collisionInfo;

            if(groundCollisionInfo != null) {
                Debug.DrawLine(_rigidbody.position, _rigidbody.position + groundCollisionInfo.normal, Color.red);
            }

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

            Vector2 playerInputDirection = new Vector2(playerInput.move.x, 0f).normalized;
            float playerInputMagnitude = Mathf.Clamp(playerInputDirection.magnitude, 0f, 1f) * playerSpeed;

            Vector2 localGroundNormal = _rigidbody.transform.InverseTransformDirection(groundCollisionInfo.normal);
            Vector2 playerMove = ProjectVelocityOnNormal(playerInputDirection, playerInputMagnitude, groundStateInfo.up, localGroundNormal);
            
            // put player closer to the ramp
            float rampMagnetVelocity = (groundCollisionInfo.rampDistance - 0.02f) / Time.fixedDeltaTime;
            Vector2 gravityDirection = Physics2D.gravity.normalized;
            Vector2 velocity = playerMove + gravityDirection * rampMagnetVelocity;

            // Apply moving platforms
            if (useMovingPlatforms)
            {
                Rigidbody2D groundRigidbody = groundStateInfo.sphereCastInfo.GetRigidbody();
                if (groundRigidbody != null)
                {
                    velocity += groundRigidbody.GetPointVelocity(_rigidbody.position);
                }
            }

            _rigidbody.linearVelocity = velocity;
            _movementState.value.activeOption = _groundState;
        }


        void OnDisable()
        {
            
        }

        void OnDrawGizmos()
        {
            //groundStateInfo.sphereCastInfo.DebugDrawHits();
            //groundStateInfo.sphereCastInfo.DebugDrawNormals();
        }

        static Vector2 ProjectVelocityOnNormal(Vector2 velocityDirection, float velocityMagnitude, Vector2 playerUp,
            Vector2 groundNormal)
        {
            Vector3 rotationAxis = Vector3.Normalize(Vector3.Cross(velocityDirection, playerUp));
            Vector2 tangent = Vector3.Normalize(Vector3.Cross(groundNormal, rotationAxis));
            return tangent * velocityMagnitude;
        }
    }
}
