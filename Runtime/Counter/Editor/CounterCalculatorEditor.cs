using System;
using UnityEditor;
using UnityEngine;

namespace GameDevForBeginners
{
    [CustomEditor(typeof(CounterCalculator))]
    public class CounterCalculatorEditor : Editor
    {
        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        private void OnEnable()
        {
            ((CounterCalculator)target).OnValidate();
        }
    }
}