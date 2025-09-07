using UnityEngine;

/// <summary>
/// 메인메뉴 씬의 버튼들에 할당된 메서드들이 담긴 스크립트 파일입니다.
/// 
/// 버튼들의 부모 GameObject인 ButtonsParent에 붙어있습니다.
/// 
/// </summary>
public class MainMenu : MonoBehaviour
{
    private void OnEnable()
    {
        GameStateManager.Instance.currentChapter = GameStateManager.Instance.currentMap = -1; //메인메뉴에선 -1입니다.
    }

    public void NewGame()
    {
     

    }

    public void ContinueGame()
    {
      
    }

    public void Options()
    {

    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
