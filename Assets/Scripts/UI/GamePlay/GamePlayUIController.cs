using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Factories;

public class GamePlayUIController: MonoBehaviour
{
    #region Injection
    private EventService _eventService;
    private SubscriptionHolder _subscriptions;
    private BalanceService _balanceService;
    private IObjectFactory _factory;

    public void Inject(EventService eventService, MainFactory factory, BalanceService balanceService)
    {
        _eventService = eventService;
        _factory = factory;
        _balanceService = balanceService;

        _subscriptions = new SubscriptionHolder(_eventService);
        _subscriptions.Subscribe<UnitModelCreated>(HandleUnitModelCreated);
        _subscriptions.Subscribe<BattleProcessEvent>(HandleBattleProcess);
    }
    #endregion

    [SerializeField]
    private GraphicRaycaster _raycaster;

    private bool HandleUnitModelCreated(UnitModelCreated e)
    {
        var description = FactoryDescriptionBuilder.Object()
                        .Type(EObjectType.UI)
                        .Parent(transform)
                        .Kind("Side")
                        .Build();
        var side = _factory.Create(description);
        side.GetComponent<SideUI>().Initialize(e, _factory, _balanceService, _eventService);
        return true;
    }

    private bool HandleBattleProcess(BattleProcessEvent e)
    {
        _raycaster.enabled = !e.State;
        return true;
    }
}
