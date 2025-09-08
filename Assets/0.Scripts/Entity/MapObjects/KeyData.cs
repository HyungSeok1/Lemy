using UnityEngine;

[CreateAssetMenu(fileName = "KeyData", menuName = "Game/KeyData")]
public class KeyData : ScriptableObject
{
    public string ToMoveSceneName;  // 먹으면 이동하게 될 씬 이름
}
