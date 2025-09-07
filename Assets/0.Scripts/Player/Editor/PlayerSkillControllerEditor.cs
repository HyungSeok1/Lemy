#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;


[CustomEditor(typeof(PlayerSkillController))]
public class PlayerSkillControllerEditor : Editor
{
    private const string PrefKeyNewName = "PSC_Editor_NewSkillName";
    private string newSkillName;

    private int[] slotIds = new[] { 0, 1, 2, 3 };
    private readonly string[] slotLabels = new[] { "Slot 0", "Slot 1", "Slot 2", "Slot 3" };
    private int dbPickIndex = 0;
    private bool learnedDirtyFlag = true;
    private List<string> dbNames;
    private PlayerSkillController controller;
    private string[] learnedSkillSelectPopupOptions;
    private string[] dbNamesPopupOptions;
    private List<ISkill> learned;
    private List<string> learnedNames;
    private string[] slotPopupOptions;  // 캐싱
    private int[] slotPickIndices = new int[4]; // 슬롯 0~3 선택값 저장

    private void OnEnable()
    {
        newSkillName = EditorPrefs.GetString(PrefKeyNewName, "");

        controller = (PlayerSkillController)target;
        dbNames = GetDatabaseSkillNames(controller);
        BuildDBNamesPopupOptions();

        learned = SafeLearned(controller);
        learnedNames = learned.Select(s => s?.skilldata?.skillName ?? "(null)").ToList();
        RebuildSlotPopupOptions();
        BuildPopupOptions();
        learnedDirtyFlag = false;
    }

    public override void OnInspectorGUI()
    {
        // 원래 필드 먼저 (skillDatabase 등)
        DrawDefaultInspector();


        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Player Skill Tools (Editor only)", EditorStyles.boldLabel);

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Play Mode에서만 동작합니다. (컴포넌트 추가/런타임 리스트 변경/슬롯 장착)", MessageType.Info);
        }

        // 현재 Learned / Current 상태 스냅샷
        if (learnedDirtyFlag)
        {
            learned = SafeLearned(controller); // List<ISkill> (null 필터)
            learnedNames = learned.Select(s => s?.skilldata != null ? s.skilldata.skillName : "(null)").ToList();
            RebuildSlotPopupOptions();
            BuildPopupOptions();

            learnedDirtyFlag = false;
        }
        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Add to Learned", EditorStyles.boldLabel);

        using (new EditorGUI.DisabledScope(!Application.isPlaying))
        {
            // 1) 텍스트 입력으로 스킬 추가
            EditorGUILayout.BeginHorizontal();
            newSkillName = EditorGUILayout.TextField("Skill Name", newSkillName);
            if (GUILayout.Button("Add (by name)", GUILayout.Width(120)))
            {
                if (!string.IsNullOrWhiteSpace(newSkillName))
                {
                    try
                    {
                        controller.AddSkill(newSkillName);
                        EditorPrefs.SetString(PrefKeyNewName, newSkillName);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
                else
                {
                    Debug.LogWarning("스킬 이름을 입력하세요.");
                }
            }
            EditorGUILayout.EndHorizontal();

            // 2) SkillDatabase로부터 선택 팝업 제공 (가능하면)
            if (dbNames != null && dbNames.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();

                dbPickIndex = EditorGUILayout.Popup("From Database", dbPickIndex, dbNamesPopupOptions);

                using (new EditorGUI.DisabledScope(dbPickIndex == 0))
                {
                    if (GUILayout.Button("Add (from DB)", GUILayout.Width(120)))
                    {
                        string chosen = dbNames[dbPickIndex - 1];
                        try
                        {
                            controller.AddSkill(chosen);
                            learnedDirtyFlag = true;
                        }
                        catch (System.Exception e) { Debug.LogException(e); }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox("SkillDatabase에서 이름 목록을 가져올 수 없었습니다. (텍스트로 추가 가능)", MessageType.None);
            }
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Learned Skills", EditorStyles.boldLabel);

        if (learned.Count == 0)
        {
            EditorGUILayout.HelpBox("배운 스킬이 없습니다.", MessageType.Info);
        }
        else
        {
            // Learned 리스트 표시 + Unlearn 버튼
            for (int i = 0; i < learned.Count; i++)
            {
                var s = learned[i];
                string sName = s?.skilldata != null ? s.skilldata.skillName : "(null)";
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"{i}. {sName}");
                using (new EditorGUI.DisabledScope(!Application.isPlaying))
                {
                    if (GUILayout.Button("Unlearn", GUILayout.Width(80)))
                    {
                        try
                        {
                            controller.learnedSkills.Remove(s);
                            // 컴포넌트 제거 (ISkill이 MonoBehaviour일 것을 가정)
                            if (s is Component c)
                                DestroyImmediate(c);

                            learnedDirtyFlag = true;
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Assign to Slots", EditorStyles.boldLabel);



        using (new EditorGUI.DisabledScope(!Application.isPlaying))
        {
            for (int si = 0; si < slotIds.Length; si++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(slotLabels[si], GUILayout.Width(60));
                slotPickIndices[si] = EditorGUILayout.Popup(slotPickIndices[si], learnedSkillSelectPopupOptions, GUILayout.Width(75));

                // 현재 장착중인 스킬 표시
                string current = "(none)";
                var cur = GetCurrent(controller, si);
                if (cur != null && cur.skilldata != null) current = cur.skilldata.skillName;
                EditorGUILayout.LabelField($"Current: {current}", GUILayout.Width(100));

                if (GUILayout.Button("Apply", GUILayout.Width(70)))
                {
                    int chosen = slotPickIndices[si];

                    if (chosen > 0) //0인 경우: unchanged
                    {
                        var skill = learned[chosen - 1];
                        if (skill != null)
                        {
                            try { controller.ChangeSkill(skill, si); }
                            catch (System.Exception e) { Debug.LogException(e); }
                        }
                    }
                }

                // 슬롯 비우기(주의: ChangeSkill가 null 허용 안 하면 NRE)
                if (GUILayout.Button("Clear", GUILayout.Width(60)))
                {
                    try { controller.ChangeSkill(null, si); }
                    catch (System.Exception e) { Debug.LogException(e); }
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        // 인스펙터 갱신
        if (GUI.changed)
        {
            EditorUtility.SetDirty(controller);
        }
    }

    // Learned 스냅샷(널 필터)
    private static List<ISkill> SafeLearned(PlayerSkillController c)
    {
        return c.learnedSkills?.Where(s => s != null)?.ToList() ?? new List<ISkill>();
    }

    // 현재 슬롯 스킬
    private static ISkill GetCurrent(PlayerSkillController c, int slot)
    {
        if (c.currentSkills == null || c.currentSkills.Length <= slot) return null;
        return c.currentSkills[slot];
    }

    // SkillDatabase로부터 스킬 이름 후보 가져오기
    private static List<string> GetDatabaseSkillNames(PlayerSkillController c)
    {
        if (c == null) return null;

        var fDb = typeof(PlayerSkillController)
            .GetField("skillDatabase", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        var db = fDb?.GetValue(c) as SkillDatabase;
        if (db == null) return null;

        var fList = typeof(SkillDatabase)
            .GetField("allSkillDataList", BindingFlags.Instance | BindingFlags.NonPublic);
        var listObj = fList?.GetValue(db) as IEnumerable;
        if (listObj == null) return null;

        var names = new List<string>();
        foreach (var item in listObj)
        {
            if (item == null) continue;
            var t = item.GetType();

            // SkillData.skillName (필드 or 프로퍼티 대응)
            var fName = t.GetField("skillName", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var pName = t.GetProperty("skillName", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            string n = null;
            if (fName != null) n = fName.GetValue(item) as string;
            else if (pName != null) n = pName.GetValue(item) as string;

            if (!string.IsNullOrWhiteSpace(n)) names.Add(n);
        }

        return names.Count > 0
            ? names.Distinct().OrderBy(n => n).ToList()
            : null;
    }


    private void BuildDBNamesPopupOptions()
    {
        var options = new string[dbNames.Count + 1];
        options[0] = "-- 선택 --";
        for (int i = 0; i < dbNames.Count; i++)
            options[i + 1] = dbNames[i];
        dbNamesPopupOptions = options;
    }

    private void BuildPopupOptions()
    {
        var options = new string[learnedNames.Count + 1];
        options[0] = "-- 선택 --";
        for (int i = 0; i < learnedNames.Count; i++)
            options[i + 1] = learnedNames[i];
        learnedSkillSelectPopupOptions = options;
    }


    private void RebuildSlotPopupOptions()
    {
        var popupItems = new List<string> { "(unchanged)" };
        if (learnedNames != null) popupItems.AddRange(learnedNames);
        slotPopupOptions = popupItems.ToArray();
    }

}

#endif

