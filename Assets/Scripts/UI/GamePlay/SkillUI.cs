using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;
    [SerializeField]
    private Button _button;

    private EventService _eventService;
    private SubscriptionHolder _subscriptions;
    private SkillModel _skill;
    private bool _leftSide;

    public void Initialize(SkillModel skill, EventService eventService, bool leftSide)
    {
        _skill = skill;
        _eventService = eventService;
        _leftSide = leftSide;

        _subscriptions = new SubscriptionHolder(_eventService);
        _subscriptions.Subscribe<ChangeGameState>(HandleGameStateChange);

        _skill.UsesRemains.Subscribe(x => _button.interactable = _skill.UsesRemains.Value > 0);
        _button.onClick.AddListener(HandleClick);
        _text.text = skill.GetId();
    }

    private void HandleClick()
    {
        _eventService.SendMessage(new SkillClickedEvent(_skill));
    }

    private bool HandleGameStateChange(ChangeGameState e)
    {
        _button.interactable = (e.IsLeftTurn == _leftSide) ? _skill.UsesRemains.Value > 0 : false;
        return true;
    }
}
