using Unity.Cinemachine;
using UnityEngine;

public class CinemachineCall : MonoBehaviour
{
    [Header("카메라 범위용 콜라이더. 진입 확인용 콜라이더와 다른 것임!")]
    [SerializeField] public Collider2D camRange;

    // --- 수정된 부분 ---
    [Header("전환할 카메라의 '이름'을 여기에 적어주세요")]
    [SerializeField] public string toChangeCameraName;
    private CinemachineCamera toChangeCamera; // 인스펙터에서 [SerializeField] 제거
    // ---

    [SerializeField] public float toChangeCamSize;
    [SerializeField] public bool[] toChangeLock;


    private CinemachineCamera basedCamera;
    private float basedCamSize;

    private void Start()
    {
        Initialize();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player") { return; }

        // toChangeCamera가 null이면 (Start에서 못찾았으면) 아무것도 하지 않음
        if (toChangeCamera == null) { return; }

        ChangeCamera();
        
    }

    public void ChangeCamera()
    {
        Initialize();

        var confinedCam = toChangeCamera.GetComponent<ConfinedCinemachine>();
        if (confinedCam != null)
        {
            confinedCam.CameraSetting(camRange, toChangeCamSize, toChangeLock);
        }

        CameraSwitcher.SwitchCamera(toChangeCamera);
        Debug.LogError("카메라 바꿈");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 카메라 전환해야 함
        if (collision.tag != "Player") { return; }

        CameraSwitcher.SwitchCamera(basedCamera);
        // 초기화 정보 더 추가해아할수도
    }

    private void Initialize()
    {
        // 씬이 시작될 때마다 이름으로 카메라를 찾아서 할당합니다.
        GameObject camObject = GameObject.Find(toChangeCameraName);
        if (camObject != null)
        {
            toChangeCamera = camObject.GetComponent<CinemachineCamera>();
        }
        else
        {
            // 카메라를 못 찾았을 경우 에러 메시지를 띄워서 실수를 방지합니다.
        }

        basedCamera = CameraSwitcher.ActiveCamera;
        basedCamSize = Camera.main.orthographicSize;
    }
    void OnDrawGizmos()
    {
        // 카메라 사이즈 미리보기
        Gizmos.color = Color.red;
        float height = toChangeCamSize * 2f;
        float width = height * (16f / 9f);
        Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 0f));
    }
}