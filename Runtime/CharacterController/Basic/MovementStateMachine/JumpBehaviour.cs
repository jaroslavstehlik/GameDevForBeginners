using UnityEngine;

namespace GameDevForBeginners
{
    public class JumpBehaviour : MonoBehaviour
    {
        const float EPSILON = 0.01f;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private CollisionState _collisionState;
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
        
        private Vector3 jumpPosition;
        private Vector3 jumpDirection;

        void OnEnable()
        {
            this.jumpPosition = _rigidbody.position;
            this.jumpDirection = -Physics.gravity.normalized;
        }

        float GetCurrentJumpHeight(Vector3 currentPosition)
        {
            return Vector3.Dot(currentPosition - jumpPosition, jumpDirection);
        }

        void FixedUpdate()
        {            
            PlayerInput playerInput = _inputController.playerInput;

            float maxJumpHeight = _jumpHeight.value.count;
            float currentJumpHeight = GetCurrentJumpHeight(_rigidbody.position);

            // Test if we got stuck in ceiling
            CeilingStateInfo ceilingStateInfo = _collisionState.GetCeilingStateInfo();
            if (ceilingStateInfo.sphereCastInfo.collides)
            {
                _rigidbody.linearVelocity = Vector3.zero;
                _movementState.value.activeOption = _fallingState;
                return;
            }
            
            // We reached our jump height, start falling
            if (currentJumpHeight >= maxJumpHeight - EPSILON)
            {
                _rigidbody.linearVelocity = Vector3.zero;
                _movementState.value.activeOption = _fallingState;
                return;
            }
            
            float jumpProgress = Mathf.Clamp01(currentJumpHeight / maxJumpHeight);
            float jumpAmount = 1f - Mathf.Pow(jumpProgress, 2f);

            Vector3 playerInputDirection = new Vector3(playerInput.move.x, 0f, playerInput.move.y).normalized;
            float playerInputMagnitude = Mathf.Clamp(playerInputDirection.magnitude, 0f, 1f) * _moveSpeed.value.count;
            Vector3 playerMove = playerInputDirection * playerInputMagnitude;

            Vector3 velocity = _rigidbody.rotation * playerMove + jumpDirection * _jumpSpeed.value.count * Physics.gravity.magnitude * jumpAmount;
            _rigidbody.linearVelocity = velocity;

            float cameraYaw = Camera.main.transform.rotation.eulerAngles.y;
            _rigidbody.rotation = Quaternion.Euler(0f, cameraYaw, 0f);                                 

            _movementState.value.activeOption = _jumpState;            
        }

        void OnDisable()
        {
            
        }
    }
}
