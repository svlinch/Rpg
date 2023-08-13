using System.Collections.Generic;
using UnityEngine;

public class BattleManager
{
    private BalanceService _balanceService;

    public BattleManager(BalanceService balanceService)
    {
        _balanceService = balanceService;
    }

    public BattleResult ProcessBattleResult(SkillModel skill, UnitModel attacker, UnitModel target)
    {
        var result = new BattleResult();
        var changes = skill.GetChanges();
        foreach(var change in changes)
        {
            var formulaResult = Mathf.RoundToInt(change.UnpackedFormula.GetResult(attacker.GetChangableParameters(), target.GetChangableParameters(), 
                                                                 result.SelfChanges, result.TargetChanges));
            if (change.TargetSelf)
            {
                if (!result.SelfChanges.ContainsKey(change.TargetParameter))
                {
                    result.SelfChanges.Add(change.TargetParameter, 0);
                }
                result.SelfChanges[change.TargetParameter] += formulaResult;
                if (!string.IsNullOrEmpty(change.BuffId))
                {
                    result.SelfBuffs.Add(change.BuffId);
                }
            }
            else
            {
                if (!result.TargetChanges.ContainsKey(change.TargetParameter))
                {
                    result.TargetChanges.Add(change.TargetParameter, 0);
                }
                result.TargetChanges[change.TargetParameter] += formulaResult;
                if (!string.IsNullOrEmpty(change.BuffId))
                {
                    result.TargetBuffs.Add(change.BuffId);
                }
            }
        }

        var specialChanges = skill.GetSpecialChanges();
        foreach(var change in specialChanges)
        {
            UnpackSpecialAction(result, change, attacker, target);
        }

        return result;
    }

    private void UnpackSpecialAction(BattleResult result, SpecialChange change, UnitModel attacker, UnitModel target)
    {
        switch (change.Id)
        {
            case "randomBuff": RandomBuff(result, attacker); break;
        }
    }

    private List<string> _availableBuffs = new List<string>();
    private void RandomBuff(BattleResult result, UnitModel attacker)
    {
        var allBuffs = _balanceService.GetAllBuffs();
        var applyedBuffs = attacker.GetBuffs();
        if (applyedBuffs.Count > 1)
        {
            return;
        }
        _availableBuffs.Clear();
        var check = true;
        foreach (var buff in allBuffs)
        {
            check = true;
            foreach (var applyedBuff in applyedBuffs)
            {
                if (buff.Key.Equals(applyedBuff))
                {
                    check = false;
                    break;
                }
            }
            if (check)
            {
                _availableBuffs.Add(buff.Key);
            }
        }
        var randomIndex = UnityEngine.Random.Range(0, _availableBuffs.Count);
        result.SelfBuffs.Add(_availableBuffs[randomIndex]);
    }
}

public class BattleResult
{
    public Dictionary<string, float> SelfChanges = new Dictionary<string, float>();
    public Dictionary<string, float> TargetChanges = new Dictionary<string, float>();

    public List<string> SelfBuffs = new List<string>();
    public List<string> TargetBuffs = new List<string>();
}
