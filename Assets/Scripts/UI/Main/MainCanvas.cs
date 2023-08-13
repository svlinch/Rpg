using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainCanvas : MonoBehaviour
{
    #region Injection
    private EventService _eventService;
    private SubscriptionHolder _subscriptions;

    public void Inject(EventService eventService)
    {
        _eventService = eventService;

        _subscriptions = new SubscriptionHolder(_eventService);
        _subscriptions.Subscribe<BattleProcessEvent>(HandleBattleProcess);
        _subscriptions.Subscribe<GamePlayInitialized>(HandleGamePlayInitialized);
    }
    #endregion

    [SerializeField]
    private GraphicRaycaster _raycaster;
    [SerializeField]
    private Button _button;
    [SerializeField]
    private TextMeshProUGUI _text;

    private void Start()
    {
        _button.onClick.AddListener(HandleResetClick);
    }

    private void HandleResetClick()
    {
        _eventService.SendMessage(new GameResetEvent());
    }

    private bool HandleBattleProcess(BattleProcessEvent e)
    {
        _raycaster.enabled = !e.State;
        return true;
    }

    private bool HandleGamePlayInitialized(GamePlayInitialized e)
    {
        e.Rounds.Subscribe(x => _text.text = x.ToString());
        return true;
    }
}
