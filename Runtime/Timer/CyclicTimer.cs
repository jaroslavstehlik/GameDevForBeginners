using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CyclicTimer : MonoBehaviour
{
    // Public event when single timer cycle finished
    public UnityEvent onTimerCycleFinished;
    
    // Public event when all timer cycles finished
    public UnityEvent onTimerFinished;
    
    // Duration of timer
    public float duration = 1f;

    // How many times do we want to repeat our timer
    public int cycles = 1;
    
    // Define coroutine so we can later stop it
    private IEnumerator _coroutine;

    // Keep track of the current timer cycle
    private int _currentCycle = 0;
    
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
        // Store coroutine in to variable
        _coroutine = TimerCoroutine();
        
        // Start coroutine
        StartCoroutine(_coroutine);
    }

    public void StopTimer()
    {
        // Stop coroutine
        StopCoroutine(_coroutine);
        
        // Clear variable
        _coroutine = null;
    }

    // The coroutine returns IEnumerator which tells Unity when to stop
    IEnumerator TimerCoroutine()
    {
        // We will execute the body of this cycle until the condition is true
        while (_currentCycle < cycles)
        {
            // Yield means that we want this function to run across multiple frames
            // WaitForSeconds means that the function will wait certain amount of time
            // before it continues execution
            yield return new WaitForSeconds(duration);

            // Check if anyone listens to our event
            if(onTimerCycleFinished != null)
                // Invoke event
                onTimerCycleFinished.Invoke();
            
            // increment curretCycle by 1
            _currentCycle++;
        }
        
        // Check if anyone listens to our event
        if(onTimerFinished != null)
            // Invoke event
            onTimerFinished.Invoke();
    }
}