[Main page](../../readme.md)

# Teleports or portals
Teleports and portals can save the player some time or they can transport the player to a totally different place on the map, which can cause disorientation and excitement.

***Portal, infinite portal***\
<img src="../../img/portal.gif" alt="portal" height="400"/>
## Trigger
We first need a trigger which detects the player and maybe our objects as well.

## Teleportation point
Then we need a specific place on the map to which the teleport or portal will transport us.

# Implementation
- Trigger, which detects that player or objects has entered the teleport
- Position, which tells where to teleport our player or object
- Move player to a specific position

## Teleport Transform

```csharp
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
```

This is the simplest teleport we can make. However, it has a major flaw.
If our object contains a RigidBody the teleportation might fail due to collision detection.
When we move RigidBodies around, they still have to detect collisions and if there is a wall in the way, the player might get stuck. Lets make this teleport compatible with RigidBodies then.

## Teleport Rigidbody

```csharp
using UnityEngine;
using UnityEngine.Events;

public class TeleportRigidbody : MonoBehaviour
{
    // Public events 
    public UnityEvent onTeleport;
    
    // Target where player should teleport
    public Transform targetTransform;
    
    public void Teleport(Collider other)
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
    
// Tell the compiler to use this portion of code only in Unity editor.    
#if UNITY_EDITOR
    // show an editor-only line between teleport origin and teleport destination
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, targetTransform.position);
    }
#endif
}
```

Now our teleport actually supports moving RigidBodies around. But we might want to filter only specific objects to be able to teleport. We can use object tag as a filter.

## Teleport Tagged Rigidbody

```csharp
using UnityEngine;
using UnityEngine.Events;

public class TeleportTaggedRigidbody : MonoBehaviour
{
    // Our array of tags
    public string[] tags;
    
    // Public events 
    public UnityEvent onTeleport;
    
    // Target where player should teleport
    public Transform targetTransform;
    
    public void Teleport(Collider other)
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
        TeleportableRigidbody teleportableRigidbody = other.GetComponent<TeleportableRigidbody>();
        if(teleportableRigidbody == null)
            // If we did not found rigidbody, terminate function
            return;
        
        // Set its target position
        teleportableRigidbody.rigidbody.position = targetTransform.position;
        // Set its target rotation
        teleportableRigidbody.rigidbody.rotation = targetTransform.rotation;
        
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
```