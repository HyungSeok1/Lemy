using System;
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
    [SerializeField] private GameObject keyObjectPrefab;
    [SerializeField] private Transform keySpawnPoint;

    [SerializeField] private ParticleSystem hitSparkPrefab;
    [SerializeField] private Transform sparkSpawnPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        currentHealth = 9999; // 사실상 무적
    }

    public void Open(Action onGetKey)
    {
        // 파괴되는 소리
        Vector3 pos = transform.position;
        SoundManager.Instance.PlaySFXAt("eggCrack2", pos, 1f);

        // 키 생성
        Vector3 spawnPos = sparkSpawnPoint != null ? sparkSpawnPoint.position : transform.position;
        var keyObject = Instantiate(keyObjectPrefab, spawnPos, Quaternion.identity);
        keyObject.transform.localScale = Vector3.one; // 강제로 1,1,1로 맞추기
        keyObject.SetActive(true);
        keyObject.GetComponent<Key>().OnGetKey += onGetKey;


        Destroy(gameObject); 
    }


    public override void TakeDamage(int damage)
    {
        if (currentState == State.Dead) return;

        // 피격되는 소리

        Vector3 spawnPos = sparkSpawnPoint != null ? sparkSpawnPoint.position : transform.position;
        var spark = Instantiate(hitSparkPrefab, spawnPos, Quaternion.identity);
        spark.Play();
    }


    protected override void Attack(Player player) { }
    protected override void OnEnterState(State newState) { }
    protected override void OnUpdateState(State state) { }
    protected override void OnExitState(State oldState) { }

}
