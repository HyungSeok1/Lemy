using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Cocoon : Enemy
{
    /// <summary>
    /// 
    /// 공격 받으면 소리만 나고, 특정 조건이 만족되어야만 열리는 고치 (상자 비슷한것?).
    /// Enemy를 상속받아 구현함 (공격 안하고, 대미지도 안입음)
    /// 
    /// </summary>
    /// 
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform itemSpawnPoint;

    [SerializeField] private ParticleSystem hitSparkPrefab;
    [SerializeField] private Transform sparkSpawnPoint;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        currentHealth = 9999; // 사실상 무적
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(Input.GetKeyDown(KeyCode.Q))
        {
            Open();
        }
        */
    }
    
    public void Open()
    {
        if (currentState == State.Dead) return;

        // 애니메이션 ( 스프라이트 변경? 컷씬? )

        // 파괴되는 소리
        Vector3 pos = transform.position;
        SoundManager.Instance.PlaySFXAt("eggCrack2", pos, 1f);

        InstantiateObject();

        GetComponent<Collider2D>().enabled = false;

        ChangeState(State.Dead);

        gameObject.SetActive(false); // 파괴된 상태가 되는 애니메이션이 있다면 이건 삭제..

        Debug.Log("Open!");

    }


    public override void TakeDamage(int damage)
    {
        if (currentState == State.Dead) return;

        // 피격되는 소리

        Vector3 spawnPos = sparkSpawnPoint != null ? sparkSpawnPoint.position : transform.position;
        var spark = Instantiate(hitSparkPrefab, spawnPos, Quaternion.identity);
        spark.Play();

        Debug.Log("날 때려도 소용없다.");

    }

    /// <summary>
    /// 자신이 정한 위치에, 자신이 정한 오브젝트를 1,1,1로 소환
    /// </summary>
    void InstantiateObject()
    {
        Vector3 spawnPos = sparkSpawnPoint != null ? sparkSpawnPoint.position : transform.position;
        var item = Instantiate(itemPrefab, spawnPos, Quaternion.identity);
        item.transform.localScale = Vector3.one; // 강제로 1,1,1로 맞추기
        item.SetActive(true);
    }

    protected override void Attack(Player player) { }
    protected override void OnEnterState(State newState) { }
    protected override void OnUpdateState(State state){ }
    protected override void OnExitState(State oldState) { }

}
