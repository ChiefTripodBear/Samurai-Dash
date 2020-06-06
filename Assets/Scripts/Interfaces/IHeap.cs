using System;

public interface IHeap<T> : IComparable<T>
{
    int HeapIndex { get; set; }
}