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
            _counter.onCountChanged?.AddListener(OnCountChanged);
            if (_activateOnEnable)
                OnCountChanged(_counter.count);
        }

        private void OnDisable()
        {
            _counter.onCountChanged?.RemoveListener(OnCountChanged);
        }

        private void OnCountChanged(float count)
        {
            onCountChanged?.Invoke(count);
        }
    }
}