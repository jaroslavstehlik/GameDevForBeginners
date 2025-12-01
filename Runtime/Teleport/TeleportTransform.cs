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
            Transform target = other.transform;
            TransformMarker marker = other.GetComponent<TransformMarker>();
            if (marker != null)
                target = marker.target;
            
            // Modify object which entered trigger
            // Set its target position
            target.position = targetTransform.position;
            // Set its target rotation
            target.rotation = targetTransform.rotation;

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