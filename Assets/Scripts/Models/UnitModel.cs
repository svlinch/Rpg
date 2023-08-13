using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utility;

public class UnitModel
{
    private readonly BalanceService _balanceService;
    private readonly EventService _eventService;

    private readonly UnpackedUnitTemplate _template;
    private readonly UnpackedUnitTemplate _changableTemplate;

    private readonly List<SkillModel> _skills;
    private readonly List<BuffModel> _buffs;
    private readonly bool _isLeft;

    public readonly CustomProperty<int> ActionPoints;
    public readonly UnitVisual UnitVisual;

    public UnitModel(EventService eventService, BalanceService balanceService, UnpackedUnitTemplate template, UnitVisual unitVisual, bool isLeft)
    {
        _template = template;
        _eventService = eventService;
        _balanceService = balanceService;
        UnitVisual = unitVisual;
        _isLeft = isLeft;

        _changableTemplate = CloneUtil.Clone(_template);

        _skills = new List<SkillModel>();
        InitializeSkills();

        _buffs = new List<BuffModel>();
        ActionPoints = new CustomProperty<int>();
        ActionPoints.Value = _template.ActionPoints;
    }

    public Dictionary<string, ParameterTemplate<float>> GetChangableParameters()
    {
        return _changableTemplate.FParameters;
    }

    public List<SkillModel> GetSkillsList()
    {
        return _skills;
    }

    public void ChangeActionPoints(int change)
    {
        _changableTemplate.ChangeActionPoints(change);
    }

    public int GetActionPoints()
    {
        return _changableTemplate.ActionPoints;
    }

    public void ApplyDamageResult(Dictionary<string, float> changes, List<string> buffs)
    {
        foreach(var change in changes)
        {
            var result = _changableTemplate.FParameters[change.Key].Property.Value + change.Value;
            result = Mathf.Max(_changableTemplate.FParameters[change.Key].MinValue, result);
            result = Mathf.Min(_changableTemplate.FParameters[change.Key].MaxValue, result);
            _changableTemplate.FParameters[change.Key].Property.Value = result;
        }

        foreach (var buff in buffs)
        {
            var newBuff = new BuffModel(_balanceService.GetBuff(buff), _changableTemplate.FParameters);
            _buffs.Add(newBuff);
            _eventService.SendMessage(new BuffListChanged(_isLeft, false, newBuff));
        }
    }

    public void TurnEnd()
    {
        CheckoutBuffs();
    }

    public void TurnStart()
    {
        ActionPoints.Value = _template.ActionPoints;
        foreach (var skill in _skills)
        {
            skill.Reset();
        }
    }

    public List<string> GetBuffs()
    {
        var result = new List<string>();
        foreach(var buff in _buffs)
        {
            result.Add(buff.GetId());
        }
        return result;
    }

    public bool CheckDeath()
    {
        return _changableTemplate.FParameters[StaticTranslator.HEALTH].Property.Value <= 0f;
    }

    public void FullReset()
    {
        foreach(var parameter in _changableTemplate.FParameters)
        {
            parameter.Value.Property.Value = _template.FParameters[parameter.Key].Property.Value;
        }
        foreach(var skill in _skills)
        {
            skill.Reset();
        }
        foreach(var buff in _buffs)
        {
            _eventService.SendMessage(new BuffListChanged(_isLeft, true, buff));
        }
        _buffs.Clear();
        ActionPoints.Value = _template.ActionPoints;
    }

    private void CheckoutBuffs()
    {
        for (int i = 0; i < _buffs.Count; i++)
        {
            if (!_buffs[i].UpdateBuff(_changableTemplate.FParameters))
            {
                _eventService.SendMessage(new BuffListChanged(_isLeft, true, _buffs[i]));
                _buffs.RemoveAt(i);
                i--;
            }
        }
    }

    private void InitializeSkills()
    {
        foreach(var skill in _changableTemplate.Skills)
        {
            var unpackedSkill = _balanceService.GetSkill(skill);
            var model = new SkillModel(unpackedSkill);
            _skills.Add(model);
        }
    }
}
