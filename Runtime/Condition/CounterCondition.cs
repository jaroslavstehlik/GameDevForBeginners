using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using B83.LogicExpressionParser;
using UnityEngine.Serialization;

public enum ParseResultType
{
    False,
    True,
    Error
}

public struct ParseResult
{
    public ParseResultType resultType;
    public string errorMessage;
    
    public ParseResult(ParseResultType resultType, string errorMessage)
    {
        this.resultType = resultType;
        this.errorMessage = errorMessage;
    }
}

[System.Serializable]
public struct CounterConditionDefinition
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
    private string _condition;
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
    
    public ParseResult TryParse()
    {
        _parsedString = _condition;
        foreach (var variable in _variables)
        {
            if(variable == null)
                continue;
            _parsedString = _parsedString.Replace(variable.name, variable.count.ToString());
        }

        Parser parser = new Parser();
        LogicExpression logicExpression = null;
        try
        {
            logicExpression = parser.Parse(_parsedString);
        }
        catch (Exception e)
        {
            return new ParseResult(ParseResultType.Error, e.Message);
        }
        return new ParseResult(logicExpression.GetResult() ? ParseResultType.True : ParseResultType.False, string.Empty);
    }
}

[CreateAssetMenu(fileName = "Counter Condition", menuName = "GMD/Condition/Counter Condition", order = 1)]
public class CounterCondition : ScriptableObject
{
    [SerializeField]
    private CounterConditionDefinition conditionDefinition;
    
    [SerializeField]
    private string _parsedResult = String.Empty;

    [SerializeField]
    private string _conditionResult = String.Empty;

    public UnityEvent onTrue;
    public UnityEvent onFalse;
    public UnityEvent onError;

    private void OnEnable()
    {
        if(!isPlayingOrWillChangePlaymode)
            return;
        conditionDefinition.RegisterVariables(OnCounterChanged);
    }

    private void OnDisable()
    {
        if(!isPlayingOrWillChangePlaymode)
            return;
        conditionDefinition.UnregisterVariables(OnCounterChanged);
    }

    void OnCounterChanged(float value)
    {
        Execute();
    }
    
    public bool Execute()
    {
        ParseResult parseResult = conditionDefinition.TryParse();
        switch (parseResult.resultType)
        {
            case ParseResultType.True:
                onTrue?.Invoke();
                break;
            case ParseResultType.False:
                onFalse?.Invoke();
                break;
            case ParseResultType.Error:
                onError?.Invoke();
                break;
        }
        return parseResult.resultType == ParseResultType.True;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!conditionDefinition.Validate(out string variableName))
        {
            Debug.LogError($"{name}, variable: {variableName} already exists!", this);
        }

        ParseResult parseResult = conditionDefinition.TryParse();
        _parsedResult = conditionDefinition.parsedString;
        _conditionResult = $"{parseResult.resultType.ToString()} {parseResult.errorMessage}";
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
