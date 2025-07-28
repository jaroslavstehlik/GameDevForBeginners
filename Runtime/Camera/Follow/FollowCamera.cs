using UnityEngine;
using UnityEngine.Serialization;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/Camera/FollowCamera")]
    public class FollowCamera : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset = new Vector3(0f, 0f, 0f);
        public bool smoothMovement = true;
        public float speed = 0.5f;

        public bool useLocalSpace = false;
        public bool useTargetRotation = false;
        
        private void LateUpdate()
        {
            Vector3 targetPosition = useLocalSpace ? target.TransformPoint(offset) : target.position + offset;
            Vector3 cameraPosition = smoothMovement
                ? LerpUtils.Lerp(transform.position, targetPosition, Time.deltaTime * speed)
                : targetPosition;
            transform.position = cameraPosition;

            if(useTargetRotation)
                transform.rotation = target.rotation;
        }
    }
}