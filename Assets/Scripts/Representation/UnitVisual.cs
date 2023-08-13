using UnityEngine;

public class UnitVisual : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _sprite;

    public SpriteRenderer GetRenderer()
    {
        return _sprite;
    }
}
