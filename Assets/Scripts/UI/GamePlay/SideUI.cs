using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Factories;

public class SideUI : MonoBehaviour
{
    private PoolFactory _pool;
    private IObjectFactory _factory;
    private BalanceService _balanceService;
    private EventService _eventService;
    private SubscriptionHolder _subscriptions;

    [SerializeField]
    private RectTransform _transform;
    [SerializeField]
    private Transform _barsHolder;
    [SerializeField]
    private Transform _skillsHolder;
    [SerializeField]
    private Transform _buffsHolder;
    [SerializeField]
    private HorizontalLayoutGroup _skillsGroup;

    private bool _isLeftSide;
    private Dictionary<BuffModel, BuffUI> _buffs = new Dictionary<BuffModel, BuffUI>();

    public void Initialize(UnitModelCreated e, IObjectFactory factory, BalanceService balanceService, EventService eventService)
    {
        _factory = factory;
        _pool = new PoolFactory(_factory, "buffs");

        _balanceService = balanceService;
        _eventService = eventService;

        _isLeftSide = e.LeftSide;

        _subscriptions = new SubscriptionHolder(_eventService);
        _subscriptions.Subscribe<BuffListChanged>(HandleBuffListChanged);

        SetupPosition(e.LeftSide);
        SetupBars(e);
        SetupSkills(e);
    }

    private void SetupPosition(bool left)
    {
        _transform.anchorMin = left ? Vector2.zero : Vector2.right;
        _transform.anchorMax = left ? Vector2.zero : Vector2.right;
        _transform.anchoredPosition = left ? new Vector2(_transform.rect.size.x / 2f, _transform.rect.size.y / 2f)
                                        : new Vector2(-_transform.rect.size.x / 2f, _transform.rect.size.y / 2f);
    }

    private void SetupBars(UnitModelCreated e)
    {
        var description = FactoryDescriptionBuilder.Object()
                        .Parent(_barsHolder)
                        .Kind("Bar")
                        .Type(EObjectType.UI)
                        .Build();
        foreach(var parameter in e.Model.GetChangableParameters())
        {
            var settings = _balanceService.GetVisual(parameter.Key);
            if (settings == null)
            {
                continue;
            }
            var newBar = _factory.Create(description).GetComponent<BarUI>();
            newBar.Initialize(parameter.Value, settings, e.LeftSide);
        }
    }

    private void SetupSkills(UnitModelCreated e)
    {
        var description = FactoryDescriptionBuilder.Object()
                        .Parent(_skillsHolder)
                        .Kind("Skill")
                        .Type(EObjectType.UI)
                        .Build();
        foreach (var skill in e.Model.GetSkillsList())
        {
            var newSKill = _factory.Create(description).GetComponent<SkillUI>();
            newSKill.Initialize(skill, _eventService, e.LeftSide);
        }
        _skillsGroup.childAlignment = e.LeftSide ? TextAnchor.MiddleLeft : TextAnchor.MiddleRight;
    }

    private bool HandleBuffListChanged(BuffListChanged e)
    {
        if (_isLeftSide != e.IsLeftSide)
        {
            return true;
        }

        if (e.IsRemoved)
        {
            _buffs[e.Model].Remove();
            _pool.ReturnToPool(_buffs[e.Model].gameObject, "Buff");
            _buffs.Remove(e.Model);
        }
        else
        {
            var description = FactoryDescriptionBuilder.Object()
                            .Parent(_buffsHolder)
                            .Kind("Buff")
                            .Type(EObjectType.UI)
                            .Build();
            var newBuffUI = _pool.Create(description).GetComponent<BuffUI>();
            newBuffUI.Initialize(e.Model);
            _buffs.Add(e.Model, newBuffUI);
        }
        return false;
    }
}
