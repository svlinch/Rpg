using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class ParametersTemplates
{
    [JsonProperty]
    private readonly Dictionary<string, ParameterTemplate<float>> _fParameters;

    public ParameterTemplate<float> GetFParameter(string id)
    {
        return _fParameters[id];
    }
}

[Serializable]
public class ParameterTemplate<T>
{
    [JsonProperty]
    private T _minValue;
    public T MinValue { get { return _minValue; } }

    [JsonProperty]
    private T _maxValue;
    public T MaxValue { get { return _maxValue; } }

    [JsonProperty]
    private CustomProperty<T> _property;
    public CustomProperty<T> Property { get { return _property; } }
}