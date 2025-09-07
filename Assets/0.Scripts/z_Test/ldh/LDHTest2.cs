using UnityEngine;

public class LDHTest2 : MonoBehaviour
{

    private void OnDestroy()
    {
        print("OnDestroy");
    }

    private void OnEnable()
    {
        print("OnEnable");
    }
}
