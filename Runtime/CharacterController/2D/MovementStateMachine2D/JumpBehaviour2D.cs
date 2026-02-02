using UnityEngine;

namespace GameDevForBeginners
{
    public class JumpBehaviour2D : MonoBehaviour
    {
        const float EPSILON = 0.01f;
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private CollisionState2D _collisionState;
        [SerializeField] private InputController _inputController;

        [Header("State")]
        [SerializedInterface(new [] {typeof(State), typeof(StateBehaviour)}, true)]
        [SerializeField] private SerializedInterface<IState> _movementState = new SerializedInterface<IState>{};
        
        [Header("State Options")]
        [SerializeField] private Option _fallingState;
        [SerializeField] private Option _jumpState;

        [Header("Variables")]
        [SerializedInterface(new [] {typeof(Counter), typeof(CounterBehaviour)}, true)]
        [SerializeField] private SerializedInterface<ICountable> _moveSpeed = new SerializedInterface<ICountable>{};

        [SerializedInterface(new [] {typeof(Counter), typeof(CounterBehaviour)}, true)]
        [SerializeField] private SerializedInterface<ICountable> _jumpHeight = new SerializedInterface<ICountable>{};

        [SerializedInterface(new [] {typeof(Counter), typeof(CounterBehaviour)}, true)]
        [SerializeField] private SerializedInterface<ICountable> _jumpSpeed = new SerializedInterface<ICountable>{};

        [SerializedInterface(new [] {typeof(Counter), typeof(CounterBehaviour)}, true)]
        [SerializeField] private SerializedInterface<ICountable> _maxSlopeAngle = new SerializedInterface<ICountable>{};
        
    
        private Vector2 _jumpDirection = Vector2.up;
        private Vector2 _jumpVelocity = Vector2.zero;
        private float _startJumpSpeed = 1f;
        private float MIN_CEILING_DISTANCE = 0.05f;

        void OnEnable()
        {
            _startJumpSpeed = _jumpSpeed.value.count;
            float velocityMagnitude = CalculateVelocityFromHeight(Physics2D.gravity.magnitude * _startJumpSpeed, _jumpHeight.value.count);
            _jumpDirection = -Physics2D.gravity.normalized;
            _jumpVelocity = _jumpDirection * velocityMagnitude;            
        }

        static float CalculateVelocityFromHeight(float gravity, float height)
        {
            return Mathf.Sqrt(2f * gravity * height);
        }

        void FixedUpdate()
        {            
            PlayerInput playerInput = _inputController.playerInput;

            // Test if we got stuck in ceiling
            CollisionStateInfo2D ceilingStateInfo = _collisionState.GetCeilingStateInfo();
            CollisionInfo2D collisionInfo = ceilingStateInfo.collisionInfo;
            if (collisionInfo != null && collisionInfo.rampDistance < MIN_CEILING_DISTANCE)
            {
                _movementState.value.activeOption = _fallingState;
                return;
            }
            
            _jumpVelocity += Physics2D.gravity * Time.fixedDeltaTime * _startJumpSpeed;
            float jumpVelocityDirection = Vector2.Dot(_jumpDirection, _jumpVelocity);
            // Detect when we start falling
            if(jumpVelocityDirection <= 0.0f)
            {
                _movementState.value.activeOption = _fallingState;
                return;
            }
            
            Vector2 playerInputDirection = new Vector2(playerInput.move.x, 0f).normalized;
            float playerInputMagnitude = Mathf.Clamp(playerInputDirection.magnitude, 0f, 1f) * _moveSpeed.value.count;
            Vector2 playerMove = playerInputDirection * playerInputMagnitude;

            Vector2 velocity = playerMove + _jumpVelocity;
            _rigidbody.linearVelocity = velocity;

            _movementState.value.activeOption = _jumpState;            
        }

        void OnDisable()
        {
            
        }
    }
}
