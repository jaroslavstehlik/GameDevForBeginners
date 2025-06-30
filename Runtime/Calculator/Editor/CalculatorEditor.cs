using System;
using UnityEditor;
using UnityEngine;

namespace GameDevForBeginners
{
    [CustomEditor(typeof(Calculator))]
    public class CalculatorEditor : Editor
    {
        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        private void OnEnable()
        {
            ((Calculator)target).OnValidate();
        }
    }
}