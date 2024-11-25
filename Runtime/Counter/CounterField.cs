using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GameDevForBeginners
{
    [System.Serializable]
    public struct CounterField : ICountable
    {
        [SerializeField] private Counter _reference;
        [SerializeField] private float _count;
        [HideInInspector] public UnityEvent<float> onCountChanged;

        public float defaultCount { get; }
        public bool wholeNumber { get; }

        public float count {
            get
            {
                if (_reference != null)
                {
                    return _reference.count;
                }

                return _count;
            }
            set
            {
                if (_reference != null)
                {
                    _reference.count = value;
                    return;
                }

                _count = value;
            }
        }

        public float Get()
        {
            return count;
        }

        public void Set(float value)
        {
            count = value;
        }

        public void Set(Counter counter)
        {
            count = counter.count;
        }

        public void Add(float value)
        {
            count += value;
        }

        public void Add(Counter counter)
        {
            count += counter.count;
        }

        public void Subtract(float value)
        {
            count -= value;
        }

        public void Subtract(Counter counter)
        {
            count -= counter.count;
        }

        public void Multiply(float value)
        {
            count *= value;
        }

        public void Multiply(Counter counter)
        {
            count *= counter.count;
        }

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
            count = defaultCount;
        }
    }
}