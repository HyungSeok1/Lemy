using UnityEngine;

/// <summary>
/// 개편 예정
/// </summary>
/// 
public interface ISaveable<TData>
{
    void Save(ref TData data);
    void Load(TData data);
}
