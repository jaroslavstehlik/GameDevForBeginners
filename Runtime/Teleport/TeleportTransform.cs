using UnityEngine;
using UnityEngine.Events;

public class TeleportTransform : MonoBehaviour
{
    // Public events 
    public UnityEvent onTeleport;
    
    // Target where player should teleport
    public Transform targetTransform;

    public void Teleport(Collider other)
    {
        TeleportableTransform teleportableTransform = other.GetComponent<TeleportableTransform>();
        // If we did not found transform, terminate function
        if(teleportableTransform == null)
            return;

        // Modify object which entered trigger
        // Set its target position
        teleportableTransform.target.position = targetTransform.position;
        // Set its target rotation
        teleportableTransform.target.rotation = targetTransform.rotation;
        
        // Invoke teleport event
        if(onTeleport != null)
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
