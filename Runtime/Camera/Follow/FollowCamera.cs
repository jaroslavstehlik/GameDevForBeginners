using UnityEngine;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/Camera/FollowCamera")]
    public class FollowCamera : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset = new Vector3(0f, 10f, 0f);
        public float speed = 0.5f;

        public bool useLocalSpace = false;
        public bool useTargetRotation = false;
        
        private void LateUpdate()
        {
            Vector3 targetPosition = useLocalSpace ? target.TransformPoint(offset) : target.position + offset;
            transform.position = LerpUtils.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
            
            if(useTargetRotation)
                transform.rotation = target.rotation;
        }
    }
}