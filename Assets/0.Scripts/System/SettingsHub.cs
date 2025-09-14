using System;
using UnityEngine;

/// <summary>
/// 게임 내 설정들을 중개하는 싱글톤.
/// ex. AllowCameraShake는 IngameSettings에서 토글 가능하다. 이를 mainCameraScript가 참조한다. (직접 객체 접근 X)
/// 
/// "중개자" 역할입니다.
/// 
/// </summary>
public class SettingsHub : PersistentSingleton<SettingsHub>
{
    public bool AllowCameraShake = true;
}
