using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EnvFieldReceiver : MonoBehaviour
{
    private readonly Dictionary<Object, Vector2> _contribs = new();
    public Vector2 EnvSum { get; private set; }

    public void SetContribution(Object sender, Vector2 velocity) => _contribs[sender] = velocity;

    public void RemoveContribution(Object sender)
    {
        if (_contribs.ContainsKey(sender)) _contribs.Remove(sender);
    }

    void LateUpdate()
    {
        Vector2 acc = Vector2.zero;
        foreach (var v in _contribs.Values) acc += v;
        EnvSum = acc;
    }

    void OnDisable()
    {
        _contribs.Clear();
        EnvSum = Vector2.zero;
    }
}
