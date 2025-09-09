using UnityEngine;

public class AutoLoader : MonoBehaviour
{
    public bool load;
    private void Start()
    {
        if (load)
            SaveLoadManager.Instance.LoadGame(1);
    }
}
