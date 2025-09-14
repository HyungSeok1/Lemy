using UnityEngine;

/// <summary>
/// Parallax를 사용하는 오브젝트/배경이 있다면, 각 씬에 하나씩 필요.
/// </summary>
public class ParallaxManager : MonoBehaviour
{
    [SerializeField] private Transform basePoint;

    public static Transform BasePoint => Instance.basePoint;
    public static ParallaxManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
}
