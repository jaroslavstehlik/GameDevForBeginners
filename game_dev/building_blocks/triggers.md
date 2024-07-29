# Triggers
Triggers are very similar to colliders but instead of being a barrier which cant be walked in to, it is a detector which main purpose is to be walked in to or detect that something has entered it. It can detect players, npcs, objects.

Triggers are the most primitive game element we can introduce in a game. From a simple switch which opens some door, to a very complicated puzzles or systems which can very easily keep us occupied for several hours.

***Minecraft trigger trap***\
![minecraft](../../img/minecraft_trap.gif)


What makes a trigger detect stuff?
# Grid
On a grid based game, we can identify a trigger with a specific trigger tile. If we can identify which tile on the grid is for walking and which is not, we can also create another class of a tile which is meant for triggering.

# Colliders
Primitive shapes which describe a specific volume can be also used as triggers in most physics engines. We usually have to use some kind of flag on the collider to make it act as a trigger, otherwise it would be behaving as a wall.

# Events
Every trigger triggers an event when it detects something. An event is simply a link to a specific function. That function is called when we activate the trigger.

# Implementation
Our trigger script needs to inherit from MonoBehaviour in order so it can receive messages about triggers.  

MonoBehaviour class implements three trigger related messages.  
- OnTriggerEnter
- OnTriggerStay
- OnTriggerExit

We will use only OnTriggerEnter and OnTriggerExit for now.  
## Naive trigger

```csharp
using UnityEngine;
using UnityEngine.Events;

public class NaiveTrigger : MonoBehaviour
{
    // Public events 
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;

    // MonoBehaviour OnTriggerEnter function
    void OnTriggerEnter(Collider other)
    {
        // Make sure someone listens to the event
        if(onTriggerEnter != null)
            // Trigger the event
            onTriggerEnter.Invoke();
    }

    // MonoBehaviour OnTriggerExit function
    void OnTriggerExit(Collider other)
    {
        // Make sure someone listens to the event
        if(onTriggerExit != null)
            // Trigger the event
            onTriggerExit.Invoke();
    }
}
```

The code above will do most of its job but it is far from perfect. It is made as simple as possible but does not cover many different cases. One of the case is that if two objects enter and then single object exits we will get this sequence.

```csharp
OnTriggerEnter Collider1 // Enable wall
OnTriggerEnter Collider2 // Enable wall
OnTriggerExit Collider1 // Disable wall
```

If our events would just enable and disable a wall for example, The wall would disable early even that an collider is still inside the trigger, that is because we got an OnTriggerExit early because an collider has exited the trigger. 

- One of the solutions would be to make sure that only a single collider can enter and exit the trigger. But this would be very hard to achieve in the scene.
- The other solution is to remember which colliders have entered and which colliders have left the trigger.
## Robust trigger

```csharp
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RobustTrigger : MonoBehaviour
{
    // Public events 
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;

    // Remember colliders inside the trigger
    private HashSet<int> colliders = new HashSet<int>();
    
    // MonoBehaviour OnTriggerEnter function
    void OnTriggerEnter(Collider other)
    {
        // Remember how many colliders we had before we add new collider
        int collidersCount = colliders.Count;
        
        // Check if collider has been already added
        if (!colliders.Contains(other.GetInstanceID()))
        {
            // Add collider to colliders
            colliders.Add(other.GetInstanceID());
            
            // First collider added to colliders, TriggerEnter now!
            if (collidersCount == 0)
            {
                // Make sure someone listens to the event
                if (onTriggerEnter != null)
                    // Trigger the event
                    onTriggerEnter.Invoke();
            }
        }
    }

    // MonoBehaviour OnTriggerExit function
    void OnTriggerExit(Collider other)
    {
        // Check if collider is in colliders
        if (colliders.Contains(other.GetInstanceID()))
        {
            // Remove that collider
            colliders.Remove(other.GetInstanceID());
            
            // Check if all colliders have left the trigger
            if (colliders.Count == 0)
            {
                // Make sure someone listens to the event
                if (onTriggerExit != null)
                    // Trigger the event
                    onTriggerExit.Invoke();
            }
        }
    }
}
```

This solution triggers enter only when at least single object enters the trigger
and triggers exit only when all objects leave the trigger. 
This trigger is pretty robust towards multiple object detection.
However it might be more useful if we could trigger the events only when certain amount of objects enter or leave the trigger. 

## Counter trigger

```csharp
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CounterTrigger : MonoBehaviour
{
    // Public events 
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;
    
    // Trigger only when certain amount of objects is inside the trigger
    // Create an editor slider with a limited range
    [Range(1, 10)]
    public int minObjectsCount = 1;

    // Remember colliders inside the trigger
    private HashSet<int> colliders = new HashSet<int>();
    
    // MonoBehaviour OnTriggerEnter function
    void OnTriggerEnter(Collider other)
    {
        // Check if collider has been already added
        if (!colliders.Contains(other.GetInstanceID()))
        {
            // Add collider to colliders
            colliders.Add(other.GetInstanceID());
            
            // Did we met our minimum criteria? Trigger! 
            if (colliders.Count == minObjectsCount)
            {
                // Make sure someone listens to the event
                if (onTriggerEnter != null)
                    // Trigger the event
                    onTriggerEnter.Invoke();
            }
        }
    }

    // MonoBehaviour OnTriggerExit function
    void OnTriggerExit(Collider other)
    {
        // Check if collider is in colliders
        if (colliders.Contains(other.GetInstanceID()))
        {
            // Remove that collider
            colliders.Remove(other.GetInstanceID());
            
            // Only when we are one element below our requirement, Trigger! 
            if (colliders.Count == minObjectsCount - 1)
            {
                // Make sure someone listens to the event
                if (onTriggerExit != null)
                    // Trigger the event
                    onTriggerExit.Invoke();
            }
        }
    }
}
```

Our code is now far more complex but can also do more stuff.