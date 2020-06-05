using System;

public interface IKillable
{
    event Action OnDeath;
    void Kill();
}