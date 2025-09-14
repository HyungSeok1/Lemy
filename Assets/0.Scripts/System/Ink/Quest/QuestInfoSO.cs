using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "QuestInfoSO", menuName = "ScriptableObjects/QuestInfoSO", order = 1)]

/// <summary>
/// Quest의 정보를 담고 있는 ScriptableObject
/// </summary>
public class QuestInfoSO : ScriptableObject
{
    [field: SerializeField] public string id { get; private set; }

    [Header("General")]
    public string displayName;

    [Header("Requirements")]
    public QuestInfoSO[] questPrerequisites;

    [Header("Quest Logic Prefab")]
    public GameObject questLogicPrefab; // 실제 퀘스트 로직이 들어있는 프리팹. Instantiate when starting the quest, Destroy when finishing the quest.


    private void OnValidate()
    {
#if UNITY_EDITOR
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
