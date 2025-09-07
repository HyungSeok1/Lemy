using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
public class Map101_103_Logic : MonoBehaviour
{
    [SerializeField] GameObject keySendingBack;
    [SerializeField] string sendingSceneName; // Inspector에서 다음 씬 이름 지정

    [SerializeField] GameObject[] turnOffWhenAllISDone; // key 먹고 난 후에 오면 끌 것들

    private bool isSendingBack = false;
    private bool alreadyHasKey = false;

    private void Awake()
    {
        if (Key.HasKey(102))
        {
            alreadyHasKey = true; // 이미 키가 있는 상태 기록

            Debug.Log("이 씬에서 더 볼 것은 없습니다.");
            foreach (GameObject go in turnOffWhenAllISDone)
            {
                go.SetActive(false);
            }
        }
    }

    void Update()
    {
        // 씬 처음부터 키를 갖고 있었다면 여기선 아무 것도 하지 않음
        if (alreadyHasKey) return;

        // 키를 새로 주운 경우에만 씬 이동 처리
        if (!isSendingBack && !alreadyHasKey && Key.HasKey(102))
        {
            isSendingBack = true;
        }
    }
}
