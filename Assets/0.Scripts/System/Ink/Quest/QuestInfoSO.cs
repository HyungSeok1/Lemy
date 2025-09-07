using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "QuestInfoSO", menuName = "ScriptableObjects/QuestInfoSO", order = 1)]

public class QuestInfoSO : ScriptableObject
{
    [field: SerializeField] public string id {  get; private set; }

    [Header("General")]
    public string displayName;

    [Header("Requirements")]
    public QuestInfoSO[] questPrerequisites;

    [Header("Steps")]
    public GameObject[] questStepPrefabs;


    private void OnValidate()
    {
#if UNITY_EDITOR
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
