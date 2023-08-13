using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarUI : MonoBehaviour
{
    [SerializeField]
    private Slider _bar;
    [SerializeField]
    private Image _frontImage;
    [SerializeField]
    private TextMeshProUGUI _text;

    public void Initialize(ParameterTemplate<float> parameter, ParameterVisualSettings setting, bool leftSide)
    {
        _bar.direction = leftSide ? Slider.Direction.LeftToRight : Slider.Direction.RightToLeft;
        _frontImage.color = setting.GetColor();
        parameter.Property.Subscribe(x =>
        {
            _bar.value = Mathf.Max(0.05f, x / parameter.MaxValue);
            _text.text = string.Format("{0}/{1}", x, parameter.MaxValue);
        });
    }
}
