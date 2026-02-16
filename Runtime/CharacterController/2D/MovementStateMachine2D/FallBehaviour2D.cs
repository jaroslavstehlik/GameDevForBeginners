using UnityEngine;

namespace GameDevForBeginners
{
    // TODO: support velocity
    // TODO: support disable movement, use last velocity from jump
    public class FallBehaviour2D : MonoBehaviour
    {
        const float EPSILON = 0.01f;
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private CollisionState2D _collisionState;
        [SerializeField] private InputController _inputController;
        
        [Header("State")]
        [SerializedInterface(typeof(IState), true)]
        [SerializeField] private SerializedInterface<IState> _movementState = new SerializedInterface<IState>{};
        
        [Header("State Options")]
        [SerializeField] private Option _fallingState;
        [SerializeField] private Option _groundState;

        [Header("Variables")]
        [SerializedInterface(typeof(IState), true)]
        [SerializeField] private SerializedInterface<ICountable> _moveSpeed = new SerializedInterface<ICountable>{};

        [SerializedInterface(typeof(IState), true)]
        [SerializeField] private SerializedInterface<ICountable> _fallSpeed = new SerializedInterface<ICountable>{};
    
        [SerializedInterface(typeof(IState), true)]
        [SerializeField] private SerializedInterface<ICountable> _maxSlopeAngle = new SerializedInterface<ICountable>{};
        
        private Vector2 fallVelocity = Vector2.zero;
        private float MIN_RAMP_DISTANCE = 0.25f;

        void OnEnable()
        {
            _rigidbody.linearVelocity = Vector2.zero;
            fallVelocity = Vector2.zero;
        }

        void FixedUpdate()
        {
            PlayerInput playerInput = _inputController.playerInput;

            CollisionStateInfo2D groundStateInfo = _collisionState.GetGroundStateInfo(_maxSlopeAngle.value.count);
            CollisionInfo2D groundCollisionInfo = groundStateInfo.collisionInfo;

            // when on ground, stop falling
            if (groundCollisionInfo != null && groundCollisionInfo.rampDistance < MIN_RAMP_DISTANCE)
            {
                _movementState.value.activeOption = _groundState;
                return;
            }

            Vector2 playerInputDirection = new Vector2(playerInput.move.x, 0f);
            float playerInputMagnitude = Mathf.Clamp(playerInputDirection.magnitude, 0f, 1f) * _moveSpeed.value.count;
            Vector2 playerMove = playerInputDirection.normalized * playerInputMagnitude;

            fallVelocity += Physics2D.gravity * Time.fixedDeltaTime * _fallSpeed.value.count;
            _rigidbody.linearVelocity = playerMove + fallVelocity;

            _movementState.value.activeOption = _fallingState;
        }

        void OnDisable()
        {
            
        }
    }
}