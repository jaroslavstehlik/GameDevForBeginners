using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MathParserTK;

public enum CalculatorResultType
{
    Value,
    Error
}

public struct CalculatorResult
{
    public CalculatorResultType resultType;
    public string errorMessage;
    public float value;
    
    public CalculatorResult(CalculatorResultType resultType, float value, string errorMessage)
    {
        this.resultType = resultType;
        this.value = value;
        this.errorMessage = errorMessage;
    }
}

[System.Serializable]
public struct CounterCalculatorDescriptor
{
    [SerializeField]
    private Counter[] _variables;

    public void RegisterVariables(UnityAction<float> onCounterChanged)
    {
        foreach (var variable in _variables)
        {
            if(variable == null)
                continue;
            variable.onCountChanged.AddListener(onCounterChanged);
        }
    }
    
    public void UnregisterVariables(UnityAction<float> onCounterChanged)
    {
        foreach (var variable in _variables)
        {
            if(variable == null)
                continue;
            variable.onCountChanged.RemoveListener(onCounterChanged);
        }
    }

    public void UnregisterAllVariables()
    {
        foreach (var variable in _variables)
        {
            if(variable == null)
                continue;
            variable.onCountChanged.RemoveAllListeners();
        }
    }
    
    [SerializeField] 
    private string _expression;
    private string _parsedString;
    public string parsedString => _parsedString;

    public bool Validate(out string variableName)
    {
        HashSet<string> encounteredVariables = new HashSet<string>();
        foreach (var variable in _variables)
        {
            if (variable == null)
                continue;
            
            if (!encounteredVariables.Add(variable.name))
            {
                variableName = variable.name;
                return false;
            }
        }

        variableName = string.Empty;
        return true;
    }
    
    public CalculatorResult TryParse()
    {
        _parsedString = _expression;
        foreach (var variable in _variables)
        {
            if(variable == null)
                continue;
            _parsedString = _parsedString.Replace(variable.name, variable.count.ToString());
        }
        
        float result = 0;
        try
        {
            MathParser mathParser = new MathParser();
            result = (float)mathParser.Parse(_parsedString);
        }
        catch (Exception e)
        {
            return new CalculatorResult(CalculatorResultType.Error, 0, e.Message);
        }
        return new CalculatorResult(CalculatorResultType.Value, result, string.Empty);
    }
}

[CreateAssetMenu(fileName = "Counter Calculator", menuName = "GMD/Counter/Calculator", order = 1)]
public class CounterCalculator : ScriptableObject
{
    [DrawHiddenFieldsAttribute] [SerializeField] private bool _dummy;

    [SerializeField]
    private CounterCalculatorDescriptor calculatorDescriptor;
    
    [ShowInInspectorAttribute(false)]
    private string _parsedResult = String.Empty;

    [ShowInInspectorAttribute(false)]
    private string _conditionResult = String.Empty;

    [SerializeField]
    private Counter _result;
    
    private void OnEnable()
    {
        if(!isPlayingOrWillChangePlaymode)
            return;
        calculatorDescriptor.RegisterVariables(OnCounterChanged);
    }

    private void OnDisable()
    {
        if(!isPlayingOrWillChangePlaymode)
            return;
        calculatorDescriptor.UnregisterVariables(OnCounterChanged);
    }

    void OnCounterChanged(float value)
    {
        Execute();
    }
    
    public bool Execute()
    {
        CalculatorResult conditionResult = calculatorDescriptor.TryParse();
        switch (conditionResult.resultType)
        {
            case CalculatorResultType.Value:
                _result.count = conditionResult.value;
                Debug.Log(conditionResult.value);
                break;
            case CalculatorResultType.Error:
                break;
        }
        return conditionResult.resultType == CalculatorResultType.Value;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!calculatorDescriptor.Validate(out string variableName))
        {
            Debug.LogError($"{name}, variable: {variableName} already exists!", this);
        }

        CalculatorResult calculatorResult = calculatorDescriptor.TryParse();
        _parsedResult = calculatorDescriptor.parsedString;
        if (calculatorResult.resultType == CalculatorResultType.Value)
        {
            _conditionResult = $"{calculatorResult.value.ToString()}";    
        }
        else
        {
            _conditionResult = $"{calculatorResult.resultType.ToString()} {calculatorResult.errorMessage}";
        }
    }
#endif
    
    public static bool isPlayingOrWillChangePlaymode
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
