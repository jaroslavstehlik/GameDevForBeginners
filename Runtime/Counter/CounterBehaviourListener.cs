using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/Counter/CounterBehaviourListener")]
    public class CounterBehaviourListener : MonoBehaviour
    {
        [SerializeField] private CounterBehaviour _counterBehaviour;
        [SerializeField] private bool _activateOnEnable = true;
        public UnityEvent<float> onCountChanged;

        private void OnEnable()
        {
            _counterBehaviour.onCountChanged?.AddListener(OnCountChanged);
            if (_activateOnEnable)
                OnCountChanged(_counterBehaviour.count);
        }

        private void OnDisable()
        {
            _counterBehaviour.onCountChanged?.RemoveListener(OnCountChanged);
        }

        private void OnCountChanged(float count)
        {
            onCountChanged?.Invoke(count);
        }
    }
}