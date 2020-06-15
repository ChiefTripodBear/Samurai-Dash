using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DashChargeUI : MonoBehaviour
{
    private List<Image> _displayImages = new List<Image>();

    private void Awake()
    {
        _displayImages = GetComponentsInChildren<Image>().ToList();
        
        PlayerDashMonitor.OnDashChargesChanged += UpdateDisplay;
    }

    private void OnDestroy()
    {
        PlayerDashMonitor.OnDashChargesChanged -= UpdateDisplay;
    }

    private void UpdateDisplay(int currentCharges)
    {
        var counter = 0;
        foreach (var image in _displayImages)
        {
            counter++;
            image.enabled = counter <= currentCharges;
        }
    }
}
