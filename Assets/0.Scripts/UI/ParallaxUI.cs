using UnityEngine;
using UnityEngine.InputSystem;

public class ParallaxUI : MonoBehaviour
{
    [Header("parallaxFactor: 얼마나 움직일지 결정합니다.\n-1에 가까울수록 원근감이 큽니다.\n('-1' ~ '+1' 이내로 설정해야 합니다.)")]
    [SerializeField] private float parallaxFactor;

    private Vector2 oldPos;

    private void Start()
    {
        oldPos = Player.Instance.playerInput.actions["Pointer"].ReadValue<Vector2>();
    }

    private void Update()
    {
        Move(Player.Instance.currentMousePosition - oldPos);
        oldPos = Player.Instance.currentMousePosition;
    }


    void Move(Vector2 delta)
    {
        Vector3 newPos = transform.localPosition;
        newPos.x -= delta.x * parallaxFactor;
        newPos.y -= delta.y * parallaxFactor;
        transform.localPosition = newPos;
    }
}
