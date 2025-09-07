using UnityEngine;

public class LdhTest : MonoBehaviour
{
    [SerializeField] private float damage;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Player.Instance.health.TakeDamage(damage);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            print(1);
            print(2);

            var data = SaveLoadManager.Instance.PlayerData;
            if (data == null)
            {
                Debug.LogWarning("data Load Error");
                return;
            }

            if (SceneTransitionManager.Instance != null)
                SceneTransitionManager.Instance.StartTransition("", data);
        }
    }

    public void ASDF()
    {
        print("ASDF");
    }

}
