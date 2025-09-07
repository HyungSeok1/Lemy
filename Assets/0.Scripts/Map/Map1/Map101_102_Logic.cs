using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class Map101_102_Logic : MonoBehaviour
{
    [SerializeField] GameObject keySendingBack;
    [SerializeField] string sendingSceneName;

    [SerializeField] GameObject cocoon;
    [SerializeField] List<GameObject> enemies = new List<GameObject>();

    [SerializeField] private GameObject[] turnOffWhenAllISDone;

    private bool isSendingBack = false;
    private bool alreadyHasKey = false;

    private void Awake()
    {
        if (Key.HasKey(101))
        {
            alreadyHasKey = true; // 이미 키가 있는 상태 기록

            Debug.Log("이 씬에서 더 볼 것은 없습니다.");
            foreach (GameObject go in turnOffWhenAllISDone)
            {
                go.SetActive(false);
            }
        }
        else
        {
            StartCoroutine(EnemyAllKillCheck());
        }
    }

    void Update()
    {
        // 씬 처음부터 키를 갖고 있었다면 여기선 아무 것도 하지 않음
        if (alreadyHasKey) return;

        // 키를 새로 주운 경우에만 씬 이동 처리
        if (!isSendingBack && !alreadyHasKey && Key.HasKey(101))
        {
            isSendingBack = true;
        }
    }

    public IEnumerator EnemyAllKillCheck()
    {
        yield return new WaitUntil(() => enemies.TrueForAll(o => o == null));
        cocoon.GetComponent<Cocoon>()?.Open();
    }
}
