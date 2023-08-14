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

    private TurnLogic[] _turns;
    private TurnLogic _currentTurn;

    public CustomProperty<uint> Rounds;

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

        Rounds = new CustomProperty<uint>();
        _eventService.SendMessage(new GamePlayInitialized(Rounds));
        yield return null;

        SetupTurns();
        ResetGame();
    }

    public void ResetGame()
    {
        _leftSideUnit.FullReset();
        _rightSideUnit.FullReset();

        _currentTurn = _turns[0];
        Rounds.Value = 0;

        _eventService.SendMessage(new BattleProcessEvent(false));
        _eventService.SendMessage(new ChangeGameState(true));
    }

    public void HandleSkillApplyed()
    {
        _eventService.SendMessage(new BattleProcessEvent(false));
    }

    public void ChangeTurnOrder(int nextTurnIndex)
    {
        _currentTurn.EndTurn();
        _currentTurn = _turns[nextTurnIndex];
        _currentTurn.StartTurn();
        _eventService.SendMessage(new ChangeGameState(nextTurnIndex == 0));
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
        _eventService.SendMessage(new BattleProcessEvent(true));
        _currentTurn.HandleSkillClicked(_mb, _battleManager, _damageApply, e);
        return true;
    }

    private void SetupTurns()
    {
        _turns = new TurnLogic[2];
        _turns[0] = new TurnLogic(_leftSideUnit, _rightSideUnit, 1, this);
        _turns[1] = new TurnLogic(_rightSideUnit, _leftSideUnit, 0, this);
    }

    private class TurnLogic
    {
        private readonly UnitModel _mainUnit;
        private readonly UnitModel _targetUnit;
        private readonly int _nextTurnIndex;
        private readonly GamePlayController _gamePlayController;

        public TurnLogic(UnitModel mainUnit, UnitModel targetUnit, int nextTurnIndex, GamePlayController gamePlayController)
        {
            _mainUnit = mainUnit;
            _targetUnit = targetUnit;
            _nextTurnIndex = nextTurnIndex;
            _gamePlayController = gamePlayController;
        }

        public void EndTurn()
        {
            _mainUnit.TurnEnd();
        }

        public void StartTurn()
        {
            if (_mainUnit.IsLeft)
            {
                _gamePlayController.Rounds.Value++;
            }
            _mainUnit.TurnStart();
        }

        public BattleResult HandleSkillClicked(MonoBehaviour mb, BattleManager battleManager, DamageVisualApply damageApply, SkillClickedEvent e)
        {
            e.Skill.UsesRemains.Value--;
            _mainUnit.ActionPoints.Value -= e.Skill.GetCost();

            var result = battleManager.ProcessBattleResult(e.Skill, _mainUnit, _targetUnit);
            mb.StartCoroutine(damageApply.Apply(result, _mainUnit, _targetUnit, HandleSkillApplyed));
            return result;
        }

        private void HandleSkillApplyed()
        {
            if (_mainUnit.CheckDeath() || _targetUnit.CheckDeath())
            {
                _gamePlayController.ResetGame();
                return;
            }
            _gamePlayController.HandleSkillApplyed();
            if (_mainUnit.ActionPoints.Value == 0)
            {
                _gamePlayController.ChangeTurnOrder(_nextTurnIndex);
            }
        }
    }
}
