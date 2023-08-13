using System.Collections.Generic;

public class SkillModel
{
    private UnpackedSkill _template;
    public CustomProperty<uint> UsesRemains;

    public SkillModel(UnpackedSkill template)
    {
        _template = template;
        UsesRemains = new CustomProperty<uint>();
        UsesRemains.Value = _template.UsesPerTurn;
    }

    public List<UnpcackedChange> GetChanges()
    {
        return _template.GetChanges();
    }

    public List<SpecialChange> GetSpecialChanges()
    {
        return _template.GetSpecialChanges();
    }

    public int GetCost()
    {
        return _template.Cost;
    }

    public void Reset()
    {
        UsesRemains.Value = _template.UsesPerTurn;
    }

    public string GetId()
    {
        return _template.Id;
    }
}
