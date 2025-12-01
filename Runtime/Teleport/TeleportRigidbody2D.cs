using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/Teleport/TeleportRigidbody2D")]
    public class TeleportRigidbody2D : MonoBehaviour
    {
        // Public events 
        public UnityEvent onTeleport;

        // Target where player should teleport
        public Transform targetTransform;

        public void Teleport(Collider2D other)
        {
            // find rigidbody
            Rigidbody2D rigidbody = other.attachedRigidbody;
            Rigidbody2DMarker marker = other.GetComponent<Rigidbody2DMarker>();
            if (marker != null)
                rigidbody = marker.rigidbody;

            // If we did not found rigidbody, terminate function
            if (rigidbody == null)
                return;

            // Set its target position
            rigidbody.position = targetTransform.position;
            // Set its target rotation
            rigidbody.rotation = targetTransform.rotation.eulerAngles.z;

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