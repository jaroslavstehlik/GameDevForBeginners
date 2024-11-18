using UnityEditor;

namespace GameDevForBeginners
{
    [CustomEditor(typeof(CounterCalculator))]
    public class CounterCalculatorEditor : Editor
    {
        public override bool RequiresConstantRepaint()
        {
            return true;
        }
    }
}