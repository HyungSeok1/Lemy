using UnityEngine;

/// <summary>
/// 
/// 
/// 추후 skillName으로 저장 관련 코드를 짤 계획입니다.
/// 
/// </summary>
public class SkillData : ScriptableObject
{
    public string skillName;
    public Sprite icon;
    public int stackGaugeCost; // 스택 게이지 소모량 (int)
}
