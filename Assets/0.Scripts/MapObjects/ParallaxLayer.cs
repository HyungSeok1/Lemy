using UnityEngine;

/// <summary>
/// 하나의 배경 블럭이 "Layer"입니다.
/// 각각의 배경에 붙는 클래스입니다.
/// </summary>
[ExecuteInEditMode]
public class ParallaxLayer : MonoBehaviour
{
    [Tooltip("parallaxFactor: 얼마나 움직일지 결정. 0에 가까울수록 원근감 큼")]
    [SerializeField] private float parallaxFactor;

    public void Move(Vector2 delta)
    {
        Vector3 newPos = transform.localPosition;
        newPos.x -= delta.x * parallaxFactor;
        newPos.y -= delta.y * parallaxFactor;
        transform.localPosition = newPos;
    }
}
