using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// This field tells UnityEditor to create an asset menu
// which creates a new scriptable object in project.
[CreateAssetMenu(fileName = "Counter", menuName = "GMD/Counter/Counter", order = 1)]

// Scriptable object can be stored only in project
// it can be referenced in scene
// it is used mostly for holding game data
public class Counter : ScriptableObject
{
    [DrawHiddenFieldsAttribute] [SerializeField] private bool _dummy;
    
    [ShowInInspectorAttribute(false)]
    private float _count = 0;
    
    [SerializeField] private float _defaultCount = 0;
    [SerializeField] private bool _wholeNumber = true;
    // The key to our counter, it has to be unique per whole game.
    [SerializeField] private string _saveKey = string.Empty;
    // public event
    [HideInInspector]
    public UnityEvent<float> onCountChanged;

    private DetectInfiniteLoop _detectInfiniteLoop = new DetectInfiniteLoop();
    
    private void OnEnable()
    {
        // Check if any counter has been saved before
        if (isPlayingOrWillChangePlaymode &&
            !string.IsNullOrEmpty(_saveKey) && 
            PlayerPrefs.HasKey(_saveKey))
        {
            // Load the counter in to our variable
            count = PlayerPrefs.GetFloat(_saveKey);
        }
        else
        {
            count = _defaultCount;   
        }
    }

    private void OnDisable()
    {
        count = _defaultCount;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _defaultCount = ValidateNumber(_defaultCount);
    }
#endif

    float ValidateNumber(float value)
    {
        return _wholeNumber ? (int)value : value;
    }
    
    public float count
    {
        get
        {
            return _count;
        }
        set
        {
            float candidate = ValidateNumber(value);
            if(_count == candidate)
                return;
            
            _count = candidate;

            if(!isPlayingOrWillChangePlaymode)
                return;

            if(!string.IsNullOrEmpty(_saveKey))
                PlayerPrefs.SetFloat(_saveKey, count);

            if(!_detectInfiniteLoop.Detect(this))
                onCountChanged?.Invoke(_count);
        }
    }

    // Method for reading count
    public float Get()
    {
        return count;
    }

    // Method for writing count
    public void Set(float value)
    {
        count = value;
    }
    
    public void Set(Counter counter)
    {
        count = counter.count;
    }

    // Method for adding count
    public void Add(float value)
    {
        count += value;
    }
    
    public void Add(Counter counter)
    {
        count += counter.count;
    }

    // Method for subtracting count
    public void Subtract(float value)
    {
        count -= value;
    }
    
    public void Subtract(Counter counter)
    {
        count -= counter.count;
    }
    
    // Method for multiplying count
    public void Multiply(float value)
    {
        count *= value;
    }

    public void Multiply(Counter counter)
    {
        count *= counter.count;
    }

    // Method for dividing count
    public void Divide(float value)
    {
        count /= value;
    }
    
    public void Divide(Counter counter)
    {
        count /= counter.count;
    }

    static bool isPlayingOrWillChangePlaymode
    {
        get
        {
            #if UNITY_EDITOR
            return UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode;
            #else
            return true;
            #endif
        }
    }
}