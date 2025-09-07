using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// 실제 제약구간 제작 예시.
/// TryExecite()를 통해 제약구간 발동.
/// Onstart(), OnEnd()에서 시작, 끝 부분 구현 (문 열리고 닫히는 애니메이션 등등...)
/// 
/// !! 반드시 씬에 오브젝트 형태로 존재해야 함 !!
/// 인스펙터 창에서 실행시킬 패턴들을 순서대로 삽입하면 작동함.
/// 
/// </summary>
public class TestZone1 : BaseZone
{
    public GameObject player;

    private void Start()
    {
        Debug.LogWarning("정중앙으로 이동하면 제약구간 1 진입");
    }
    private void Update()
    {
        if (player != null && player.transform.position.x > 0)
        {
            TryExecute(); // 실제 제약구간 작동 메서드
        }
    }

    protected override IEnumerator OnStart()
    {
        Debug.Log("Zone1: 시작 준비 중");
        yield return new WaitForSeconds(3f);
        Debug.Log("Zone1: 시작");
    }

    protected override IEnumerator OnEnd()
    {
        Debug.Log("Zone1: 종료 처리 중");
        yield return new WaitForSeconds(3f);
        Debug.Log("Zone1: 종료");
    }
}
