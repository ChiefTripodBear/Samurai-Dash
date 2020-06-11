using UnityEngine;
using UnityEngine.InputSystem;

public class InputProcessor
{
    private Camera MainCam => Camera.main;

    public bool MouseDown => Mouse.current.leftButton.wasPressedThisFrame;
    public bool MouseUp => Mouse.current.leftButton.wasReleasedThisFrame;

    public Vector2 MousePosition()
    {
        return MainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }

    public Vector2 GetMoveDirection(Vector2 from, Vector2 to)
    {
        return (to - from).normalized;
    }
}