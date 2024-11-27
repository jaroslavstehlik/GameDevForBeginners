using UnityEngine;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/Character/RotateRendererBasedOnVelocity")]
    public class RotateRendererBasedOnVelocity : MonoBehaviour
    {
        public Transform rendererTransform;
        public Rigidbody rigidbody;

        private void Update()
        {
            Vector3 velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);
            if (velocity.sqrMagnitude > 0f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(velocity);
                rendererTransform.rotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);
            }
        }
    }
}