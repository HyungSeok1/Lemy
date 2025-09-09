using UnityEngine;

public class LDHTest2 : MonoBehaviour
{
    public MonsterChallenge chal;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            chal.EnemyAllKillCheck();
    }
}
