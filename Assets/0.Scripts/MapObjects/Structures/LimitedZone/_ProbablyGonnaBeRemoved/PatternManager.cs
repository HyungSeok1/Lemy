using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

/// <summary>
/// 
///  수정 전 PatternManager. 
///   
///  PatternManager : 모든 패턴 진행 방식과 그 패턴에 필요한 오브젝트들을 전부 담은 방식
///  ModifiedPatternManager : 각 패턴이 별개의 클래스로 구현되고, 매니저는 필요한 오브젝트만 생성자를 통해 넘겨주는 방식.
/// 
///  실제 사용할 것은 아마 ModifiedPatternManager이나, 혹시 모를 경우를 대비해 남겨둠
///  ModifiedPatternManager를 사용하기로 한 이유는, 나중에 패턴 수가 많아지면 코드가 너무 길어질까봐....
/// 
/// </summary>
///

/*
public class PatternManager : Singleton<PatternManager>
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject testItem;
    public enum PatternName
    {
        Pattern1,
        Pattern2,
    }

    private Dictionary<PatternName, Func<IEnumerator>> patternMap;

    private void Start()
    {
        patternMap = new Dictionary<PatternName, Func<IEnumerator>>
        {
            { PatternName.Pattern1, Pattern1 },
            { PatternName.Pattern2, Pattern2 },
        };
    }

    public IEnumerator GetPattern(PatternName name)
    {
        if (patternMap.TryGetValue(name, out var pattern))
            return pattern();

        Debug.LogWarning($"패턴 {name} 을 찾을 수 없음.");
        return null;
    }
    
    bool AllDestroyed(List<GameObject> list)
    {
        // 리스트의 모든 오브젝트가 null이면 true
        return list.TrueForAll(obj => obj == null);
    }

    IEnumerator Pattern1()
    {
        List<GameObject> spawnedList = new List<GameObject>();

        for (int i = 0; i < 3; i++) // 예: 5개 생성
        {
            Vector3 pos = new Vector3(-3 + i * 2, 0, 0); // 위치 다르게
            GameObject spawned = Instantiate(testItem, pos, Quaternion.identity);
            spawnedList.Add(spawned);
        }

        Debug.Log("프리팹 여러 개 생성됨. 전부 닿아서 사라지면 다음 패턴으로.");

        yield return new WaitUntil(() => AllDestroyed(spawnedList));

        Debug.Log("패턴 1 종료");
    }

    IEnumerator Pattern2()
    {
        Debug.Log("패턴 2 실행");
        yield return new WaitForSeconds(5f);
        Debug.Log("패턴 2 종료");

    }
}
*/