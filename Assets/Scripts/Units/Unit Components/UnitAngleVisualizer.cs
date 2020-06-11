using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class UnitAngleVisualizer : MonoBehaviour
{
    private AngleDefinition _unitAngle;
    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _unitAngle = GetComponent<AngleDefinition>();
        _lineRenderer = GetComponent<LineRenderer>();
    }
    
    private void Update()
    {
        _lineRenderer.SetPosition(0, _unitAngle.GetPointOne);
        _lineRenderer.SetPosition(1, _unitAngle.GetPointTwo);
    }
}