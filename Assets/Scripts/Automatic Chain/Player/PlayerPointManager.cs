using System;
using UnityEngine;

public class PlayerPointManager : MonoBehaviour
{
    public static event Action<int, int> OnPointsChanged;
    private int _currentPoints;
    
    private static PlayerPointManager _instance;
    public static PlayerPointManager Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ModifyPoints(int amount)
    {
        _currentPoints += amount;
        OnPointsChanged?.Invoke(amount, _currentPoints);
    }

    public void ResetPoints()
    {
        _currentPoints = 0;
    }
}