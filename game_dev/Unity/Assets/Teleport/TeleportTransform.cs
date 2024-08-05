using UnityEngine;
using UnityEngine.Events;

public class TeleportTransform : MonoBehaviour
{
    // Public events 
    public UnityEvent onTeleport;
    
    // Target where player should teleport
    public Transform targetTransform;
    
    // MonoBehaviour OnTriggerEnter function
    void OnTriggerEnter(Collider other)
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
        
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, targetTransform.position);
    }
#endif
}
