using UnityEngine;
using UnityEngine.InputSystem;

public static class RedirectEvaluator
{
    private static Camera MainCam => Camera.main;
    
    public static bool ValidRedirect(Vector2 to, Vector2 destination, float dotThreshold)
    {
        var mousePosition = (Vector2)MainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        
        var requiredMouseDirection = (to - destination).normalized;
        var mouseDirection = (mousePosition - destination).normalized;

        var mouseDot = Vector2.Dot(requiredMouseDirection, mouseDirection);

        return mouseDot > dotThreshold;
    }
}