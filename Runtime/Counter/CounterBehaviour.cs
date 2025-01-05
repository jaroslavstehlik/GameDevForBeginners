using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/Counter/CounterBehaviour")]
    public class CounterBehaviour : MonoBehaviour, ICountable
    {

    /*
     * TODO:
     * Scriptable Objects can be created during runtime.
     * If we create scriptable objects during runtime, we can reference them everywhere
     * during runtime as well, without the need to differentiate runtime and non runtime objects.
     * The complexity arises from several elements.
     * Runtime scriptable objects need descriptors in order to have initial state.
     * In order so they can be created during runtime, mono behaviour has to create them.
     * In order to properly link runtime scriptable objects to other scriptable objects we need
     * to dynamically link them after they have been properly initialized with a descriptor.
     * How does the dynamic linking actually look like? We cant reference the scriptable object it self
     * before it has been created during runtime, so we need to identify them ahead of time differently.
     * Probably identifying them by their owner who has created them.
     *
     *
     * The objects which would benefit from runtime instancing.
     * Counters, Counter conditions, Counter calculators
     * States, State conditions,
     *
     * Counter condition and counter calculator can reference runtime and non runtime objects.
     * State conditions can reference runtime and non runtime objects as well.
     */

    // Counter behaviour
    // Descriptor
    // Injector

        [DrawHiddenFieldsAttribute] [SerializeField]
        private bool _dummy;

        [SerializeField] private CounterDescriptor _counterDescriptor = new CounterDescriptor()
        {
            name = string.Empty,
            defaultCount = 0,
            saveKey = string.Empty,
            wholeNumber = true
        };
        
        [SerializeField] private UnityEvent<Counter> _onCounterCreated;
        public UnityEvent<Counter> onCounterCreated => _onCounterCreated;
        
        [SerializeField] private UnityEvent<float> _onCountChanged;
        public UnityEvent<float> onCountChanged => _onCountChanged;
        
        private Counter _counter;
        public Counter counter => _counter;
        
        private Counter GetOrCreateCounter()
        {
            if (_counter == null)
            {
                _counter = Counter.CreateCounter(_counterDescriptor);
                _onCounterCreated?.Invoke(_counter);
            }

            return _counter;
        }

        private void Awake()
        {
            _ = GetOrCreateCounter();
        }

        private void OnDestroy()
        {
            Destroy(_counter);
            _counter = null;
        }

        public float defaultCount
        {
            get
            {
                if (_counter == null)
                    return 0;
                
                return _counter.defaultCount;
            }
        }

        public bool wholeNumber
        {
            get
            {
                if (_counter == null)
                    return false;

                return _counter.wholeNumber;
            }
        }
        
        public float count
        {
            get
            {
                if (_counter == null)
                    return 0;
                
                return _counter.count;
            }
            set
            {
                if (_counter == null)
                    return;

                _counter.count = value;
            }
        }

        // Method for reading count
        public float Get()
        {
            return count;
        }

        // Method for writing count
        public void Set(float value)
        {
            count = value;
        }

        public void Set(Counter counter)
        {
            count = counter.count;
        }

        // Method for adding count
        public void Add(float value)
        {
            count += value;
        }

        public void Add(Counter counter)
        {
            count += counter.count;
        }

        // Method for subtracting count
        public void Subtract(float value)
        {
            count -= value;
        }

        public void Subtract(Counter counter)
        {
            count -= counter.count;
        }

        // Method for multiplying count
        public void Multiply(float value)
        {
            count *= value;
        }

        public void Multiply(Counter counter)
        {
            count *= counter.count;
        }

        // Method for dividing count
        public void Divide(float value)
        {
            count /= value;
        }

        public void Divide(Counter counter)
        {
            count /= counter.count;
        }

        public void Reset()
        {
            if(_counter != null)
                count = _counter.defaultCount;
        }
    }
}