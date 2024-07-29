using UnityEngine;
using UnityEngine.Events;

public class TeleportLevel2 : MonoBehaviour
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
        Rigidbody rigidBody = other.GetComponent<Rigidbody>();
        // Set its target position
        rigidBody.position = targetTransform.position;
        // Set its target rotation
        rigidBody.rotation = targetTransform.rotation;
        
        // Invoke teleport event
        if(onTeleport != null)
            onTeleport.Invoke();
    }
}