using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TeleportLevel3 : MonoBehaviour
{
    // Our array of tags
    public string[] tags;
    
    // Public events 
    public UnityEvent onTeleport;
    
    // Target where player should teleport
    public Transform targetTransform;
    
    // MonoBehaviour OnTriggerEnter function
    void OnTriggerEnter(Collider other)
    {
        // Lets memorize if a tag has been found
        bool foundTag = false;
        // Iterate over all tags
        foreach (string tag in tags)
        {
            // Check if tag is same
            if (other.CompareTag(tag))
            {
                // we found our tag, set our variable to true 
                foundTag = true;
                // we can stop iterating over all the other tags
                break;
            }
        }
        
        // we have not found object with specific tag!
        if(!foundTag)
            // Terminate function so we dont continue the code execution.
            return;
        
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