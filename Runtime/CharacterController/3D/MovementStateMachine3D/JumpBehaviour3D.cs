using UnityEngine;

namespace GameDevForBeginners
{
    public class JumpBehaviour3D : MonoBehaviour
    {
        const float EPSILON = 0.01f;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private CollisionState3D _collisionState;
        [SerializeField] private InputController _inputController;

        [Header("State")]
        [SerializedInterface(typeof(IState), true)]
        [SerializeField] private SerializedInterface<IState> _movementState = new SerializedInterface<IState>{};
        
        [Header("State Options")]
        [SerializeField] private Option _fallingState;
        [SerializeField] private Option _jumpState;

        [Header("Variables")]
        [SerializedInterface(typeof(ICountable), true)]
        [SerializeField] private SerializedInterface<ICountable> _moveSpeed = new SerializedInterface<ICountable>{};

        [SerializedInterface(typeof(ICountable), true)]
        [SerializeField] private SerializedInterface<ICountable> _jumpHeight = new SerializedInterface<ICountable>{};

        [SerializedInterface(typeof(ICountable), true)]
        [SerializeField] private SerializedInterface<ICountable> _jumpSpeed = new SerializedInterface<ICountable>{};

        [SerializedInterface(typeof(ICountable), true)]
        [SerializeField] private SerializedInterface<ICountable> _maxSlopeAngle = new SerializedInterface<ICountable>{};
        
    
        private Vector3 _jumpDirection = Vector3.up;
        private Vector3 _jumpVelocity = Vector3.zero;
        private float _startJumpSpeed = 1f;
        private float MIN_RAMP_DISTANCE = 0.25f;

        void OnEnable()
        {
            _startJumpSpeed = _jumpSpeed.value.count;
            float velocityMagnitude = CalculateVelocityFromHeight(Physics.gravity.magnitude * _startJumpSpeed, _jumpHeight.value.count);
            _jumpDirection = -Physics.gravity.normalized;
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
            CollisionStateInfo ceilingStateInfo = _collisionState.GetCeilingStateInfo();
            CollisionInfo collisionInfo = ceilingStateInfo.collisionInfo;
            if (collisionInfo != null && collisionInfo.rampDistance < MIN_RAMP_DISTANCE)
            {
                _movementState.value.activeOption = _fallingState;
                return;
            }
            
            _jumpVelocity += Physics.gravity * Time.fixedDeltaTime * _startJumpSpeed;
            float jumpVelocityDirection = Vector3.Dot(_jumpDirection, _jumpVelocity);
            // Detect when we start falling
            if(jumpVelocityDirection <= 0.0f)
            {
                _movementState.value.activeOption = _fallingState;
                return;
            }
            
            Vector3 playerInputDirection = new Vector3(playerInput.move.x, 0f, playerInput.move.y).normalized;
            float playerInputMagnitude = Mathf.Clamp(playerInputDirection.magnitude, 0f, 1f) * _moveSpeed.value.count;
            Vector3 playerMove = playerInputDirection * playerInputMagnitude;

            Vector3 velocity = _rigidbody.rotation * playerMove + _jumpVelocity;
            _rigidbody.linearVelocity = velocity;

            float cameraYaw = Camera.main.transform.rotation.eulerAngles.y;
            // replace with angular velocity
            _rigidbody.rotation = Quaternion.Euler(0f, cameraYaw, 0f);                                 

            _movementState.value.activeOption = _jumpState;            
        }

        void OnDisable()
        {
            
        }
    }
}
