using System.Collections.Generic;
using System;
using System.Globalization;

[Serializable]
public class Formula
{
    private List<Bracket> _brackets;

    public Formula(string data)
    {
        var chars = data.ToCharArray();

        _brackets = new List<Bracket>();
        var currentBracketIndex = 1;
        var bracket = new Bracket(0, 0);
        _brackets.Add(bracket);
        var parameterKind = new List<char>();
        for (int i = 0; i < chars.Length; i++)
        {
            switch (chars[i])
            {
                case '+': bracket.AddTemplateValue(parameterKind); parameterKind.Clear(); bracket.AddOperation(EOperations.Plus); break;
                case '-': bracket.AddTemplateValue(parameterKind); parameterKind.Clear(); bracket.AddOperation(EOperations.Minus); break;
                case '*': bracket.AddTemplateValue(parameterKind); parameterKind.Clear(); bracket.AddOperation(EOperations.Multiply); break;
                case '/': bracket.AddTemplateValue(parameterKind); parameterKind.Clear(); bracket.AddOperation(EOperations.Divide); break;
                case '(':
                    bracket.AddTemplateValue(parameterKind); parameterKind.Clear();
                    var current = bracket.CurrentIndex;
                    bracket = new Bracket(current, currentBracketIndex); currentBracketIndex++;
                    _brackets.Add(bracket); break;
                case ')':
                    bracket.AddTemplateValue(parameterKind); parameterKind.Clear();
                    _brackets[bracket.ParentIndex].AddBracketValue(bracket.CurrentIndex);
                    bracket = _brackets[bracket.ParentIndex];
                    break;
                default: parameterKind.Add(chars[i]); break;
            }
        }
        bracket.AddTemplateValue(parameterKind);
    }

    public float GetResult(Dictionary<string, ParameterTemplate<float>> attacker, Dictionary<string, ParameterTemplate<float>> target, Dictionary<string, float> resultAttacker, Dictionary<string, float> resultTarget)
    {
        return _brackets[0].GetValue(attacker, target, resultAttacker, resultTarget, _brackets);
    }

    [Serializable]
    private class Bracket
    {
        private List<EOperations> _operations;
        private List<FormulaParameter> _parameters;
        public int ParentIndex { get; private set; }
        public int CurrentIndex { get; private set; }

        public Bracket(int parentIndex, int currentIndex)
        {
            ParentIndex = parentIndex;
            CurrentIndex = currentIndex;
            _operations = new List<EOperations>();
            _parameters = new List<FormulaParameter>();
        }

        public void AddOperation(EOperations operation)
        {
            _operations.Add(operation);
        }

        public void AddTemplateValue(List<char> parameter)
        {
            if (parameter.Count == 0)
            {
                return;
            }
            var array = parameter.ToArray();
            _parameters.Add(new FormulaParameter(new string(array)));
        }

        public void AddBracketValue(int index)
        {
            _parameters.Add(new FormulaParameter(index));
        }

        public float GetValue(Dictionary<string, ParameterTemplate<float>> attacker, Dictionary<string, ParameterTemplate<float>> target, Dictionary<string, float> resultAttacker, Dictionary<string, float> resultTarget, List<Bracket> brackets)
        {
            var value = 0f;

            var partsAfterMultiply = new List<float>();

            var newElement = _parameters[0].GetValue(attacker, target, resultAttacker, resultTarget, brackets);

            for (int i = 0; i < _operations.Count; i++)
            {
                if ((int)_operations[i] > 1)
                {
                    switch (_operations[i])
                    {
                        case EOperations.Multiply: newElement *= _parameters[i + 1].GetValue(attacker, target, resultAttacker, resultTarget, brackets); break;
                        case EOperations.Divide: newElement /= _parameters[i + 1].GetValue(attacker, target, resultAttacker, resultTarget, brackets); break;
                    }

                    if (_operations.Count - 1 == i)
                    {
                        partsAfterMultiply.Add(newElement);
                    }
                }
                else
                {
                    partsAfterMultiply.Add(newElement);
                    if (_operations.Count - 1 == i)
                    {
                        partsAfterMultiply.Add(_parameters[i + 1].GetValue(attacker, target, resultAttacker, resultTarget, brackets));
                    }
                    else
                    {
                        newElement = _parameters[i + 1].GetValue(attacker, target, resultAttacker, resultTarget, brackets);
                    }
                }
            }

            if (_parameters.Count == 1)
            {
                value = _parameters[0].GetValue(attacker, target, resultAttacker, resultTarget, brackets);
            }
            else
            {
                value = partsAfterMultiply[0];
            }

            var operationIndex = 0;
            for (int i = 1; i < partsAfterMultiply.Count; i++)
            {
                var nextOperation = 0;
                for (int j = operationIndex; j < _operations.Count; j++)
                {
                    if ((int)_operations[j] != 2)
                    {
                        nextOperation = (int)_operations[j];
                        operationIndex = j + 1;
                        break;
                    }
                }
                switch (nextOperation)
                {
                    case 0: value += partsAfterMultiply[i]; break;
                    case 1: value -= partsAfterMultiply[i]; break;
                }
            }

            return value;
        }
    }

    [Serializable]
    private class FormulaParameter
    {
        private EParameterType _type;
        private string _kind;
        private int _index;

        public FormulaParameter(string kind)
        {
            switch (kind[0])
            {
                case 'A':
                    _type = EParameterType.Attacker;
                    _kind = kind.Remove(0, 1);
                    break;
                case 'T':
                    _type = EParameterType.Target;
                    _kind = kind.Remove(0, 1);
                    break;
                case 'R':
                    switch (kind[1])
                    {
                        case 'A':
                            _type = EParameterType.ResultAttacker; break;
                        case 'T':
                            _type = EParameterType.ResultTarget; break;
                    }
                    _kind = kind.Remove(0, 2);
                    break;
                default:
                    _type = EParameterType.Value;
                    _kind = kind;
                    break;
            }
        }

        public FormulaParameter(int index)
        {
            _type = EParameterType.Bracket;
            _index = index;
        }

        public float GetValue(Dictionary<string, ParameterTemplate<float>> attacker, Dictionary<string, ParameterTemplate<float>> target, Dictionary<string, float> resultAttacker, Dictionary<string, float> resultTarget, List<Bracket> brackets)
        {
            switch (_type)
            {
                case EParameterType.Bracket: return brackets[_index].GetValue(attacker, target, resultAttacker, resultTarget, brackets);
                case EParameterType.Attacker: return attacker[_kind].Property.Value;
                case EParameterType.Target: return target[_kind].Property.Value;
                case EParameterType.ResultAttacker: return resultAttacker[_kind];
                case EParameterType.ResultTarget: return resultTarget[_kind];
                case EParameterType.Value: return float.Parse(_kind, CultureInfo.InvariantCulture.NumberFormat);
            }
            return 1f;
        }
    }

    private enum EParameterType
    {
        Bracket,
        Attacker,
        Target,
        ResultAttacker,
        ResultTarget,
        Value
    }

    private enum EOperations
    {
        Plus = 0,
        Minus = 1,
        Multiply = 2,
        Divide = 3
    }
}