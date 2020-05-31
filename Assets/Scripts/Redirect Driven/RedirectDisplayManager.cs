using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RedirectDisplayManager : MonoBehaviour
{
    [SerializeField] private RedirectDisplayObject _redirectObjectPrefab;

    private static RedirectDisplayManager _instance;
    public static RedirectDisplayManager Instance => _instance;

    private List<RedirectDisplayObject> _currentDisplays = new List<RedirectDisplayObject>();
    
    private void Awake()
    {
        _instance = this;
    }

    public void DisplayCorrectVector(Vector2 direction, Vector2 intersection)
    {
        var redirectObject = Instantiate(_redirectObjectPrefab, intersection, Quaternion.identity);

        redirectObject.SetDisplayParameters(direction, intersection);

        Debug.Log("Creating displays");
        _currentDisplays.Add(redirectObject);
    }

    public void SetActiveDisplayWithIntersectionAt(Vector2 intersection)
    {
        RedirectDisplayObject displayToActivate = null;
        foreach (var display in _currentDisplays)
        {
            if (display != null && (Vector2) display.transform.position == intersection)
            {
                displayToActivate = display;
                break;
            }
        }

        if (displayToActivate == null) return;


        displayToActivate.ToggleActive(true);

        foreach (var display in _currentDisplays)
            if (display != displayToActivate)
                display.ToggleActive(false);
    }

    public void ResetDisplay()
    {
        foreach (var display in _currentDisplays)
        {
            if (display != null)
            {
                Destroy(display.gameObject);
            }
        }
        _currentDisplays.Clear();
    }
}