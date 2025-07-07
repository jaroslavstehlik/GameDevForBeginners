using System.Collections.Generic;
using UnityEngine;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/Trigger/TriggerCounter")]
    public class TriggerCounter : MonoBehaviour
    {
        public Trigger trigger;

        [SerializedInterface(new [] { typeof(Counter), typeof(CounterBehaviour)}, true)]
        public SerializedInterface<ICountable> counter;
        // Remember colliders inside the trigger
        private HashSet<int> _colliders = new HashSet<int>();

        private void OnEnable()
        {
            trigger.onTriggerEnter.AddListener(OnTriggerTriggerEnter);
            trigger.onTriggerExit.AddListener(OnTriggerTriggerExit);
            counter.value.count = _colliders.Count;
        }

        // Clear all colliders when disabling trigger
        private void OnDisable()
        {
            trigger.onTriggerEnter.RemoveListener(OnTriggerTriggerEnter);
            trigger.onTriggerExit.RemoveListener(OnTriggerTriggerExit);
            _colliders.Clear();
            counter.value.count = _colliders.Count;
        }

        void OnTriggerTriggerEnter(Collider other)
        {
            // Check if collider has been already added
            if (!_colliders.Contains(other.GetInstanceID()))
            {
                // Add collider to colliders
                _colliders.Add(other.GetInstanceID()); // InstanceID is a unique identifier
                counter.value.count = _colliders.Count;
            }
        }
        
        void OnTriggerTriggerExit(Collider other)
        {
            // Check if collider is in colliders
            if (_colliders.Contains(other.GetInstanceID()))
            {
                // Remove that collider
                _colliders.Remove(other.GetInstanceID());
                counter.value.count = _colliders.Count;
            }
        }
    }
}