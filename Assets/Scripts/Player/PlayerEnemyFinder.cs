using UnityEngine;

public class PlayerEnemyFinder : MonoBehaviour
{
    [SerializeField] private float _searchResolution = 15;
    [SerializeField] private float _degreesToCheck = 15;
    [SerializeField] private float _checkCushion = 1f;

    private float _angleStep;
    private LayerMask _enemyMask;
    private Vector2? _moveDirection;

    private void Awake()
    {
        _enemyMask = LayerMask.GetMask("Enemy");
        _angleStep = _degreesToCheck / _searchResolution;
    }

    public Enemy CheckForEnemyInFrontAtPosition(Transform origin, float rayLength, Vector2 moveDirection)
    {
        var originalAngle = Vector2.SignedAngle(moveDirection, origin.up);
        
        var previousAngle = 0f;
        for (var i = 0; i <= _searchResolution; i++)
        {
            // If i == 0, set the first value, if i is even, subtract the step * i from the previous value, otherwise if we're odd, add step * i to the previous value
            var newAngle = i == 0 ? originalAngle : i % 2 != 0 ? previousAngle + i * _angleStep :  previousAngle - i * _angleStep;

            previousAngle = newAngle;

            var point =
                new Vector2(Mathf.Sin(previousAngle * Mathf.Deg2Rad), Mathf.Cos(previousAngle * Mathf.Deg2Rad)) * 5 +
                (Vector2)origin.position;

            var hit = Physics2D.Raycast(transform.position, (point - (Vector2)origin.position).normalized,
                rayLength + _checkCushion, _enemyMask);

            Debug.DrawLine(origin.position, point, Color.red);
            
            if(!hit) continue;

            var enemy = hit.collider.GetComponent<Enemy>();
            
            if(enemy == null) continue;

            return enemy;
        }

        return null;
    }
    
}