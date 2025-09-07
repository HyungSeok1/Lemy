using UnityEngine;

public class ParralaxObject2 : MonoBehaviour
{
    [Header("parallaxFactor: 얼마나 움직일지 결정합니다.\n-1에 가까울수록 원근감이 큽니다.\n('-1' ~ '+1' 이내로 설정해야 합니다.)")]
    [SerializeField] private float parallaxFactor;

    public ParallaxCamera parallaxCamera; // 연결안돼있으면 Start()에서 자동연결되나, 연결하는 것을 추천합니다.

    // 오브젝트가 씬에 배치된 최초의 로컬 위치를 저장할 변수입니다.
    private Vector3 startingPosition;

    // 카메라의 최초 위치를 저장할 변수입니다.
    private Vector3 cameraStartingPosition;

    void Awake()
    {
        // 스크립트가 시작될 때, 씬에 배치된 현재 위치를 저장합니다.
        startingPosition = transform.localPosition;

        if (parallaxCamera == null)
            parallaxCamera = Camera.main.GetComponent<ParallaxCamera>();

        if (parallaxCamera != null)
        {
            // 카메라의 시작 위치를 저장합니다.
            cameraStartingPosition = parallaxCamera.transform.position;
        }
    }

    private void OnEnable()
    {
        parallaxCamera.onCameraTranslateAction += Move;
    }

    private void OnDisable()
    {
        parallaxCamera.onCameraTranslateAction -= Move;
    }

    // 이 방식은 카메라가 이동할 때마다 'delta' 만큼만 움직여서, 부동 소수점 오차가 누적될 수 있습니다.
    // 만약 오차가 발생한다면 아래의 주석 처리된 Move_BasedOnTotalMovement 방식으로 변경하는 것을 고려해보세요.

    void Move(Vector2 delta)
    {
        Vector3 newPos = transform.localPosition;
        newPos.x -= delta.x * parallaxFactor;
        newPos.y -= delta.y * parallaxFactor;
        transform.localPosition = newPos;
    }



    // 부동 소수점 오차 누적을 방지하는 대체 이동 함수입니다.
    // 이 함수를 사용하려면 ParallaxCamera 스크립트에서 onCameraTranslate 델리게이트가
    // 현재 카메라 위치를 매개변수로 전달하도록 수정해야 할 수 있습니다.
    // 혹은 LateUpdate()에서 직접 카메라 위치를 가져와서 계산할 수도 있습니다.
    /*
    void Move()
    {
        // 카메라가 시작 위치로부터 얼마나 움직였는지를 계산합니다.
        Vector3 cameraDisplacement = parallaxCamera.transform.position - cameraStartingPosition;
        
        // 오브젝트의 위치를 '원래 시작 위치' + '카메라 총 이동량에 패럴랙스 계수를 곱한 값'으로 설정합니다.
        // 이렇게 하면 매 프레임마다 더하는 방식보다 오차가 적고 안정적입니다.
        transform.localPosition = startingPosition - cameraDisplacement * parallaxFactor;
    }
    */
}