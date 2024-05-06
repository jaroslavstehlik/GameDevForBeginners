# Teleports or portals
Teleports and portals can save the player some time or they can transport the player to a totally different place on the map, which can cause disorientation and excitement.

***Portal, infinite portal***\
![portal](../../img/portal.gif)
## Trigger
We first need a trigger which detects the player and maybe our objects as well.

## Teleportation point
Then we need a specific place on the map to which the teleport or portal will transport us.

# Implementation
- Trigger, which detects that player or objects has entered the teleport
- Position, which tells where to teleport our player or object
- Move player to a specific position

## Level 1

```csharp
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

```

This is the simplest teleport we can make. However, it has a major flaw.
If our object contains a RigidBody the teleportation might fail due to collision detection.
When we move RigidBodies around, they still have to detect collisions and if there is a wall in the way, the player might get stuck. Lets make this teleport compatible with RigidBodies then.

## Level 2

```csharp
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
```

Now our teleport actually supports moving RigidBodies around. But we might want to filter only specific objects to be able to teleport. We can use object tag as a filter.

## Level 3

```csharp
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
```