using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    private List<IStarter> _starters;

    public void Inject(BalanceService balanceService, GamePlayController gamePlayController)
    {
        _starters = new List<IStarter>();
        _starters.Add(balanceService);
        _starters.Add(gamePlayController);
    }

    private IEnumerator Start()
    {
        foreach(var starter in _starters)
        {
            yield return StartCoroutine(starter.Initialize(this));
            Debug.Log(string.Format("{0} initialized", starter.GetType().ToString()));
        }
    }
}
