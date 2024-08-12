using UnityEngine;
using UnityEngine.Events;

public class NaiveTrigger : MonoBehaviour
{
    // Public events 
    public UnityEvent<Collider> onTriggerEnter;
    public UnityEvent<Collider> onTriggerExit;

    // MonoBehaviour OnTriggerEnter function
    void OnTriggerEnter(Collider other)
    {
        // Make sure someone listens to the event
        if(onTriggerEnter != null)
            // Trigger the event
            onTriggerEnter.Invoke(other);
    }

    // MonoBehaviour OnTriggerExit function
    void OnTriggerExit(Collider other)
    {
        // Make sure someone listens to the event
        if(onTriggerExit != null)
            // Trigger the event
            onTriggerExit.Invoke(other);
    }
}