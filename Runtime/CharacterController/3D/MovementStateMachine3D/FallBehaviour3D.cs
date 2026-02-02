using UnityEngine;

namespace GameDevForBeginners
{
    // TODO: support velocity
    // TODO: support disable movement, use last velocity from jump
    public class FallBehaviour3D : MonoBehaviour
    {
        const float EPSILON = 0.01f;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private CollisionState3D _collisionState;
        [SerializeField] private InputController _inputController;
        
        [Header("State")]
        [SerializedInterface(new [] {typeof(State), typeof(StateBehaviour)}, true)]
        [SerializeField] private SerializedInterface<IState> _movementState = new SerializedInterface<IState>{};
        
        [Header("State Options")]
        [SerializeField] private Option _fallingState;
        [SerializeField] private Option _groundState;

        [Header("Variables")]
        [SerializedInterface(new [] {typeof(Counter), typeof(CounterBehaviour)}, true)]
        [SerializeField] private SerializedInterface<ICountable> _moveSpeed = new SerializedInterface<ICountable>{};

        [SerializedInterface(new [] {typeof(Counter), typeof(CounterBehaviour)}, true)]
        [SerializeField] private SerializedInterface<ICountable> _fallSpeed = new SerializedInterface<ICountable>{};
    
        [SerializedInterface(new [] {typeof(Counter), typeof(CounterBehaviour)}, true)]
        [SerializeField] private SerializedInterface<ICountable> _maxSlopeAngle = new SerializedInterface<ICountable>{};
        
        private Vector3 fallVelocity = Vector3.zero;
        private float MIN_RAMP_DISTANCE = 0.25f;

        void OnEnable()
        {
            _rigidbody.linearVelocity = Vector3.zero;
            fallVelocity = Vector3.zero;
        }

        void FixedUpdate()
        {            
            PlayerInput playerInput = _inputController.playerInput;

            CollisionStateInfo groundStateInfo = _collisionState.GetGroundStateInfo(_maxSlopeAngle.value.count);
            CollisionInfo groundCollisionInfo = groundStateInfo.collisionInfo;

            // when on ground, stop falling
            if (groundCollisionInfo != null && groundCollisionInfo.rampDistance <= MIN_RAMP_DISTANCE)
            {
                _movementState.value.activeOption = _groundState;
                return;
            }

            Vector3 playerInputDirection = new Vector3(playerInput.move.x, 0f, playerInput.move.y);
            float playerInputMagnitude = Mathf.Clamp(playerInputDirection.magnitude, 0f, 1f) * _moveSpeed.value.count;
            Vector3 playerMove = playerInputDirection.normalized * playerInputMagnitude;

            fallVelocity += Physics.gravity * Time.fixedDeltaTime * _fallSpeed.value.count;
            Vector3 moveVelocity = _rigidbody.rotation * playerMove;
            _rigidbody.linearVelocity = moveVelocity + fallVelocity;

            float cameraYaw = Camera.main.transform.rotation.eulerAngles.y;
            // replace with angular velocity
            _rigidbody.rotation = Quaternion.Euler(0f, cameraYaw, 0f);                                 
            
            _movementState.value.activeOption = _fallingState;
        }

        void OnDisable()
        {
            
        }
    }
}