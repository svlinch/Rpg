using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Assets.Scripts.Utility;

public class GameData
{
    private ParametersTemplates _parameters;
    private UnitsTemplates _units;
    private SkillsTemplates _skills;
    private SetupTemplate _setup;
    private ParametersVisualSettings _visualSettings;
    private BuffsTemplates _buffs;

    private Dictionary<string, UnpackedUnitTemplate> _unpackedUnits;
    private Dictionary<string, UnpackedSkill> _unpackedSkills;

    public IEnumerator Initialize()
    {
        _parameters = JsonConvert.DeserializeObject<ParametersTemplates>(Resources.Load<TextAsset>("Parameters").ToString());
        yield return null;

        _units = JsonConvert.DeserializeObject<UnitsTemplates>(Resources.Load<TextAsset>("Units").ToString());
        yield return null;

        _skills = JsonConvert.DeserializeObject<SkillsTemplates>(Resources.Load<TextAsset>("Skills").ToString());
        yield return null;

        _setup = JsonConvert.DeserializeObject<SetupTemplate>(Resources.Load<TextAsset>("GameSetup").ToString());
        yield return null;

        _visualSettings = JsonConvert.DeserializeObject<ParametersVisualSettings>(Resources.Load<TextAsset>("VisualSettings").ToString());
        yield return null;

        _buffs = JsonConvert.DeserializeObject<BuffsTemplates>(Resources.Load<TextAsset>("Buffs").ToString());
        yield return null;

        _unpackedUnits = new Dictionary<string, UnpackedUnitTemplate>();
        InitializeUnits();
        yield return null;

        _unpackedSkills = new Dictionary<string, UnpackedSkill>();
        IintializeSkills();
    }
   
    private void InitializeUnits()
    {
        foreach (var unit in _units.Units)
        {
            var newUnit = new UnpackedUnitTemplate(unit.Value, _parameters);
            _unpackedUnits.Add(newUnit.Id, newUnit);
        }
    }

    private void IintializeSkills()
    {
        foreach(var skill in _skills.Skills)
        {
            var newSKill = new UnpackedSkill(skill.Value);
            _unpackedSkills.Add(newSKill.Id, newSKill);
        }
    }

    public UnpackedUnitTemplate GetUnit(string id)
    {
        return CloneUtil.Clone(_unpackedUnits[id]);
    }
    
    public UnpackedSkill GetSkill(string id)
    {
        return CloneUtil.Clone(_unpackedSkills[id]);
    }

    public SetupTemplate GetSetup()
    {
        return _setup;
    }

    public ParameterVisualSettings GetVisualSettings(string id)
    {
        ParameterVisualSettings result;
        if (_visualSettings.Settings.TryGetValue(id, out result))
        {
            return result;
        }
        return null;
    }

    public BuffTemplate GetBuff(string id)
    {
        return _buffs.Buffs[id];
    }
    
    public Dictionary<string, BuffTemplate> GetBuffs()
    {
        return _buffs.Buffs;
    }
}
