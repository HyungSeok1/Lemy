using UnityEngine;

public class LdhTest : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            GameStateManager.Instance.ChangeGameState(GameStateManager.GameState.Playing);
        }

        

    }

}
