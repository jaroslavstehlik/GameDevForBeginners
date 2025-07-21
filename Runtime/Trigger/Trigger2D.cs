using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/Trigger/Trigger2D")]
    public class Trigger2D : MonoBehaviour
    {
        // Tag attribute renders editor field with available editor tags
        [TagAttribute]
        // Selected tags for filtering
        public string[] filterTags;
        [Space]
        // Public events 
        public UnityEvent<Collider2D> onTriggerEnter;
        public UnityEvent<Collider2D> onTriggerExit;

        // MonoBehaviour OnTriggerEnter function
        void OnTriggerEnter2D(Collider2D other)
        {
            if (filterTags.Length > 0 && !filterTags.Contains(other.tag))
                return;

            // Make sure someone listens to the event
            if (onTriggerEnter != null)
                // Trigger the event
                onTriggerEnter.Invoke(other);
        }

        // MonoBehaviour OnTriggerExit function
        void OnTriggerExit2D(Collider2D other)
        {
            if (filterTags.Length > 0 && !filterTags.Contains(other.tag))
                return;

            // Make sure someone listens to the event
            if (onTriggerExit != null)
                // Trigger the event
                onTriggerExit.Invoke(other);
        }
    }
}