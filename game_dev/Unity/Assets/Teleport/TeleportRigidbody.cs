using System;
using UnityEngine;
using UnityEngine.Events;

public class TeleportRigidbody : MonoBehaviour
{
    // Public events 
    public UnityEvent onTeleport;
    
    // Target where player should teleport
    public Transform targetTransform;
    
    // MonoBehaviour OnTriggerEnter function
    void OnTriggerEnter(Collider other)
    {
        // Modify object which entered trigger
        // find rigidbody
        TeleportableRigidbody teleportableRigidbody = other.GetComponent<TeleportableRigidbody>();
        // If we did not found rigidbody, terminate function
        if(teleportableRigidbody == null)
            return;

        // Set its target position
        teleportableRigidbody.rigidbody.position = targetTransform.position;
        // Set its target rotation
        teleportableRigidbody.rigidbody.rotation = targetTransform.rotation;
        
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