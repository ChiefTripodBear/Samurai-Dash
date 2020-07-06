using UnityEngine;

public static class TargetDetector
{
    private const int CheckCushion = 2;
    private const int SearchResolution = 15;
    private const float DegreesToCheck = 15;
    private static readonly float AngleStep = DegreesToCheck / SearchResolution;
    private static Vector2 _startCheckPosition;

    private static LayerMask EnemyMask => LayerMask.GetMask("Enemy");

    public static IUnit GetValidUnitInFrontFromTargetPosition(IUnit startingUnit, float rayLength,
        Vector2 moveDirection, Vector2 firePosition, float dotThreshold)
    {
        var originalAngle = Vector2.SignedAngle(moveDirection, Vector2.up);
        
        var previousAngle = 0f;
        for (var i = 0; i <= SearchResolution; i++)
        {
            var newAngle = i == 0 ? originalAngle : i % 2 != 0 ? previousAngle + i * AngleStep :  previousAngle - i * AngleStep;

            previousAngle = newAngle;

            var point =
                new Vector2(Mathf.Sin(previousAngle * Mathf.Deg2Rad), Mathf.Cos(previousAngle * Mathf.Deg2Rad)) 
                * (rayLength + CheckCushion) + firePosition;

            Debug.DrawLine(firePosition, point, Color.red);
            var hit = Physics2D.Raycast(firePosition, (point - firePosition).normalized,
                rayLength + CheckCushion, EnemyMask);
            
            if(!hit) continue;
            
            var unit = hit.collider.GetComponent<IUnit>();

            if (unit == null || startingUnit != null && startingUnit == unit || !DotProductSuccess(unit, moveDirection, dotThreshold)) continue;;

            return unit;
        }

        return null;
    }
    
    public static bool DotProductSuccess(IUnit unit, Vector2 moveDirection, float dotThreshold)
    {
        return Vector2.Dot(unit.AngleDefinition.KillVector, moveDirection) > dotThreshold;
    }
}