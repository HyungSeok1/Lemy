using MyCameraTools;
using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// 카메라 제한 범위 두면서 따라가게 하기 위한 코드 
/// x축 / y축 고정을 하고 싶다면 lock을 걸어주면 됨. 이경우 콜라이더 크기는 커도 상관 없음
/// 
/// CinemachineTrigger 스크립트가 collider를 넘겨주면, 그 collider 범위 내에서만 카메라가 움직임
/// 
/// </summary>

public class ConfinedCinemachine : MonoBehaviour
{
    [Header("X LOCK & Y LOCK. X축 Y축 고정여부")]
    public bool[] locks = { false, false };

    [Header("Orthographic Size. 카메라 줌 정도")]
    public float camSize;

    [Header("카메라가 움직일 범위")]
    public Collider2D camBound;

    CinemachineCamera cam;
    CinemachineConfiner2D con;
    AxisLockExtension lockControl;

    private void Awake()
    {
        cam = GetComponent<CinemachineCamera>();
        con = GetComponent<CinemachineConfiner2D>();
        lockControl = GetComponent<AxisLockExtension>();
    }


    // ConfinedCinemachine을 재설정하는 메서드. CameraSwitch 하기 전에 이걸 이용해 카메라 정보를 설정해야함.
    public void CameraSetting(Collider2D boundCollider, float camSize, bool[] locks = null, Transform target = null)
    {

        this.camBound = boundCollider;
        this.camSize = camSize;

        con.BoundingShape2D = null; // 영역 없애버림

        if (target == null) // 타겟 재설정
        {
            cam.Follow = Player.Instance.transform;
        }
        else
        {
            cam.Follow = target;
        }

        if (boundCollider != null && con != null) // 영역 설정 (카메라 움직일 범위)
        {
            con.BoundingShape2D = this.camBound;
        }

        // lock 설정
        if (locks == null)
            locks = new bool[] { false, false };
        else
        {
            lockControl.lockX = locks[0];
            lockControl.lockY = locks[1];
        }

        cam.Lens.OrthographicSize = this.camSize;

        cam.ForceCameraPosition(cam.Follow.position, Quaternion.identity);

    }


    /*
    void OnDrawGizmos()
    {
        // 카메라 사이즈 미리보기 (하늘색)
        Gizmos.color = Color.cyan;
        float height = camSize * 2f; // camSize=15일 때 30
        float width = height * (16f / 9f); // 16:9 비율 적용
        Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 0f));
    }
    */
}
