using UnityEngine;

namespace GameDevForBeginners
{
    [System.Serializable]
    public class SerializedInterface<T> where T : class
    {
        [SerializeField] private Object _value;
        [HideInInspector]
        [SerializeField] private int _selectedTypeIndex;

        public T value
        {
            get => _value as T;
            set => _value = value as Object;
        }
    }
}