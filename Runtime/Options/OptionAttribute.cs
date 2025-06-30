using UnityEngine;

namespace GameDevForBeginners
{
    public class OptionAttribute : PropertyAttribute
    {
        public string fieldName;

        public OptionAttribute(string fieldName)
        {
            this.fieldName = fieldName;
        }
    }
}