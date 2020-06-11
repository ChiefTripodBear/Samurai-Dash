using System;
using UnityEngine;

public class RedirectDisplayObject : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private IUnit _intersectingUnit;
    private Vector2 _intersectionDirection;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ToggleActive(bool isActive)
    {
        var color = _spriteRenderer.color;
        var alpha = isActive ? 1f : 0.1f;
        
        _spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
    }

    private void Update()
    {
        if (_intersectingUnit != null)
        {
            transform.position = _intersectingUnit.AngleDefinition.IntersectionPoint;
            var angle = Mathf.Atan2(_intersectionDirection.y, _intersectionDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 180);
        }
    }

    public void SetDisplayParameters(Vector2 direction, IUnit unit)
    {
        _intersectingUnit = unit;
        _intersectionDirection = direction;
    }
}