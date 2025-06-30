using System;
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

        // Keep track of the current timer cycle
        [ShowInInspectorAttribute(false)] private int _currentCycle = 0;

        // Monobehaviour calls this method when component is enabled in scene
        void OnEnable()
        {
            StartTimer();
        }

        void AdvanceTime()
        {
            _elapsedTime += Time.unscaledDeltaTime;
            if (onTimerProgress != null)
            {
                onTimerProgress?.Invoke(_elapsedTime);
            }
        }
        
        private void Update()
        {
            if (infiniteCycles)
            {
                if (_elapsedTime < duration)
                {
                    AdvanceTime();
                }
                else
                {
                    // Reset timer
                    _elapsedTime = 0f;
                    
                    // Check if anyone listens to our event
                    if (onTimerCycleFinished != null)
                    {
                        // Invoke event
                        onTimerCycleFinished.Invoke(_currentCycle);
                    }
                
                    // Check if anyone listens to our event
                    if (onTimerFinished != null)
                    {
                        // Invoke event
                        onTimerFinished.Invoke();
                    }   
                }
            } else if(_currentCycle > 0)
            {
                if (_elapsedTime < duration)
                {
                    AdvanceTime();
                }
                else
                {
                    // Reset timer
                    _elapsedTime = 0f;
                    
                    // decrement currentCycle by 1
                    _currentCycle--;

                    // Check if anyone listens to our event
                    if (onTimerCycleFinished != null)
                    {
                        // Invoke event
                        onTimerCycleFinished.Invoke(_currentCycle);
                    }

                    if (_currentCycle <= 0)
                    {
                        // Check if anyone listens to our event
                        if (onTimerFinished != null)
                        {
                            // Invoke event
                            onTimerFinished.Invoke();
                        }

                        StopTimer();
                    }
                }
            }
            else
            {
                StopTimer();
            }
        }

        // Monobehaviour calls this method when component is disabled in scene
        void OnDisable()
        {
            StopTimer();
        }

        public void StartTimer()
        {
            _currentCycle = cycles;
            _elapsedTime = 0f;
            enabled = true;
        }

        public void StopTimer()
        {
            _currentCycle = cycles;
            _elapsedTime = 0f;
            enabled = false;
        }
    }
}