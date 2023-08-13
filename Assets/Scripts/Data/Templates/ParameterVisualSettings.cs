using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[Serializable]
public class ParametersVisualSettings
{
    [JsonProperty]
    public readonly Dictionary<string, ParameterVisualSettings> Settings;
}

[Serializable]
public class ParameterVisualSettings
{
    [JsonProperty]
    private readonly float[] Color;

    public Color GetColor()
    {
        return new Color(Color[0], Color[1], Color[2]);
    }
}

