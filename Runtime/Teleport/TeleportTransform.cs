using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/Teleport/TeleportTransform")]
    public class TeleportTransform : MonoBehaviour
    {
        // Public events 
        public UnityEvent onTeleport;

        // Target where player should teleport
        public Transform targetTransform;

        public void Teleport(Collider other)
        {
            TransformMarker transformMarker = other.GetComponent<TransformMarker>();
            // If we did not found transform, terminate function
            if (transformMarker == null)
                return;

            // Modify object which entered trigger
            // Set its target position
            transformMarker.target.position = targetTransform.position;
            // Set its target rotation
            transformMarker.target.rotation = targetTransform.rotation;

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