using UnityEngine;

public class TempSaver : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SaveSelectPanel.Instance.OnSaveButtonPressed(1);
    }
}
