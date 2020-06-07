using UnityEngine;

public class TargetEvaluator : MonoBehaviour
{
    [SerializeField] private float _dotKillThreshold;

    [SerializeField] private int _searchResolution = 15;
    [SerializeField] private int _checkCushion = 1;
    [SerializeField] private float _degreesToCheck = 15;

    private float _angleStep;

    private LayerMask _enemyMask;

    private void Awake()
    {
        _enemyMask = LayerMask.GetMask("Enemy");
        _angleStep = _degreesToCheck / _searchResolution;
    }

    
    public IKillableWithAngle EnemyInMoveDirection(Transform origin, float rayLength, Vector2 moveDirection)
    {
        var originalAngle = Vector2.SignedAngle(moveDirection, origin.up);
        
        var previousAngle = 0f;
        for (var i = 0; i <= _searchResolution; i++)
        {
            // If i == 0, set the first value, if i is even, subtract the step * i from the previous value, otherwise if we're odd, add step * i to the previous value
            var newAngle = i == 0 ? originalAngle : i % 2 != 0 ? previousAngle + i * _angleStep :  previousAngle - i * _angleStep;

            previousAngle = newAngle;

            var point =
                new Vector2(Mathf.Sin(previousAngle * Mathf.Deg2Rad), Mathf.Cos(previousAngle * Mathf.Deg2Rad)) * (rayLength + _checkCushion) +
                (Vector2)origin.position;

            var hit = Physics2D.Raycast(transform.position, (point - (Vector2)origin.position).normalized,
                rayLength + _checkCushion, _enemyMask);

            Debug.DrawLine(origin.position, point, Color.red);
            
            if(!hit || hit && hit.collider != null && hit.collider.transform == origin) continue;

            var enemy = hit.collider.GetComponent<IKillableWithAngle>();
            
            if(enemy == null) continue;

            return enemy;
        }

        return null;
    }

    public IKillableWithAngle GetParallelUnitFromTargetLocation(Vector2 targetLocation, float rayLength, Vector2 direction)
    {
        var hit = Physics2D.Raycast(targetLocation, direction, rayLength, _enemyMask);

        if (!hit) return null;

        var enemy = hit.collider.GetComponent<IKillableWithAngle>();

        return enemy;
    }

    public bool DotProductSuccess(IKillableWithAngle unit, Vector2 moveDirection)
    {
        return Vector2.Dot(unit.UnitAngle.KillVector, moveDirection) > _dotKillThreshold;
    }
}