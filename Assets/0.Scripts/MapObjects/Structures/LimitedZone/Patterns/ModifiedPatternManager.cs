using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 
/// 패턴을 관리하는 ModifedPatternManager.
/// 반드시 씬에 오브젝트 형태로 존재해야 함.
/// 패턴 동작에 필요한 모든 오브젝트들을 갖고 있는 채로, Start에서 생성자로 오브젝트들을 건네줌.
/// 
/// !! 패턴 스크립트를 생성하고 나면, enum PatternName과 Start() 내부 patternMap도 반드시 갱신해줘야 함 !!
/// 그래야만 각 제약구간 인스펙터 창에서 패턴을 올바르게 인식 후 선택할 수 있음.
/// 
/// </summary>
public class ModifiedPatternManager : Singleton<ModifiedPatternManager>
{
    [SerializeField] GameObject player;
    //[SerializeField] GameObject testItem;

    [Header("Lightning")]
    [SerializeField] GameObject lightningRod;
    [SerializeField] GameObject lightning;
    [SerializeField] GameObject lightningWarner;


    [Header("Enemy")]
    [SerializeField] GameObject junkMass;
    [SerializeField] GameObject armoredOctopus;

    [Header("Enemy Spawn Effect")]
    [SerializeField] GameObject spawnEffectWarning_Cy;
    [SerializeField] GameObject spawnEffectWarning_Ju;
    [SerializeField] GameObject spawnEffect;

    [Header("ConditionUI")]
    [SerializeField] GameObject conditionUI;
    [SerializeField] TextMeshProUGUI conditionText;

    [Header("DroppedItem")]
    [SerializeField] GameObject pom;

    private GameObject canvasObject;

    public enum PatternName // 여기 패턴 새로 만들때마다 갱신
    {
        Pattern1, 
        Pattern2,
        Pattern3,
        Map3_IntegratedPattern
    }

    Dictionary<PatternName, Func<IEnumerator>> patternMap;

    void Start()
    {
        canvasObject = GameObject.Find("UI Canvas");

        Transform[] allChildren = canvasObject.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in allChildren)
        {
            if (t.name == "ConditionUI")
            {
                conditionUI = t.gameObject;
                break;
            }
        }

        if (conditionUI != null)
        {
            conditionText = conditionUI.transform.Find("Condition")?.GetComponent<TextMeshProUGUI>();
        }

        patternMap = new Dictionary<PatternName, Func<IEnumerator>> // 여기 패턴 새로 만들때마다 갱신
        {
            //{ PatternName.Pattern1, () => new TestPattern1(testItem).Execute() }, // 생성자로 필요한 요소 넘겨주기
            { PatternName.Pattern2, () => new TestPattern2().Execute() },
            { PatternName.Pattern3, () => new TestPattern3().Execute() },
            { PatternName.Map3_IntegratedPattern, () => new Map3_IntegratedPattern(junkMass, armoredOctopus, pom,lightning, lightningRod, lightningWarner, spawnEffectWarning_Cy,spawnEffectWarning_Ju, spawnEffect, conditionUI, conditionText).Execute() }
        };
    }

    /// <summary>패턴 타입에 맞는 코루틴 반환</summary>
    public IEnumerator GetPattern(PatternName name)
    {
        if (patternMap.TryGetValue(name, out var factory))
            return factory();

        Debug.LogWarning($"패턴 {name} 을 찾을 수 없음");
        return null;
    }
}
