using System;
using Newtonsoft.Json;

[Serializable]
public class SetupTemplate
{
    [JsonProperty]
    public readonly SideSettings LeftSide;
    [JsonProperty]
    public readonly SideSettings RightSide;
}

[Serializable]
public class SideSettings
{
    [JsonProperty]
    public readonly string UnitId;
    [JsonProperty]
    public readonly float PositionX;
    [JsonProperty]
    public readonly float PositionY;
}
