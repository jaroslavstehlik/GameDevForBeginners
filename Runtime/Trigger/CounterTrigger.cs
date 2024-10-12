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
    
    // Clear all colliders when disabling trigger
    private void OnDisable()
    {
        colliders.Clear();
    }

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