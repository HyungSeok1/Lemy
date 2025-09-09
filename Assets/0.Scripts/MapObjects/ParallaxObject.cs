using System;
using UnityEngine;

public class ParallaxObject : MonoBehaviour
{
    [Header("parallaxFactor: 얼마나 움직일지 결정합니다.\n-1에 가까울수록 원근감이 큽니다.\n('-1' ~ '+1' 이내로 설정해야 합니다.)")]
    [SerializeField] private float parallaxFactor;

    public ParallaxCamera parallaxCamera; // 연결안돼있으면 Start()에서 자동연결되나, 연결하는 것을 추천합니다.
    // Awake에서 연결하게 수정

    private void Awake()
    {
        if (Camera.main != null)
        {
            parallaxCamera = Camera.main.GetComponent<ParallaxCamera>();
        }
        else
        {
            Debug.LogError("메인 카메라를 찾을 수 없습니다! 카메라에 'MainCamera' 태그가 있는지 확인해주세요.");
        }
    }

    void Start()
    {
        var cam = Camera.main ? Camera.main.transform : null;

        Vector3 camLocal = transform.parent ? transform.parent.InverseTransformPoint(cam.position) : cam.position;
        Vector3 offset = new Vector3(camLocal.x - ParallaxManager.BasePoint.position.x, camLocal.y - ParallaxManager.BasePoint.position.y, 0f);
        transform.localPosition = Vector3.zero - offset * parallaxFactor;
    }

    private void OnEnable()
    {
        parallaxCamera.onCameraTranslateAction += Move;
    }

    private void OnDisable()
    {
        parallaxCamera.onCameraTranslateAction -= Move;
    }

    void Move(Vector2 delta)
    {
        Vector3 newPos = transform.localPosition;
        newPos.x -= delta.x * parallaxFactor;
        newPos.y -= delta.y * parallaxFactor;
        transform.localPosition = newPos;
    }
}
