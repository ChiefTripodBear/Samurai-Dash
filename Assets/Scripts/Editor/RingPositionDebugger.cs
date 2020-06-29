using System.Globalization;
using UnityEditor;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

[CustomEditor(typeof(RingPosition)), CanEditMultipleObjects]
public class RingPositionDebugger : Editor
{
    private void OnSceneGUI()
    {
        var style = new GUIStyle {normal = {textColor = Color.green}};

        var ringPosition = target as RingPosition;

        if (ringPosition == null) return;
     
        var ringNode = ringPosition.RingNode;

        if (ringNode == null) return;

        style.normal.textColor = Color.green;
        style.fontSize = 30;
        Handles.Label(ringPosition.FCostLocation.position, ringNode.FCost.ToString(CultureInfo.InvariantCulture), style);
        
        style.normal.textColor = Color.blue;
        style.fontSize = 20;
        Handles.Label(ringPosition.HCostLocation.position, ringNode.HCost.ToString(CultureInfo.InvariantCulture), style);
        
        style.normal.textColor = Color.red;
        style.fontSize = 20;
        Handles.Label(ringPosition.GCostLocation.position, ringNode.GCost.ToString(CultureInfo.InvariantCulture), style);
    }
}