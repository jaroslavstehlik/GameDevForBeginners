using UnityEngine;

namespace GameDevForBeginners
{
    public class StateAttribute : PropertyAttribute
    {
        public string fieldName;

        public StateAttribute(string fieldName)
        {
            this.fieldName = fieldName;
        }
    }
}