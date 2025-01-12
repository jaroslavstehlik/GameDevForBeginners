using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/Counter/CounterListener")]
    public class CounterListener : MonoBehaviour
    {
        [SerializeField] private Counter _counter;
        [SerializeField] private bool _activateOnEnable = true;
        public UnityEvent<float> onCountChanged;

        private void OnEnable()
        {
            if(_counter == null)
                return;
            
            _counter.onCountChanged?.AddListener(OnCountChanged);
            _counter.onDestroy?.AddListener(OnCounterDestroyed);
            if (_activateOnEnable)
                OnCountChanged(_counter.count);
        }

        private void OnDisable()
        {
            if(_counter == null)
                return;

            _counter.onCountChanged?.RemoveListener(OnCountChanged);
            _counter.onDestroy?.RemoveListener(OnCounterDestroyed);
        }

        public Counter counter
        {
            get
            {
                return _counter;
            }
            set
            {
                if (_counter != null)
                {
                    _counter.onCountChanged?.RemoveListener(OnCountChanged);
                    _counter.onDestroy?.RemoveListener(OnCounterDestroyed);
                }

                _counter = value;

                if (isActiveAndEnabled && _counter != null)
                {
                    _counter.onCountChanged?.AddListener(OnCountChanged);
                    _counter.onDestroy?.AddListener(OnCounterDestroyed);
                    OnCountChanged(_counter.count);
                }
            }
        }
        
        private void OnCountChanged(float count)
        {
            onCountChanged?.Invoke(count);
        }

        private void OnCounterDestroyed(ScriptableValue scriptableValue)
        {
            Counter destroyedCounter = scriptableValue as Counter;
            if(destroyedCounter == null)
                return;
            
            destroyedCounter.onCountChanged?.RemoveListener(OnCountChanged);
            destroyedCounter.onDestroy?.RemoveListener(OnCounterDestroyed);

            if (_counter == destroyedCounter)
            {
                _counter = null;
            }
        }
    }
}