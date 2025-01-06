using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OpType
{
    Unknown,
    And,
    Or,
    Xor,
    Negate,
    Equal,
    InEqual,
    LessThan,
    LessThanEqual,
    GreaterThan,
    GreaterThanEqual,
    BracketOpen,
    BracketClose,
    Quotation
}

public struct Element
{
    public OpType opType;
    public string value;
}

public class LogicParser : MonoBehaviour
{
    [SerializeField] private string _input;
    [SerializeField] private string _output;

    private static Dictionary<char, string[]> charPredictionTable = new Dictionary<char, string[]>()
    {
        {'|', new string[]{"||"}},
        {'^', new string[]{"^"}},
        {'&', new string[]{"&&"}},
        {'=', new string[]{"=="}},
        {'!', new string[]{"!=", "!"}},
        {'>', new string[]{">=", ">"}},
        {'<', new string[]{"<=", "<"}},
        {'(', new string[]{"("}},
        {')', new string[]{")"}},
        {'"', new string[]{"\""}},
    };

    string GetOpString(OpType value)
    {
        Dictionary<OpType, string> table = new Dictionary<OpType, string>
        {
            {OpType.And, "&&"},
            {OpType.Or, "||"},
            {OpType.Xor, "^"},
            {OpType.LessThan, "<"},
            {OpType.LessThanEqual, "<="},
            {OpType.Equal, "=="},
            {OpType.InEqual, "!="},
            {OpType.GreaterThanEqual, ">="},
            {OpType.GreaterThan, ">"},
            {OpType.Negate, "!"},
            {OpType.BracketOpen, "("},
            {OpType.BracketClose, ")"},
            {OpType.Quotation, "\""},
        };

        if (table.TryGetValue(value, out string opString))
        {
            return opString;
        }

        return string.Empty;
    }

    OpType GetOpType(string value)
    {
        Dictionary<string, OpType> table = new Dictionary<string, OpType>
        {
            {"&&", OpType.And},
            {"||", OpType.Or},
            {"^", OpType.Xor},
            {"<", OpType.LessThan},
            {"<=", OpType.LessThanEqual},
            {"==", OpType.Equal},
            {"!=", OpType.InEqual},
            {">=", OpType.GreaterThanEqual},
            {">", OpType.GreaterThan},
            {"!", OpType.Negate},
            {"(", OpType.BracketOpen},
            {")", OpType.BracketClose},
            {"\"", OpType.Quotation},
        };

        if (table.TryGetValue(value, out OpType opType))
        {
            return opType;
        }

        return OpType.Unknown;
    }

    static void ConsumeUnknown(ref string unknown, List<Element> elements, bool removeSpaces)
    {
        unknown = removeSpaces ? unknown.Replace(" ", string.Empty) : unknown;
        if (unknown != string.Empty)
        {
            elements.Add(new Element()
            {
                opType = OpType.Unknown,
                value = unknown
            });
            unknown = string.Empty;
        }
    }
    
    List<Element> SplitElements(string input)
    {
        List<Element> elements = new List<Element>();

        string unknown = string.Empty;
        bool insideQuotationMarks = false;
        for (int i = 0; i < input.Length; i++)
        {
            if (charPredictionTable.TryGetValue(input[i], out string[] options))
            {
                if (insideQuotationMarks)
                {
                    if (input[i] == '"')
                    {
                        ConsumeUnknown(ref unknown, elements, false);
                        elements.Add(new Element()
                        {
                            opType = OpType.Quotation,
                            value = "\""
                        });
                        insideQuotationMarks = false;
                        continue;
                    }
                    
                    unknown += input[i];
                    continue;
                }

                ConsumeUnknown(ref unknown, elements, true);
                
                foreach (var option in options)
                {
                    string tryMatch = string.Empty;
                    if (i + option.Length < input.Length)
                    {
                        tryMatch = input.Substring(i, option.Length);
                    }

                    OpType opType = GetOpType(tryMatch);
                    if (opType != OpType.Unknown)
                    {
                        if (opType == OpType.Quotation)
                            insideQuotationMarks = true;
                        
                        elements.Add(new Element()
                        {
                            opType = GetOpType(tryMatch),
                            value = tryMatch
                        });
                        
                        i += tryMatch.Length - 1;
                        break;
                    }
                }
            }
            else
            {
                unknown += input[i];
            }
        }
        
        ConsumeUnknown(ref unknown, elements, !insideQuotationMarks);
        return elements;
    }

    private void OnDrawGizmos()
    {
        _output = string.Empty;
        List<Element> elements = SplitElements(_input);
        // replace variables with values
        // ignore variables inside quotation marks
        foreach (var element in elements)
        {
            _output += element.value + ",";
        }
    }
}
