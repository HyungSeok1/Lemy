using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T> where T : Enum
{
    public T Current { get; private set; }
    readonly Dictionary<T, IState> _map = new();

    public event Action<T, T> OnChanged; // (from, to)

    public void Register(T key, IState state) => _map[key] = state;

    public void Change(T next)
    {
        if (EqualityComparer<T>.Default.Equals(Current, next)) return;
        var prev = Current;
        if (_map.TryGetValue(prev, out var sPrev)) sPrev.OnExit();
        Current = next;
        if (_map.TryGetValue(Current, out var sNext)) sNext.OnEnter();
        OnChanged?.Invoke(prev, Current);
    }

    public void Tick(float dt)
    {
        if (_map.TryGetValue(Current, out var s)) s.Tick(dt);
    }
}

public interface IState
{
    void OnEnter();
    void OnExit();
    void Tick(float dt);
}