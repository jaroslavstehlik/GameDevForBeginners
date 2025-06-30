using System.Collections.Generic;

namespace GameDevForBeginners
{
    struct ConditionDescriptorCache
    {
        private static char[] operatorList = new char[]
        {
            '|',
            '^',
            '&',
            '=',
            '!',
            '>',
            '<',
            '(',
            ')',
            ' '
        };

        int GetOperatorIndex(char letter)
        {
            for (int i = 0; i < operatorList.Length; i++)
            {
                if (operatorList[i] != letter)
                    continue;
                return i;
            }

            return -1;
        }

        private List<string> _cachedCondition;
        private Dictionary<string, IScriptableValue> _variables;

        public ConditionDescriptorCache(string condition, Dictionary<string, IScriptableValue> variables)
        {
            _cachedCondition = new List<string>();
            _variables = variables;

            string buffer = string.Empty;
            string variableBuffer = string.Empty;
            if (!string.IsNullOrEmpty(condition))
            {
                for (int i = 0; i < condition.Length; i++)
                {
                    if (GetOperatorIndex(condition[i]) == -1)
                    {
                        variableBuffer += condition[i];
                        if (buffer != string.Empty)
                        {
                            _cachedCondition.Add(buffer);
                            buffer = string.Empty;
                        }
                    }
                    else
                    {
                        buffer += condition[i];
                        if (variableBuffer != string.Empty)
                        {
                            _cachedCondition.Add(variableBuffer);
                            variableBuffer = string.Empty;
                        }
                    }
                }
            }

            if (buffer != string.Empty)
                _cachedCondition.Add(buffer);

            if (variableBuffer != string.Empty)
                _cachedCondition.Add(variableBuffer);
        }

        public void ReplaceVariablesWithValues(out string replacedString)
        {
            replacedString = string.Empty;
            if (_variables == null)
                return;

            foreach (var variable in _cachedCondition)
            {
                if (_variables.TryGetValue(variable, out IScriptableValue scriptableValue) && scriptableValue != null)
                {
                    replacedString += scriptableValue.GetValue();
                }
                else
                {
                    replacedString += variable;
                }
            }
        }
    }
}