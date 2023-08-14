using System;
using System.Collections;
using UnityEngine;

public class DamageVisualApply
{
    public IEnumerator Apply(BattleResult result, UnitModel attacker, UnitModel target, Action callback)
    {
        if (result.TargetChanges.ContainsKey(StaticTranslator.HEALTH) && result.TargetChanges[StaticTranslator.HEALTH] < 0)
        {
            var progress = 1f;
            var speed = 2f;
            var sprite = target.UnitVisual.GetRenderer();
            while(progress > 0f)
            {
                progress -= Time.deltaTime * speed;
                sprite.color = new Color(1f, progress, progress);
                yield return null;
            }
            progress = 0f;
            while(progress < 1f)
            {
                progress += Time.deltaTime * speed;
                sprite.color = new Color(1f, progress, progress);
                yield return null;
            }
        }
        callback.Invoke();
        yield return null;
    }
}
