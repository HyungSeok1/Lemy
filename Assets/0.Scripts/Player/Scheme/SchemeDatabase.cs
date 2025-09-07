using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SchemeDatabase", menuName = "Game/SchemeDatabase")]
public class SchemeDatabase : ScriptableObject
{
    [SerializeField] private List<SchemeData> allSchemeDataList;

    private Dictionary<string, SchemeData> schemeDataDict;

    private void OnEnable()
    {
        schemeDataDict = new Dictionary<string, SchemeData>();
        foreach (var schemeData in allSchemeDataList)
        {
            schemeDataDict[schemeData.schemeName] = schemeData;
        }
    }

    public IControlScheme AddSchemeComponent(GameObject target, string schemeName)
    {
        // 대응되는 SkillData 찾기
        if (!schemeDataDict.TryGetValue(schemeName, out var schemeData))
        {
            Debug.Log("data 못 찾음");
            return null;
        }

        switch (schemeName)
        {
            case "BasicScheme":
                var scheme = target.AddComponent<BasicScheme>();
                scheme.data = (BasicSchemeData)schemeData;
                return scheme;

            default:
                Debug.LogWarning("schemeName 지정이 잘못됨");
                return null;
        }
    }
}
