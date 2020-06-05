using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EnemyAngleVisualizer : MonoBehaviour
{
    private EnemyAngle _enemyAngle;
    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _enemyAngle = GetComponent<EnemyAngle>();
        _lineRenderer = GetComponent<LineRenderer>();
    }
    
    private void Update()
    {
        _lineRenderer.SetPosition(0, _enemyAngle.GetPointOne);
        _lineRenderer.SetPosition(1, _enemyAngle.GetPointTwo);
    }
}