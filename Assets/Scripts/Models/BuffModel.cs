using System.Collections.Generic;
using UnityEngine;

public class BuffModel
{
    private BuffTemplate _template;
    public CustomProperty<uint> Duration { get; private set; }

    public BuffModel(BuffTemplate template, Dictionary<string, ParameterTemplate<float>> target)
    {
        _template = template;
        Duration = new CustomProperty<uint>();
        Duration.Value = template.Duration;
        ApplyBuff(_template.OnStartChanges, target);
    }

    public bool UpdateBuff(Dictionary<string, ParameterTemplate<float>> target)
    {
        Duration.Value--;
        if (Duration.Value == 0)
        {
            ApplyBuff(_template.OnEndChanges, target);
            return false;
        }
        return true;
    }

    public string GetId()
    {
        return _template.Id;
    }

    private void ApplyBuff(List<BuffChange> changes, Dictionary<string, ParameterTemplate<float>> target)
    {
        foreach(var change in changes)
        {
            var result = target[change.TargetParameter].Property.Value + change.ChangeAmount;
            result = Mathf.Max(target[change.TargetParameter].MinValue, result);
            result = Mathf.Min(target[change.TargetParameter].MaxValue, result);
            target[change.TargetParameter].Property.Value = result;
        }
    }
}
