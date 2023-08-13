using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class SkillsTemplates
{
    [JsonProperty]
    public readonly Dictionary<string, SkillTemplate> Skills;
}

[Serializable]
public class SkillTemplate
{
    [JsonProperty]
    public readonly string Id;

    [JsonProperty]
    public readonly List<SkillChange> Changes;

    [JsonProperty]
    public readonly List<SpecialChange> SpecialChanges;

    [JsonProperty]
    public readonly uint UsesPerTurn;

    [JsonProperty]
    public readonly int Cost;
}

[Serializable]
public class SkillChange
{
    [JsonProperty]
    public readonly string Formula;
    [JsonProperty]
    public readonly string TargetParameter;
    [JsonProperty]
    public readonly bool TargetSelf;
    [JsonProperty]
    public readonly string BuffId;

    [NonSerialized]
    public readonly Formula UnpackedFormula;
}

[Serializable]
public class SpecialChange
{
    [JsonProperty]
    public readonly string Id;
    [JsonProperty]
    public readonly bool TargetSelf;
}

[Serializable]
public class UnpcackedChange
{
    public readonly string TargetParameter;
    public readonly bool TargetSelf;
    public readonly string BuffId;

    public readonly Formula UnpackedFormula;

    public UnpcackedChange(SkillChange change)
    {
        TargetParameter = change.TargetParameter;
        TargetSelf = change.TargetSelf;
        BuffId = string.IsNullOrEmpty(change.BuffId) ? string.Empty : change.BuffId;
        UnpackedFormula = new Formula(change.Formula);
    }
}

[Serializable]
public class UnpackedSkill
{
    private readonly List<UnpcackedChange> _changes;
    private readonly List<SpecialChange> _specialChanges;

    public readonly string Id;
    public readonly uint UsesPerTurn;
    public readonly int Cost;

    public UnpackedSkill(SkillTemplate template)
    {
        Id = string.Copy(template.Id);
        UsesPerTurn = template.UsesPerTurn;
        Cost = template.Cost;

        _changes = new List<UnpcackedChange>();
        if (template.Changes != null)
        {
            foreach (var change in template.Changes)
            {
                var newFormula = new UnpcackedChange(change);
                _changes.Add(newFormula);
            }
        }

        _specialChanges = new List<SpecialChange>(template.SpecialChanges);
    }

    public List<UnpcackedChange> GetChanges()
    {
        return _changes;
    }

    public List<SpecialChange> GetSpecialChanges()
    {
        return _specialChanges;
    }
}
