using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public PlayerMover PlayerMover { get; private set; }

    private void Awake()
    {
        Time.timeScale = 1f;

        PlayerMover = GetComponent<PlayerMover>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var enemy = other.GetComponent<Enemy>();

        if (enemy == null) return;

        SceneManager.LoadScene(0);
    }
}