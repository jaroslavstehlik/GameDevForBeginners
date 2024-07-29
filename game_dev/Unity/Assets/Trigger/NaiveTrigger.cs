using UnityEngine;
using UnityEngine.Events;

public class BasicTrigger : MonoBehaviour
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