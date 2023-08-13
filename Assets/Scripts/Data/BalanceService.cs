using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceService: IStarter
{
    private GameData _data;

    public IEnumerator Initialize(MonoBehaviour mb)
    {
        _data = new GameData();
        yield return mb.StartCoroutine(_data.Initialize());
    }

    public SetupTemplate GetSetup()
    {
        return _data.GetSetup();
    }

    public UnpackedUnitTemplate GetUnit(string id)
    {
        var result = _data.GetUnit(id);
        return result;
    }

    public UnpackedSkill GetSkill(string id)
    {
        return _data.GetSkill(id);
    }

    public ParameterVisualSettings GetVisual(string id)
    {
        return _data.GetVisualSettings(id);
    }

    public BuffTemplate GetBuff(string id)
    {
        return _data.GetBuff(id);
    }

    public Dictionary<string, BuffTemplate> GetAllBuffs()
    {
        return _data.GetBuffs();
    }
}
