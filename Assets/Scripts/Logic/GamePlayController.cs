using System.Collections;
using UnityEngine;
using Assets.Scripts.Factories;

public class GamePlayController: IStarter
{
    #region Injection
    private EventService _eventService;
    private SubscriptionHolder _subscriptions;
    private IObjectFactory _factory;
    private BalanceService _balanceService;

    public void Inject(EventService eventService, BalanceService balanceService, MainFactory factory)
    {
        _factory = factory;
        _eventService = eventService;
        _balanceService = balanceService;

        _subscriptions = new SubscriptionHolder(_eventService);
        _subscriptions.Subscribe<SkillClickedEvent>(HandleSkillClicked);
        _subscriptions.Subscribe<GameResetEvent>(HandleGameReset);
    }
    #endregion

    private UnitModel _leftSideUnit;
    private UnitModel _rightSideUnit;

    private BattleManager _battleManager;
    private DamageVisualApply _damageApply;
    private MonoBehaviour _mb;

    private EState _currentState;
    private CustomProperty<uint> _rounds;

    public IEnumerator Initialize(MonoBehaviour mb)
    {
        _mb = mb;
        _battleManager = new BattleManager(_balanceService);
        _damageApply = new DamageVisualApply();
        var setup = _balanceService.GetSetup();

        var newUnit = CreateUnit(setup.LeftSide);
        _leftSideUnit = new UnitModel(_eventService, _balanceService, _balanceService.GetUnit(setup.LeftSide.UnitId), newUnit, true);
        _eventService.SendMessage(new UnitModelCreated(_leftSideUnit, true));

        newUnit = CreateUnit(setup.RightSide);
        _rightSideUnit = new UnitModel(_eventService, _balanceService, _balanceService.GetUnit(setup.RightSide.UnitId), newUnit, false);
        _eventService.SendMessage(new UnitModelCreated(_rightSideUnit, false));

        _rounds = new CustomProperty<uint>();
        _eventService.SendMessage(new GamePlayInitialized(_rounds));
        yield return null;

        ResetGame();
    }

    private void ResetGame()
    {
        _leftSideUnit.FullReset();
        _rightSideUnit.FullReset();

        _currentState = EState.Left;
        _rounds.Value = 0;
        _eventService.SendMessage(new ChangeGameState(_currentState));
    }

    private bool HandleGameReset(GameResetEvent e)
    {
        ResetGame();
        return true;
    }

    private UnitVisual CreateUnit(SideSettings side)
    {
        var description = FactoryDescriptionBuilder.Object()
                        .Kind(side.UnitId)
                        .Position(new Vector2(side.PositionX, side.PositionY))
                        .Parent(null)
                        .Type(EObjectType.Unit)
                        .Build();
        return _factory.Create(description).GetComponent<UnitVisual>();
    }

    private bool HandleSkillClicked(SkillClickedEvent e)
    {
        e.Skill.UsesRemains.Value--;
        BattleResult result;
        _eventService.SendMessage(new BattleProcessEvent(true));
        if (_currentState == EState.Left)
        {
            result = _battleManager.ProcessBattleResult(e.Skill, _leftSideUnit, _rightSideUnit);
            _mb.StartCoroutine(_damageApply.Apply(result, _leftSideUnit, _rightSideUnit, SkillApplyed));
            _leftSideUnit.ActionPoints.Value -= e.Skill.GetCost();
        }
        else
        {
            result = _battleManager.ProcessBattleResult(e.Skill, _rightSideUnit, _leftSideUnit);
            _mb.StartCoroutine(_damageApply.Apply(result, _rightSideUnit, _leftSideUnit, SkillApplyed));
            _rightSideUnit.ActionPoints.Value -= e.Skill.GetCost();
        }
        return true;
    }

    private void SkillApplyed()
    {
        if (_leftSideUnit.CheckDeath() || _rightSideUnit.CheckDeath())
        {
            ResetGame();
            _eventService.SendMessage(new BattleProcessEvent(false));
            return;
        }

        if (_currentState == EState.Left)
        {
            if (_leftSideUnit.ActionPoints.Value == 0)
            {
                ChangeTurnOrder();
            }
        }
        else
        {
            if (_rightSideUnit.ActionPoints.Value == 0)
            {
                ChangeTurnOrder();
            }
        }
        _eventService.SendMessage(new BattleProcessEvent(false));
    }

    private void ChangeTurnOrder()
    {
        if (_currentState == EState.Left)
        {
            _leftSideUnit.TurnEnd();
            _currentState = EState.Right;
            _rightSideUnit.TurnStart();
            _eventService.SendMessage(new ChangeGameState(_currentState));
        }
        else
        {
            _rounds.Value++;
            _rightSideUnit.TurnEnd();
            _currentState = EState.Left;
            _leftSideUnit.TurnStart();
            _eventService.SendMessage(new ChangeGameState(_currentState));
        }
    }
}
