using System.Collections.Generic;
using Newtonsoft.Json;
using System;

[Serializable]
public class UnitsTemplates
{
    [JsonProperty]
    public readonly Dictionary<string, UnitTemplate> Units;
}

[Serializable]
public class UnitTemplate
{
    [JsonProperty]
    public readonly string Id;

    [JsonProperty]
    public readonly Dictionary<string, float> FParameters;

    [JsonProperty]
    public readonly List<string> Skills;

    [JsonProperty]
    public readonly int ActionPoints;
}

[Serializable]
public class UnpackedUnitTemplate
{
    public string Id { get; private set; }
    public readonly Dictionary<string, ParameterTemplate<float>> FParameters;
    public readonly List<string> Skills;
    public int ActionPoints { get; private set; }

    public UnpackedUnitTemplate(UnitTemplate template, ParametersTemplates parameters)
    {
        Id = string.Copy(template.Id);

        FParameters = new Dictionary<string, ParameterTemplate<float>>();
        foreach(var pair in template.FParameters)
        {
            var newParameter = parameters.GetFParameter(pair.Key);
            newParameter.Property.Value = pair.Value;
            FParameters.Add(pair.Key, newParameter);
        }

        Skills = new List<string>(template.Skills);
        ActionPoints = template.ActionPoints;
    }

    public void ChangeActionPoints(int change)
    {
        ActionPoints += change;
    }
}