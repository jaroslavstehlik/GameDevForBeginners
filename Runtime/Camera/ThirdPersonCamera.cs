using UnityEngine;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/Camera/ThirdPersonCamera")]
    public class ThirdPersonCamera : MonoBehaviour
    {
        public Transform target;
        public Vector2 minMaxDistance = new Vector2(0.25f, 5f);
        public float mouseSensitivity = 1f;
        public float mouseScrollWheelSensitivity = 1f;
        public bool flipMouseY = true;
        public float cameraPitch = 0;
        public float cameraDistance = 10f;
        public LayerMask cameraCollisionMask;
        public float cameraRadius = 0.1f;
        public bool useTargetUpDirection = false;

        private static GameObject DEBUG_GO;
        private void LateUpdate()
        {
            float mouseX = Input.GetAxis("Mouse X");
            float yawDelta = mouseX * mouseSensitivity * 10f; 

            float mouseY = Input.GetAxis("Mouse Y");
            if (flipMouseY)
                mouseY *= -1f;
            
            float pitchDelta = mouseY * 10f * mouseSensitivity;
            
            // clamp horizon
            cameraPitch = Mathf.Clamp(cameraPitch + pitchDelta, -90, 90);
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
            
            float mouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");
            cameraDistance = Mathf.Clamp(cameraDistance + mouseScrollWheel * mouseScrollWheelSensitivity,
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
            //transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10f);
            transform.rotation = rotation;
        }
    }
}