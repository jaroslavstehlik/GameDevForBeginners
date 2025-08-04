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
            // Modify object which entered trigger
            // find rigidbody
            Rigidbody2DMarker rigidbody2DMarkerRigidbody = other.GetComponent<Rigidbody2DMarker>();
            // If we did not found rigidbody, terminate function
            if (rigidbody2DMarkerRigidbody == null)
                return;

            // Set its target position
            rigidbody2DMarkerRigidbody.rigidbody.position = targetTransform.position;
            // Set its target rotation
            rigidbody2DMarkerRigidbody.rigidbody.rotation = targetTransform.rotation.eulerAngles.z;

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