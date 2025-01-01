using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
    public enum CounterTriggerCondition
    {
        Equal,
        NotEqual,
        LessThan,
        LessThanEqual,
        GreaterThan,
        GreaterThanEqual,
    }

    [AddComponentMenu("GMD/Trigger/CounterTrigger")]
    public class CounterTrigger : MonoBehaviour
    {
        // Condition which ensures when the trigger should be triggered
        public CounterTriggerCondition condition;

        // Trigger only when certain amount of objects is inside the trigger
        public int minObjectsCount = 1;

        // Tag attribute renders editor field with available editor tags
        [TagAttribute]
        // Selected tags for filtering
        public string[] filterTags;

        // Public events 
        public UnityEvent onTriggerEnter;
        public UnityEvent onTriggerExit;

        // Remember colliders inside the trigger
        private HashSet<int> _colliders = new HashSet<int>();

        // Clear all colliders when disabling trigger
        private void OnDisable()
        {
            _colliders.Clear();
        }

        bool IsConditionMet(int a, int b)
        {
            switch (condition)
            {
                case CounterTriggerCondition.Equal:
                    return a == b;
                case CounterTriggerCondition.NotEqual:
                    return a == b;
                case CounterTriggerCondition.GreaterThan:
                    return a > b;
                case CounterTriggerCondition.GreaterThanEqual:
                    return a >= b;
                case CounterTriggerCondition.LessThan:
                    return a < b;
                case CounterTriggerCondition.LessThanEqual:
                    return a <= b;
            }

            return false;
        }

        // MonoBehaviour OnTriggerEnter function
        void OnTriggerEnter(Collider other)
        {
            if (filterTags.Length > 0 && !filterTags.Contains(other.tag))
                return;

            // Check if collider has been already added
            if (!_colliders.Contains(other.GetInstanceID()))
            {
                // Add collider to colliders
                _colliders.Add(other.GetInstanceID());

                // Did we met our minimum criteria? Trigger! 
                if (IsConditionMet(_colliders.Count, minObjectsCount))
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
            if (filterTags.Length > 0 && !filterTags.Contains(other.tag))
                return;

            // Check if collider is in colliders
            if (_colliders.Contains(other.GetInstanceID()))
            {
                // Remove that collider
                _colliders.Remove(other.GetInstanceID());

                // Only when we are one element below our requirement, Trigger! 
                if (!IsConditionMet(_colliders.Count, minObjectsCount))
                {
                    // Make sure someone listens to the event
                    if (onTriggerExit != null)
                        // Trigger the event
                        onTriggerExit.Invoke();
                }
            }
        }
    }
}