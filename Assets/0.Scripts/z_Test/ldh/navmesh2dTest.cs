using UnityEngine;
using UnityEngine.AI;

public class ClickToMove2D : MonoBehaviour
{
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // 2D 환경을 위한 NavMeshAgent 설정
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 좌클릭
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0; // 2D라 z축 보정

            // 클릭 위치로 이동
            agent.SetDestination(mousePos);
        }
    }
}
