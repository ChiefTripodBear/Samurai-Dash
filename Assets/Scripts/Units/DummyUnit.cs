using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DummyUnit : MonoBehaviour, IUnit
{
    public AngleDefinition AngleDefinition { get; private set; }
    public UnitKillHandler KillHandler { get; private set; }
    public Transform Transform { get; private set; }
    public Node CurrentNode { get; private set; }

    private NodeGrid _nodeGrid;

    private void Awake()
    {
        _nodeGrid = FindObjectOfType<NodeGrid>();
        AngleDefinition = GetComponent<AngleDefinition>();
        KillHandler = GetComponent<UnitKillHandler>();
        Transform = transform;
    }


    private void OnDisable()
    {
        UnitChainEvaluator.Instance.RemoveUnit(this);
    }

    private void OnEnable()
    {
        Transform = transform;
    }

    private void Start()
    {
        GameUnitManager.RegisterUnit(this);
        UnitChainEvaluator.Instance.AddUnit(this);
    }
    
    private void Update()
    {
        CurrentNode = _nodeGrid.NodeFromWorldPosition(transform.position);

        if (Mouse.current.middleButton.wasReleasedThisFrame) Player.Reload();
    }
}