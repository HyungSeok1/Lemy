using UnityEngine;
using UnityEditor;

public class ResetPlayerPrefs : MonoBehaviour
{
    [MenuItem("Window/PlayerPrefs 초기화")]
    private static void ResetPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("PlayerPrefs has been reset.");
    }
}
