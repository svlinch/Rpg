using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class BuffsTemplates
{
    [JsonProperty]
    public readonly Dictionary<string, BuffTemplate> Buffs;
}

[Serializable]
public class BuffTemplate 
{
    [JsonProperty]
    public string Id;
    [JsonProperty]
    public readonly List<BuffChange> OnStartChanges;
    [JsonProperty]
    public readonly List<BuffChange> OnEndChanges;
    [JsonProperty]
    public readonly uint Duration;
}

[Serializable]
public class BuffChange
{
    [JsonProperty]
    public readonly string TargetParameter;
    [JsonProperty]
    public readonly float ChangeAmount;
}