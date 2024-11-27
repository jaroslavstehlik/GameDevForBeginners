using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/Timer/BasicTimer")]
    public class BasicTimer : MonoBehaviour
    {
        // Public event when timer finishes
        public UnityEvent onTimerFinished;

        // Duration of timer
        public float duration = 1f;

        // Should the timer repeat
        public bool repeat = false;

        // Define coroutine so we can later stop it
        private IEnumerator coroutine;

        // Monobehaviour calls this method when component is enabled in scene
        void OnEnable()
        {
            StartTimer();
        }

        // Monobehaviour calls this method when component is disabled in scene
        void OnDisable()
        {
            StopTimer();
        }

        public void StartTimer()
        {
            // Store coroutine in to variable
            coroutine = TimerCoroutine();

            // Start coroutine
            StartCoroutine(coroutine);
        }

        public void StopTimer()
        {
            // Stop coroutine
            StopCoroutine(coroutine);

            // Clear variable
            coroutine = null;
        }

        // The coroutine returns IEnumerator which tells Unity when to stop
        IEnumerator TimerCoroutine()
        {
            // Yield means that we want this function to run across multiple frames
            // WaitForSeconds means that the function will wait certain amount of time
            // before it continues execution
            yield return new WaitForSeconds(duration);

            // Check if anyone listens to our event
            if (onTimerFinished != null)
                // Invoke event
                onTimerFinished.Invoke();

            if (repeat)
            {
                StartTimer();
            }
        }
    }
}