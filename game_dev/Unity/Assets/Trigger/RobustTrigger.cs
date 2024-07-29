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