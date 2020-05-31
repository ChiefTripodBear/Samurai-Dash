using UnityEngine;

public class RedirectDisplayObject : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

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
    
    public void SetDisplayParameters(Vector2 direction, Vector2 intersection)
    {
        transform.position = intersection;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 180);
    }
}