using System;
using UnityEngine;
using TMPro;

public class BuffUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _titleText;
    [SerializeField]
    private TextMeshProUGUI _durationText;

    private Action<uint> _subscription;
    private Action _unsubscribeAction;

    public void Initialize(BuffModel model)
    {
        _subscription = x => _durationText.text = x.ToString();
        model.Duration.Subscribe(_subscription);
        _unsubscribeAction = () => model.Duration.Unsubscribe(_subscription);

        _titleText.text = model.GetId();
        _durationText.text = model.Duration.Value.ToString();
    }

    public void Remove()
    {
        _unsubscribeAction.Invoke();
    }
}
