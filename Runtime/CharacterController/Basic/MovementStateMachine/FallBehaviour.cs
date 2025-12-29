using UnityEngine;

namespace GameDevForBeginners
{
    public class FallBehaviour : MonoBehaviour
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
        [SerializeField] private Option _groundState;

        [Header("Variables")]
        [SerializedInterface(new [] {typeof(Counter), typeof(CounterBehaviour)}, true)]
        [SerializeField] private SerializedInterface<ICountable> _moveSpeed = new SerializedInterface<ICountable>{};

        [SerializedInterface(new [] {typeof(Counter), typeof(CounterBehaviour)}, true)]
        [SerializeField] private SerializedInterface<ICountable> _fallSpeed = new SerializedInterface<ICountable>{};
    
        [SerializedInterface(new [] {typeof(Counter), typeof(CounterBehaviour)}, true)]
        [SerializeField] private SerializedInterface<ICountable> _maxSlopeAngle = new SerializedInterface<ICountable>{};
        
        private float fallStartTime;

        void OnEnable()
        {
            fallStartTime = Time.fixedTime;
        }

        void FixedUpdate()
        {            
            PlayerInput playerInput = _inputController.playerInput;

            GroundStateInfo groundStateInfo = _collisionState.GetGroundStateInfo(_rigidbody, _maxSlopeAngle.value.count);
            GroundCollisionInfo groundCollisionInfo = groundStateInfo.groundCollisionInfo;

            if (groundCollisionInfo != null && groundCollisionInfo.isGrounded && !groundCollisionInfo.isTooSteep)
            {
                _movementState.value.activeOption = _groundState;
                return;
            }

            Vector3 playerInputDirection = new Vector3(playerInput.move.x, 0f, playerInput.move.y);
            float playerInputMagnitude = Mathf.Clamp(playerInputDirection.magnitude, 0f, 1f) * _moveSpeed.value.count;
            Vector3 playerMove = playerInputDirection.normalized * playerInputMagnitude;

            float fallDuration = Time.fixedTime - fallStartTime;
            float physicsDuration = 1f;
            float fallProgress = Mathf.Clamp01(fallDuration / physicsDuration);
            float fallAmount = Mathf.Pow(fallProgress, 2f);
            
            Vector3 velocity = _rigidbody.rotation * playerMove + _fallSpeed.value.count * Physics.gravity * fallAmount;
            _rigidbody.linearVelocity = velocity;

            float cameraYaw = Camera.main.transform.rotation.eulerAngles.y;
            _rigidbody.rotation = Quaternion.Euler(0f, cameraYaw, 0f);                                 
            
            _movementState.value.activeOption = _fallingState;
        }

        void OnDisable()
        {
            
        }
    }
}