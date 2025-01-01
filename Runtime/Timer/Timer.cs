using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/Timer/Timer")]
    public class Timer : MonoBehaviour
    {
        [DrawHiddenFieldsAttribute] [SerializeField]
        private bool _dummy;

        [ShowInInspectorAttribute(false)] private float _elapsedTime = 0;

        // Public event when all timer cycles finished
        public UnityEvent<float> onTimerProgress;

        // Public event when single timer cycle finished
        public UnityEvent<int> onTimerCycleFinished;

        // Public event when all timer cycles finished
        public UnityEvent onTimerFinished;

        // Ignore cycle count and run infinitely
        public bool infiniteCycles = false;

        // How many times do we want to repeat our timer
        public int cycles = 1;

        // Duration of timer
        public float duration = 1f;

        // Define coroutine so we can later stop it
        private IEnumerator _coroutine;

        // Keep track of the current timer cycle
        [ShowInInspectorAttribute(false)] private int _currentCycle = 0;

        // Monobehaviour calls this method when component is enabled in scene
        void OnEnable()
        {
            // Reset cycles
            _currentCycle = 0;

            StartTimer();
        }

        // Monobehaviour calls this method when component is disabled in scene
        void OnDisable()
        {
            StopTimer();
        }

        public void StartTimer()
        {
            _currentCycle = 0;
            _elapsedTime = 0f;
            
            // Store coroutine in to variable
            _coroutine = TimerCoroutine();

            // Start coroutine
            StartCoroutine(_coroutine);
        }

        public void StopTimer()
        {
            _currentCycle = 0;
            _elapsedTime = 0f;
            
            // Stop coroutine
            StopCoroutine(_coroutine);

            // Clear variable
            _coroutine = null;
        }

        // The coroutine returns IEnumerator which tells Unity when to stop
        IEnumerator TimerCoroutine()
        {
            // We will execute the body of this cycle until the condition is true
            while (infiniteCycles || _currentCycle < cycles)
            {
                // We use yield to run this function across multiple frames
                while (_elapsedTime < duration)
                {
                    _elapsedTime += Time.unscaledDeltaTime;
                    if (onTimerProgress != null)
                    {
                        onTimerProgress?.Invoke(_elapsedTime);   
                    }
                    yield return null;
                }

                // increment currentCycle by 1
                _currentCycle++;

                // reset elapsed time
                _elapsedTime = 0f;

                // Check if anyone listens to our event
                if (onTimerCycleFinished != null)
                {
                    // Invoke event
                    onTimerCycleFinished.Invoke(_currentCycle);
                }
                
                if (_currentCycle == cycles)
                {
                    // reset cycles
                    _currentCycle = 0;
                    
                    // Check if anyone listens to our event
                    if (onTimerFinished != null)
                    {
                        // Invoke event
                        onTimerFinished.Invoke();
                    }
                }
            }
        }
    }
}