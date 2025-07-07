using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/Counter/CounterListener")]
    public class CounterListener : MonoBehaviour
    {
        [SerializedInterface(new [] {typeof(Counter), typeof(CounterBehaviour)}, true)]
        [SerializeField] private SerializedInterface<ICountable> _counter;

        [SerializeField] private bool _activateOnEnable = true;
        public UnityEvent<float> onCountChanged;

        private void OnEnable()
        {
            if(_counter.value == null)
                return;
            
            _counter.value.onCountChanged?.AddListener(OnCountChanged);
            _counter.value.onDestroy?.AddListener(OnCounterDestroyed);
            if (_activateOnEnable)
                OnCountChanged(_counter.value.count);
        }

        private void OnDisable()
        {
            if(_counter == null || _counter.value == null)
                return;

            _counter.value.onCountChanged?.RemoveListener(OnCountChanged);
            _counter.value.onDestroy?.RemoveListener(OnCounterDestroyed);
        }

        public ICountable counter
        {
            get
            {
                return _counter.value;
            }
            set
            {
                if (_counter.value != null)
                {
                    _counter.value.onCountChanged?.RemoveListener(OnCountChanged);
                    _counter.value.onDestroy?.RemoveListener(OnCounterDestroyed);
                }

                _counter.value = value;

                if (isActiveAndEnabled && _counter != null)
                {
                    _counter.value.onCountChanged?.AddListener(OnCountChanged);
                    _counter.value.onDestroy?.AddListener(OnCounterDestroyed);
                    OnCountChanged(_counter.value.count);
                }
            }
        }
        
        private void OnCountChanged(float count)
        {
            onCountChanged?.Invoke(count);
        }

        private void OnCounterDestroyed(IScriptableValue scriptableValue)
        {
            Counter destroyedCounter = scriptableValue as Counter;
            if(destroyedCounter == null)
                return;
            
            destroyedCounter.onCountChanged?.RemoveListener(OnCountChanged);
            destroyedCounter.onDestroy?.RemoveListener(OnCounterDestroyed);

            if (_counter.value == destroyedCounter)
            {
                _counter = null;
            }
        }
    }
}