using UnityEngine;
using UnityEngine.Events;

public class TeleportLevel1 : MonoBehaviour
{
    // Public events 
    public UnityEvent onTeleport;
    
    // Target where player should teleport
    public Transform targetTransform;
    
    // MonoBehaviour OnTriggerEnter function
    void OnTriggerEnter(Collider other)
    {
        // Modify object which entered trigger
        // Set its target position
        other.transform.position = targetTransform.position;
        // Set its target rotation
        other.transform.rotation = targetTransform.rotation;
        
        // Invoke teleport event
        if(onTeleport != null)
            onTeleport.Invoke();
    }
}
