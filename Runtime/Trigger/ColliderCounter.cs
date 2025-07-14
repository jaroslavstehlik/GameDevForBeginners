using System.Collections.Generic;
using UnityEngine;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/Counter/ColliderCounter")]
    public class ColliderCounter : MonoBehaviour
    {
        [SerializedInterface(new [] { typeof(Counter), typeof(CounterBehaviour)}, true)]
        public SerializedInterface<ICountable> counter;
        // Remember colliders inside the trigger
        private HashSet<int> _colliders = new HashSet<int>();

        private void OnEnable()
        {
            counter.value.count = _colliders.Count;
        }

        // Clear all colliders when disabling trigger
        private void OnDisable()
        {
            _colliders.Clear();
            counter.value.count = _colliders.Count;
        }

        public void Add(Collider other)
        {
            // Check if collider has been already added
            if (_colliders.Contains(other.GetInstanceID()))
                return;
            
            // Add collider to colliders
            _colliders.Add(other.GetInstanceID()); // InstanceID is a unique identifier
            counter.value.count = _colliders.Count;
        }
        
        public void Remove(Collider other)
        {
            // Check if collider is in colliders
            if (!_colliders.Contains(other.GetInstanceID()))
                return;
                        
            // Remove that collider
            _colliders.Remove(other.GetInstanceID());
            counter.value.count = _colliders.Count;
        }
    }
}