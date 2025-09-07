using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// 제약 구간의 추상 클래스 틀. (제약구간이긴 한데 이름을 Zone으로 지어서 죄송....)
/// 
/// 이걸 상속받은 Zone을 Zone 폴더에 스크립트 형태로 만들고, 거기서 기능 구현
/// TestZone 파일 보면 이해하기 쉬울 것.
/// 
/// 시작 - 패턴 실행 - 종료로 구성됨.
/// 시작 : 처음 진입시. 문이 닫힌다든가, 애니메이션이 나온다던가....
/// 패턴 실행 : 인스펙터창의 리스트에 원하는 순서로 패턴을 넣으면 그 순서로 실행
/// 종료 : 패턴 모두 완료시. 문이 열린다든가....
/// 
/// </summary>
public abstract class BaseZone : MonoBehaviour
{
    [SerializeField]
    //protected List<PatternManager.PatternType> patternTypes;
    protected List<ModifiedPatternManager.PatternName> patternNames;

    private bool hasExecuted = false;

    protected void TryExecute()
    {
        if (!hasExecuted)
        {
            hasExecuted = true;
            StartCoroutine(ExecuteZoneSequence());
        }
    }

    protected IEnumerator ExecuteZoneSequence()
    {
        yield return StartCoroutine(OnStart());
        yield return StartCoroutine(RunPatterns());
        yield return StartCoroutine(OnEnd());
    }

    protected virtual IEnumerator OnStart()
    {
        Debug.Log("Default Message : Zone OnStart");
        yield return null;
    }

    protected virtual IEnumerator RunPatterns()
    {
        foreach (/*PatternManager*/ModifiedPatternManager.PatternName patternName in patternNames)
        {
            Debug.Log($"BaseZone: 패턴 실행 - {patternName}");
            IEnumerator pattern = /*PatternManager*/ModifiedPatternManager.Instance.GetPattern(patternName);
            if (pattern != null)
                yield return StartCoroutine(pattern);
        }
    }

    protected virtual IEnumerator OnEnd()
    {
        Debug.Log("Default Message : Zone OnEnd");
        yield return null;
    }
}
