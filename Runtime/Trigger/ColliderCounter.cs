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

        private void Add(int instanceID)
        {
            // Check if collider has been already added
            if (_colliders.Contains(instanceID))
                return;
            
            // Add collider to colliders
            _colliders.Add(instanceID); // InstanceID is a unique identifier
            counter.value.count = _colliders.Count;
        }
        
        public void AddCollider(Collider other)
        {
            Add(other.GetInstanceID());
        }

        public void AddCollider2D(Collider2D other)
        {
            Add(other.GetInstanceID());
        }

        void Remove(int instanceID)
        {
            // Check if collider is in colliders
            if (!_colliders.Contains(instanceID))
                return;
                        
            // Remove that collider
            _colliders.Remove(instanceID);
            counter.value.count = _colliders.Count;
        }
        
        public void RemoveCollider(Collider other)
        {
            Remove(other.GetInstanceID());
        }
        
        public void RemoveCollider2D(Collider2D other)
        {
            Remove(other.GetInstanceID());
        }
    }
}