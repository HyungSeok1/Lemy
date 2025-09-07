using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 스킬에 모두 구현될 인터페이스입니다.
/// 
/// skilldata 변수에는 SkillData ScriptableObject가 아닌, 각 스킬마다의 SkillData를 따로 만들어서 할당해주셔야 합니다.
/// 
/// UI 로직에서는 다형성을 사용하여 skilldata를 참조해 icon을 가져옵니다.
/// 
/// </summary>
public interface ISkill
{
    bool CanExecute { get; }
    SkillData skilldata { get; }


    void ExecuteSkill();
    void InitializeSkill();
    /// <summary>
    /// 이 스킬이 다른 스킬로 교체될 때 호출되는 메서드.
    /// <para>
    /// ex. 소환수 3명을 소환하는 스킬이라면, 교체될 때 소환수를 그 즉시 없애야 한다. 이 상황에서 ReleaseSkill에 해당 로직을 넣는다.
    /// </para>
    /// </summary>
    void ReleaseSkill();

    /// <summary>
    /// UI에 쿨타임 시각화해주기 위해 필요한 메서드입니다. (롤에서 스킬 쿨타임 표시해주는 느낌)
    /// <para>
    /// 스킬마다 사용가능 여부 표시 방법이 다르므로, 각 스킬에 해당하는 방법으로 구현하면 됩니다. 
    /// </para>
    /// ex. 활성화 O or X 반환, 쿨타임 반환 등
    /// </summary>
    float GetNormalizedRemainingCooldown();
}

