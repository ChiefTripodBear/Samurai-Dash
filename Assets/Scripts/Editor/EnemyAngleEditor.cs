using System;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(AngleDefinition))]
public class EnemyAngleEditor : Editor
{
    private void OnSceneGUI()
    {
        var angleDefiner = target as AngleDefinition;

        if (angleDefiner == null) return;

        var angle = angleDefiner.Angle;
        var oppositeAngle = angle - 180 < 0 ? 360 - (180 - angle) : angle - 180;
        var pointOne = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad)) + (Vector2) angleDefiner.transform.position;

        var pointTwo =  new Vector2(Mathf.Sin(oppositeAngle * Mathf.Deg2Rad), Mathf.Cos(oppositeAngle *  Mathf.Deg2Rad)) + (Vector2) angleDefiner.transform.position;

        Handles.color = Color.cyan;
        Handles.DrawLine(pointOne, pointTwo);
    }
}