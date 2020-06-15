using UnityEngine;

public static class TargetDetector
{
    private const int CheckCushion = 1;
    private static int _searchResolution = 15;
    private static float _degreesToCheck = 15;
    private static float _dotKillThreshold = 0.6f;
    private static readonly float _angleStep = _degreesToCheck / _searchResolution;
    private static Vector2 _firePosition;

    private static LayerMask EnemyMask => LayerMask.GetMask("Enemy");
    
    
    public static IUnit GetValidUnitInFrontFromTargetPosition(IUnit startingUnit, float rayLength,
        Vector2 moveDirection, Vector2 firePosition)
    {
        _firePosition = firePosition;
        var originalAngle = Vector2.SignedAngle(moveDirection, Vector2.up);
        
        var previousAngle = 0f;
        for (var i = 0; i <= _searchResolution; i++)
        {
            var newAngle = i == 0 ? originalAngle : i % 2 != 0 ? previousAngle + i * _angleStep :  previousAngle - i * _angleStep;

            previousAngle = newAngle;

            var point =
                new Vector2(Mathf.Sin(previousAngle * Mathf.Deg2Rad), Mathf.Cos(previousAngle * Mathf.Deg2Rad)) 
                * (rayLength + CheckCushion) + firePosition;

            Debug.DrawLine(firePosition, point, Color.red);
            var hit = Physics2D.Raycast(firePosition, (point - firePosition).normalized,
                rayLength + CheckCushion, EnemyMask);
            
            if(!hit) continue;
            
            var unit = hit.collider.GetComponent<IUnit>();

            if (unit == null || startingUnit != null && startingUnit == unit || !DotProductSuccess(unit, moveDirection)) continue;;

            return unit;
        }

        return null;
    }

    private static bool UnitKillPointInUnwalkable(IUnit unit)
    {
        return PlayerBoundaryDetector.WillCollideWithBoundaryAtTargetLocation(unit.KillHandler.GetFauxKillPoint());
    }

    public static bool FoundInvalidEnemy(float rayLength, Vector2 moveDirection, Vector2 firePosition)
    {
        var originalAngle = Vector2.SignedAngle(moveDirection, Vector2.up);
        
        var previousAngle = 0f;
        for (var i = 0; i <= _searchResolution; i++)
        {
            var newAngle = i == 0 ? originalAngle :
                i % 2 != 0 ? previousAngle + i * _angleStep : previousAngle - i * _angleStep;

            previousAngle = newAngle;

            var point =
                new Vector2(Mathf.Sin(previousAngle * Mathf.Deg2Rad), Mathf.Cos(previousAngle * Mathf.Deg2Rad))
                * (rayLength + CheckCushion) + firePosition;

            var hit = Physics2D.Raycast(firePosition, (point - firePosition).normalized,
                rayLength + CheckCushion, EnemyMask);

            if (!hit) continue;

            var unit = hit.collider.GetComponent<IUnit>();
            
            if (unit != null && !DotProductSuccess(unit, moveDirection)) return true;
        }

        return false;
    }

    public static bool DotProductSuccess(IUnit unit, Vector2 moveDirection)
    {
        return Vector2.Dot(unit.AngleDefinition.KillVector, moveDirection) > _dotKillThreshold;
    }

    public static void DrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(_firePosition, 1f);
    }
}