using UnityEngine;
using UnityEngine.Serialization;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/Camera/ThirdPersonCamera")]
    public class ThirdPersonCamera : MonoBehaviour
    {
        public InputController InputController;
        public Transform target;
        public Vector2 minMaxDistance = new Vector2(1f, 10f);
        public Vector2 minMaxPitch = new Vector2(-89f, 89f);
        public float orbitSpeed = 100f;
        public float zoomSpeed = 0.01f;
        public bool flipMouseY = true;
        public float cameraPitch = 0;
        public float cameraDistance = 10f;
        public LayerMask cameraCollisionMask;
        public float cameraRadius = 0.1f;
        public bool useTargetUpDirection = false;
        
        private PlayerInput _playerInput = new PlayerInput();

        private void OnEnable()
        {
            InputController.onPlayerInputChanged += OnInputChanged;
            Cursor.lockState = CursorLockMode.Locked;
        }

        void OnInputChanged(PlayerInput playerInput)
        {
            _playerInput = playerInput;
        }

        private void LateUpdate()
        {
            float mouseX = _playerInput.look.x * orbitSpeed * Time.deltaTime;
            float mouseY = (flipMouseY ? -_playerInput.look.y : _playerInput.look.y)  * orbitSpeed * Time.deltaTime;
            
            float yawDelta = mouseX; 
            float pitchDelta = mouseY;
            
            // clamp horizon
            cameraPitch = Mathf.Clamp(cameraPitch + pitchDelta, minMaxPitch.x, minMaxPitch.y);
            Vector3 up = Vector3.up;
            if (useTargetUpDirection)
            {
                up = target.up;
            }
           
            Vector3 forward = Vector3.ProjectOnPlane(transform.forward, up).normalized;
            Vector3 right = Vector3.Cross(forward, up);
            Vector3 newRight = Vector3.ProjectOnPlane(transform.forward, right).normalized;
            
            Quaternion rotation = Quaternion.LookRotation(forward, up);
            rotation *= Quaternion.AngleAxis(yawDelta, Vector3.up);
            rotation *= Quaternion.AngleAxis(cameraPitch, Vector3.right);

            Vector3 targetPosition = target.position;
            Vector3 targetDirection = rotation * Vector3.forward;
            
            cameraDistance = Mathf.Clamp(cameraDistance + _playerInput.zoom * zoomSpeed,
                minMaxDistance.x, minMaxDistance.y);

            Ray sphereCastRay = new Ray(targetPosition, -targetDirection);
            RaycastHit raycastHit;
            bool hit = Physics.SphereCast(sphereCastRay, cameraRadius, out raycastHit, minMaxDistance.y,
                cameraCollisionMask,
                QueryTriggerInteraction.Ignore);

            float actualCameraDistance = Vector3.Distance(targetPosition, transform.position);

            float cameraDistanceWithPhysics =
                LerpUtils.Lerp(actualCameraDistance, cameraDistance, Time.deltaTime * 0.1f);
            if (hit)
            {
                cameraDistanceWithPhysics = Mathf.Min(cameraDistance, raycastHit.distance);
            }

            transform.position = targetPosition - targetDirection * cameraDistanceWithPhysics;
            transform.rotation = rotation;
        }

        private void OnDisable()
        {
            InputController.onPlayerInputChanged -= OnInputChanged;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}