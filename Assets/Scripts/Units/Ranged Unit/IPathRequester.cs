using System;
using UnityEngine;

public interface IPathRequester
{
    Transform Mover { get; }
    event Action<PathRequest> PathRequested;
    event Action PathCompleted;
}