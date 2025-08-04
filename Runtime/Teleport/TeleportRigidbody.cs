using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/Teleport/TeleportRigidbody")]
    public class TeleportRigidbody : MonoBehaviour
    {
        // Public events 
        public UnityEvent onTeleport;

        // Target where player should teleport
        public Transform targetTransform;

        public void Teleport(Collider other)
        {
            // Modify object which entered trigger
            // find rigidbody
            RigidbodyMarker rigidbodyMarker = other.GetComponent<RigidbodyMarker>();
            // If we did not found rigidbody, terminate function
            if (rigidbodyMarker == null)
                return;

            // Set its target position
            rigidbodyMarker.rigidbody.position = targetTransform.position;
            // Set its target rotation
            rigidbodyMarker.rigidbody.rotation = targetTransform.rotation;

            // Invoke teleport event
            if (onTeleport != null)
                onTeleport.Invoke();
        }

// Tell the compiler to use this portion of code only in Unity editor.    
#if UNITY_EDITOR
        // show an editor-only line between teleport origin and teleport destination
        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, targetTransform.position);
        }
#endif
    }
}