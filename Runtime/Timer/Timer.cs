using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/Timer/Timer")]
    public class Timer : MonoBehaviour
    {
        [SerializedInterface(new []{typeof(Counter), typeof(CounterBehaviour)}, true)]
        public SerializedInterface<ICountable> elapsedTime;
        public bool resetOnEnable = true;
        public bool useUnscaledTime = false;

        private void OnEnable()
        {
            if(resetOnEnable)
                elapsedTime.value.count = 0;
        }

        private void Update()
        {
            elapsedTime.value.count += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        }
    }
}