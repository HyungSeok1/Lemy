using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// Player의 자식 오브젝트인 PlayerSkills에 붙어있는 스크립트입니다.
/// 
///   <역할>: 
///  스킬실행 로직  처리  
///  
///  UI는 다른 스크립트에서 처리하도록 분리되었습니다. (아이콘 교체, 쿨타임 시각화 등)
/// 
/// </summary>
/// 
public class PlayerSkillController : MonoBehaviour, ISaveable<PlayerSkillData>
{
    public ISkill[] currentSkills = new ISkill[4];
    // Q, W, E

    [SerializeField] public SkillDatabase skillDatabase;

    // Data에서 로드하여 처음에 가져오는 것
    public List<ISkill> learnedSkills = new();

    /// <summary>
    /// 특정 스킬을 배웁니다.
    /// </summary>
    /// <param name="skillName"></param>
    public void AddSkill(string skillName)
    {
        // 스킬 데이터베이스에서 스킬을 추가합니다.
        ISkill newSkill = skillDatabase.AddSkillComponent(gameObject, skillName);
        if (newSkill != null)
        {
            learnedSkills.Add(newSkill);
            Debug.Log($"Skill {skillName} added to PlayerSkills.");
            DialoguePanel.Instance.autoMessagePanel.ShowMessageByTime($"\"{skillName}\" 스킬 획득. 1키를 눌러서 스킬 변경 창을 여십시오.", null, OutputType.Message);

        }
        else
        {
            Debug.LogWarning($"Skill {skillName} could not be added. Check if the skill exists in the SkillDatabase.");
        }
    }

    public Action<int> OnSkillChanged;

    // 맨 처음에 스킬 불러올때도 이 메서드가 호출됩니다.
    public void ChangeSkill(ISkill skillRef, int slot)
    {
        if (skillRef == null)
        {
            currentSkills[slot] = null;
            OnSkillChanged?.Invoke(slot);
            return;
        }

        currentSkills[slot] = skillRef;
        currentSkills[slot].InitializeSkill();

        // UI에서 등록한 이벤트 Invoke
        OnSkillChanged?.Invoke(slot);
    }

    public void ExecuteSkill0(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (currentSkills[0] != null && currentSkills[0].CanExecute)
            currentSkills[0].ExecuteSkill();
        else
        {
            Debug.LogError("Skill not ready or not assigned.");
        }
    }

    public void ExecuteSkill1(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (currentSkills[1] != null && currentSkills[1].CanExecute)
            currentSkills[1].ExecuteSkill();
        else
        {
            Debug.LogError("Skill not ready or not assigned.");
        }
    }
    public void ExecuteSkill2(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (currentSkills[2] != null && currentSkills[2].CanExecute)
            currentSkills[2].ExecuteSkill();
        else
        {
            Debug.Log("Skill not ready or not assigned.");
        }
    }
    public void ExecuteSkill3(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (currentSkills[3] != null && currentSkills[3].CanExecute)
            currentSkills[3].ExecuteSkill();
        else
        {
            Debug.Log("Skill not ready or not assigned.");
        }
    }

    public void Save(ref PlayerSkillData data)
    {
        data.learnedSkills = learnedSkills
        .Where(s => s != null && s.skilldata != null) // 안전 체크
        .Select(s => s.skilldata.skillName)
        .ToArray();
        data.currentSkills[0] = currentSkills[0]?.skilldata.skillName;
        data.currentSkills[1] = currentSkills[1]?.skilldata.skillName;
        data.currentSkills[2] = currentSkills[2]?.skilldata.skillName;
        data.currentSkills[3] = currentSkills[3]?.skilldata.skillName;
    }

    public void Load(PlayerSkillData data)
    {
        foreach (string learnedSkillName in data.learnedSkills)
        {
            var skill = skillDatabase.AddSkillComponent(gameObject, learnedSkillName);
            learnedSkills.Add(skill);

            for (int i = 0; i < 4; i++)
                if (learnedSkillName == data.currentSkills[i])
                    ChangeSkill(skill, i); // 이름 같으면 실행
        }
    }
}
