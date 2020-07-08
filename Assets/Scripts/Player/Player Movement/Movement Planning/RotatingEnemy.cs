using UnityEngine;

public class RotatingEnemy : EnemyUnit
{
    [SerializeField] private float _minRotationDirectionChangeTime = 2f;
    [SerializeField] private float _maxRotationDirectionChangeTime = 10f;
    
    [SerializeField] private float _rotationAmountPerSecond = 10f;
    public override IUnitAttack UnitAttack { get; protected set; }
    public override UnitManager UnitManager { get; protected set; }

    private float _currentRotationDirectionChangeTime;
    private float _rotationTimer;
    
    protected override void Awake()
    { 
        UnitManager = FindObjectOfType<RotatingEnemyManager>();

        _currentRotationDirectionChangeTime =
            Random.Range(_minRotationDirectionChangeTime, _maxRotationDirectionChangeTime);
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();

        _rotationTimer += Time.deltaTime;

        if (_rotationTimer >= _currentRotationDirectionChangeTime)
        {
            _rotationTimer = 0f;
            _rotationAmountPerSecond *= -1;
            _currentRotationDirectionChangeTime =
                Random.Range(_minRotationDirectionChangeTime, _maxRotationDirectionChangeTime);
        }
        
        AngleDefinition.DoAngleRotation(_rotationAmountPerSecond);
    }
}