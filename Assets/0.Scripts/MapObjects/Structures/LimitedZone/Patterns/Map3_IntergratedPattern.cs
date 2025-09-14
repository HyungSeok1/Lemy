using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using DG.Tweening;
using System.Linq;

/// <summary>
/// 
/// 맵3 패턴입니다.
/// 원래 제 의도는 패턴 하나당 스크립트 하나를 만들어서
/// 인스펙터창에서 리스트형식으로 이어붙이는 거였는데
/// 너무 귀찮아서 그냥 한 패턴에 모든 걸 몰아넣었습니다.
/// 그래서 저기 생성자 부분 인자가 많습니다 죄송합니다....
/// 
/// </summary>
public class Map3_IntegratedPattern : IPattern
{
    GameObject lightningRod;
    GameObject junkMass;
    GameObject cyclops;
    GameObject pom;
    GameObject lightning;
    GameObject lightningWarner;

    GameObject spawnEffectWarning_Cy;
    GameObject spawnEffectWarning_Ju;
    GameObject spawnEffect;

    GameObject conditionUI;
    TextMeshProUGUI conditionText;

    private Vector2 _initialConditionUIPos;
    private bool _isUIPosInitialized = false;

    Vector3 zonePos = new Vector3(0, 0, 0);
    private List<GameObject> navmeshTempList = new List<GameObject>(); //cyclops가 삭제되면 여기 리스트도 통째로 삭제
    public Map3_IntegratedPattern(
        GameObject junkMass,
        GameObject cyclops,
        GameObject pom,
        GameObject lightning,
        GameObject lightningRod,
        GameObject lightningWarner,
        GameObject spawnEffectWarning_Cy,
        GameObject spawnEffectWarning_Ju,
        GameObject spawnEffect,

        GameObject conditionUI,
        TextMeshProUGUI conditionText
        )
    {
        this.junkMass = junkMass;
        this.cyclops = cyclops;
        this.pom = pom;
        this.lightning = lightning;
        this.lightningWarner = lightningWarner;
        this.spawnEffectWarning_Ju = spawnEffectWarning_Ju;
        this.spawnEffectWarning_Cy = spawnEffectWarning_Cy;
        this.spawnEffect = spawnEffect;
        this.lightningRod = lightningRod;

        this.conditionUI = conditionUI;
        this.conditionText = conditionText;

    }
    public IEnumerator Execute() // 여기에 일단 맵3 패턴들 존재. 
    {
        SoundManager.Instance.PlayBGM("yeonok_limit_bgm", 1f);
        Debug.Log("맵 3용 묶음 패턴 시작");

        OpenConditionUI();
        yield return M3_P1();
        yield return new WaitForSeconds(1f);
        yield return M3_P2();
        yield return new WaitForSeconds(1f);
        yield return M3_P3();
        yield return new WaitForSeconds(1f);
        yield return M3_P4();
        yield return new WaitForSeconds(1f);
        yield return M3_P5();
        yield return new WaitForSeconds(1f);
        yield return M3_P6();
        yield return new WaitForSeconds(1f);
        

        // yield return M3_P7(); // 부서지는 벽 프리팹 없어서 안만듦

        
        yield return M3_P8();
        yield return M3_P9();
        

        yield return M3_P10();
        yield return M3_P11();

        CloseConditionUI();
        Debug.Log("맵 3용 묶음 패턴 완료");
        yield break;
    }

    public void OpenConditionUI()
    {
        // 처음 함수가 호출될 때만 UI의 원래 위치를 저장합니다.
        if (!_isUIPosInitialized)
        {
            // UI 요소는 RectTransform을 사용하므로 GetComponent로 가져옵니다.
            _initialConditionUIPos = conditionUI.GetComponent<RectTransform>().anchoredPosition;
            _isUIPosInitialized = true;
        }

        conditionText.text = "";

        // conditionUI를 350만큼 아래로 이동시킵니다.
        float targetY = _initialConditionUIPos.y - 350f;
        conditionUI.GetComponent<RectTransform>()
            .DOAnchorPosY(targetY, 1f) 
            .SetEase(Ease.OutExpo);      
    }

    public void ConditionTextTransition(string text)
    {
        // 만약 현재 실행 중인 트윈이 있다면 중복 실행을 막기 위해 즉시 종료시킵니다.
        conditionText.DOKill();

        // 현재 텍스트를 부드럽게 사라지게 합니다.
        conditionText.DOFade(0, 0.3f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // 사라진 후에 텍스트 내용을 새로운 텍스트로 변경합니다.
                conditionText.text = text;

                // 부드럽게 다시 나타나게 합니다.
                conditionText.DOFade(1, 0.3f)
               .SetEase(Ease.InQuad);
            });
    }

    public void SetConditionText(string text)
    {
        conditionText.text = text;
    }

    public void CloseConditionUI()
    {
        // UI가 한 번도 열리지 않았다면(초기 위치가 저장되지 않았다면) 아무것도 하지 않습니다.
        if (!_isUIPosInitialized) return;

        // UI를 부드럽게 원래 Y좌표로 되돌립니다.
        conditionUI.GetComponent<RectTransform>()
            .DOAnchorPosY(_initialConditionUIPos.y, 1f)
            .SetEase(Ease.InExpo);                      

        // 2. 텍스트를 부드럽게 사라지게 합니다.
        conditionText.DOFade(0, 0.5f).OnComplete(() => {
            conditionText.text = "";
            conditionText.alpha = 1f;   
        });
    }

    public IEnumerator M3_P1()
    {
        var spawned = new List<GameObject>();
        var spawnEffects = new List<GameObject>();
        Player player = Player.Instance;
        Vector3 center = zonePos;
        navmeshTempList.Clear();
        int totalEnemies = 12;

        ConditionTextTransition($"적을 모두 처치\n( 0 / {totalEnemies} )");

        // [수정사항 1] 몬스터 소환과 동시에 '감시' 코루틴을 시작시킵니다.
        Coroutine tracker = CoroutineRunner.Instance.StartCoroutine(TrackEnemyKills(spawned, totalEnemies));

        // cyclops 소환 
        CoroutineRunner.Instance.StartCoroutine(SpawnCyclops(1f, new Vector3(12, 12, 0), spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnCyclops(1f, new Vector3(-12, -12, 0), spawned));

        // junkMass 소환
        CoroutineRunner.Instance.StartCoroutine(SpawnJunkMass(1f, new Vector3(20, -20, 0), spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnJunkMass(1f, new Vector3(-20, 20, 0), spawned));

        yield return new WaitForSeconds(3f);


        // junkMass 소환
        CoroutineRunner.Instance.StartCoroutine(SpawnJunkMass(1f, new Vector3(8, 8, 0), spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnJunkMass(1f, new Vector3(-8, -8, 0), spawned));

        yield return new WaitForSeconds(3f);

        // cyclops 소환 
        CoroutineRunner.Instance.StartCoroutine(SpawnCyclops(1f, new Vector3(-12, 12, 0), spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnCyclops(1f, new Vector3(12, -12, 0), spawned));

        // junkMass 소환
        CoroutineRunner.Instance.StartCoroutine(SpawnJunkMass(1f, new Vector3(-20, -20, 0), spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnJunkMass(1f, new Vector3(20, 20, 0), spawned));

        yield return new WaitForSeconds(3f);

        CoroutineRunner.Instance.StartCoroutine(SpawnJunkMass(1f, new Vector3(-8, 8, 0), spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnJunkMass(1f, new Vector3(8, -8, 0), spawned));

        yield return new WaitForSeconds(1.1f);

        //yield return new WaitUntil(() => spawned.TrueForAll(o => o == null)); // 이게 몬스터가 다 죽었는지 확인하는 것
        // [수정사항 2] 기존의 WaitUntil 대신, 감시 코루틴(tracker)이 끝날 때까지 기다립니다.
        yield return tracker;

        foreach (var obj in navmeshTempList) // 이건 외눈박이 작동 방식이 좀 특이해서.... 그거 죽었을 때 처리하는 코드
        {
            if (obj != null)
            {
                Object.Destroy(obj);
            }
        }
        navmeshTempList.Clear();

        Debug.Log("P1 완료");

    }

    public IEnumerator M3_P2()
    {
        ConditionTextTransition("패턴이 끝날 때까지\n생존");

        yield return LightningStripes(zonePos, Random.Range(7, 9), 11, 0.5f);
        yield return new WaitForSeconds(1.5f);
        yield return LightningStripes(zonePos, Random.Range(7, 9), 11, 0.5f, 90f);
        yield return new WaitForSeconds(1.5f);

        SpawnPom(0, 0);
        yield return new WaitForSeconds(2f);

        Debug.Log("P2 완료");

    }

    public IEnumerator M3_P3()
    {
        var spawned = new List<GameObject>();
        var spawnEffects = new List<GameObject>();
        Player player = Player.Instance;
        Vector3 center = zonePos; //player.transform.position;
        navmeshTempList.Clear();
        int totalEnemies = 8;

        ConditionTextTransition($"적을 모두 처치\n( 0 / {totalEnemies} )");

        // [수정사항 1] 몬스터 소환과 동시에 '감시' 코루틴을 시작시킵니다.
        Coroutine tracker = CoroutineRunner.Instance.StartCoroutine(TrackEnemyKills(spawned, totalEnemies));


        // junkMass 소환
        CoroutineRunner.Instance.StartCoroutine(SpawnJunkMass(1f, new Vector3(32, 0, 0), spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnJunkMass(1f, new Vector3(0, 32, 0), spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnJunkMass(1f, new Vector3(0, -32, 0), spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnJunkMass(1f, new Vector3(-32, 0, 0), spawned));

        yield return new WaitForSeconds(3f);

        CoroutineRunner.Instance.StartCoroutine(SingleLightning(zonePos, 0, 0.7f));
        CoroutineRunner.Instance.StartCoroutine(SingleLightning(zonePos, 90, 0.7f));

        yield return new WaitForSeconds(1f);

        CoroutineRunner.Instance.StartCoroutine(SpawnJunkMass(1f, new Vector3(23, 23, 0), spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnJunkMass(1f, new Vector3(-23, 23, 0), spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnJunkMass(1f, new Vector3(23, -23, 0), spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnJunkMass(1f, new Vector3(-23, -23, 0), spawned));

        yield return new WaitForSeconds(3f);

        CoroutineRunner.Instance.StartCoroutine(SingleLightning(zonePos, 45f, 0.7f));
        CoroutineRunner.Instance.StartCoroutine(SingleLightning(zonePos, 135f, 0.7f));


        //yield return new WaitUntil(() => spawned.TrueForAll(o => o == null)); // 이게 몬스터가 다 죽었는지 확인하는 것
        // [수정사항 2] 기존의 WaitUntil 대신, 감시 코루틴(tracker)이 끝날 때까지 기다립니다.
        yield return tracker;

        SpawnPom(0, 0);
        yield return new WaitForSeconds(2f);


        Debug.Log("P3 완료");

    }

    public IEnumerator M3_P4()
    {
        var spawned = new List<GameObject>();
        var spawnEffects = new List<GameObject>();
        Player player = Player.Instance;
        navmeshTempList.Clear();

        int totalEnemies = 4;
        ConditionTextTransition($"적을 모두 처치\n( 0 / {totalEnemies} )");
        Coroutine tracker = CoroutineRunner.Instance.StartCoroutine(TrackEnemyKills(spawned, totalEnemies));



        float spawnX = 0; float spawnY = 0;

        CoroutineRunner.Instance.StartCoroutine(LightningStripes(zonePos, 5, 15, 0.5f));
        CoroutineRunner.Instance.StartCoroutine(LightningStripes(zonePos, 5, 15, 0.5f, 90f));
        // 번개 3개 간격 15. -15 0 15... 7.5로

        spawnX = player.transform.position.x < zonePos.x ? 7.5f : -7.5f;
        spawnY = player.transform.position.y < zonePos.y ? 7.5f : -7.5f;
        CoroutineRunner.Instance.StartCoroutine(SpawnCyclops(1f, new Vector3(spawnX, spawnY, 0), spawned));

        yield return new WaitForSeconds(2.5f);


        CoroutineRunner.Instance.StartCoroutine(LightningStripes(zonePos, 5, 15, 0.5f));
        CoroutineRunner.Instance.StartCoroutine(LightningStripes(zonePos, 5, 15, 0.5f, 90f));

        spawnX = player.transform.position.x < zonePos.x ? 22f : -22f;
        spawnY = player.transform.position.y < zonePos.y ? 22f : -22f;
        CoroutineRunner.Instance.StartCoroutine(SpawnCyclops(1f, new Vector3(spawnX, spawnY, 0), spawned));

        yield return new WaitForSeconds(2.5f);


        CoroutineRunner.Instance.StartCoroutine(LightningStripes(zonePos, 5, 15, 0.5f, 45f));
        CoroutineRunner.Instance.StartCoroutine(LightningStripes(zonePos, 5, 15, 0.5f, 135f));

        if (Mathf.Abs(player.transform.position.x- zonePos.x) > Mathf.Abs(player.transform.position.y - zonePos.y))
        {
            spawnX = player.transform.position.x < zonePos.x ? 11f : -11f;
            spawnY = 0;
        }
        else
        {
            spawnX = 0;
            spawnY = player.transform.position.y < zonePos.y ? 11f : -11;
        }

        CoroutineRunner.Instance.StartCoroutine(SpawnCyclops(1f, new Vector3(spawnX, spawnY, 0), spawned));

        yield return new WaitForSeconds(2.5f);


        CoroutineRunner.Instance.StartCoroutine(LightningStripes(zonePos, 5, 15, 0.5f, 45f));
        CoroutineRunner.Instance.StartCoroutine(LightningStripes(zonePos, 5, 15, 0.5f, 135f));

        if (Mathf.Abs(player.transform.position.x - zonePos.x) > Mathf.Abs(player.transform.position.y - zonePos.y))
        {
            spawnX = player.transform.position.x < zonePos.x ? 32f : -32f;
            spawnY = 0;
        }
        else
        {
            spawnX = 0;
            spawnY = player.transform.position.y < zonePos.y ? 32f : -32f;
        }
        CoroutineRunner.Instance.StartCoroutine(SpawnCyclops(1f, new Vector3(spawnX, spawnY, 0), spawned));

        yield return new WaitForSeconds(2.5f);


        //yield return new WaitUntil(() => spawned.TrueForAll(o => o == null));
        yield return tracker;

        SpawnPom(0, 0);
        yield return new WaitForSeconds(3f);

        foreach (var obj in navmeshTempList)
        {
            if (obj != null)
            {
                Object.Destroy(obj);
            }
        }
        navmeshTempList.Clear();
    }

    public IEnumerator M3_P5()
    {
        int[] angles = {0, 60, 120};
        bool[] attacked = {false , false , false};
        ConditionTextTransition("패턴이 끝날 때까지\n생존");

        for (float i = 0f; i < 7.5; i += 2.5f)
        {
            if (angles[0] == angles[1] || angles[0] == angles[2])
            {
                CoroutineRunner.Instance.StartCoroutine(SingleLightning(zonePos, angles[0], 2f));
                attacked[0] = true;
                if (angles[0] == angles[1]) { attacked[1] = true; }
                if (angles[0] == angles[2]) { attacked[2] = true; }
            }

            if (angles[1] == angles[2])
            {
                CoroutineRunner.Instance.StartCoroutine(SingleLightning(zonePos, angles[1], 2f));
                attacked[1] = true; attacked[2] = true;
            }

            if (attacked[0] == false) { CoroutineRunner.Instance.StartCoroutine(SingleLightning(zonePos, angles[0], 1f)); }
            if (attacked[1] == false) { CoroutineRunner.Instance.StartCoroutine(SingleLightning(zonePos, angles[1], 1f)); }
            if (attacked[2] == false) { CoroutineRunner.Instance.StartCoroutine(SingleLightning(zonePos, angles[2], 1f)); }

            angles[0] -= 30;
            angles[1] += 30;
            angles[2] += 30;

            for(int j = 0; j < 3; j++)
            {
                if (angles[j] < 0) { angles[j] += 180; }
                if (angles[j] >= 180) { angles[j] -= 180; }
                attacked[j] = false;
            }

            yield return new WaitForSeconds(2.5f);
        }
        Debug.Log("P5 완료");
    }

    public IEnumerator M3_P6()
    {
        Vector3 startPos = zonePos - new Vector3(0, 32, 0);
        Vector3 addToStartPos = new Vector3(0, 1, 0);
        int weight = 1;
        float angle = 90f;
        ConditionTextTransition("패턴이 끝날 때까지\n생존");

        for (int j = 0; j < 2; j++)
        {
            switch (Random.Range(0, 4))
            {
                case 0:
                    {
                        startPos = zonePos - new Vector3(0, 36, 0);
                        addToStartPos = new Vector3(0, 1, 0);
                        weight = 1;
                        angle = 90f;
                        break;
                    }
                case 1:
                    {
                        startPos = zonePos + new Vector3(0, 36, 0);
                        addToStartPos = new Vector3(0, 1, 0);
                        weight = -1;
                        angle = 90f;
                        break;
                    }
                case 2:
                    {
                        startPos = zonePos - new Vector3(36, 0, 0);
                        addToStartPos = new Vector3(1, 0, 0);
                        weight = 1;
                        angle = 0f;
                        break;
                    }
                case 3:
                    {
                        startPos = zonePos + new Vector3(36, 0, 0);
                        addToStartPos = new Vector3(1, 0, 0);
                        weight = -1;
                        angle = 0f;
                        break;
                    }
            }
            for (int i = 0; i < 64; i += 4)
            {
                CoroutineRunner.Instance.StartCoroutine(SingleLightning(startPos + (addToStartPos * i * weight), angle, 0.7f));
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(2.3f);
        }

        SpawnPom(0, 0);
        yield return new WaitForSeconds(2f);

        Debug.Log("P6 완료");

    }

    public IEnumerator M3_P7()
    {
        yield break;
    }


    public IEnumerator M3_P8()
    {
        var spawned = new List<GameObject>();
        var spawnEffects = new List<GameObject>();
        Player player = Player.Instance;
        Vector3 center = zonePos;
        navmeshTempList.Clear();

        int totalEnemies = 6;
        ConditionTextTransition($"적을 모두 처치\n( 0 / {totalEnemies} )");
        Coroutine tracker = CoroutineRunner.Instance.StartCoroutine(TrackEnemyKills(spawned, totalEnemies));

        // cyclops 소환 
        Coroutine coroutine = CoroutineRunner.Instance.StartCoroutine(InfiniteTrackingLightning(0.5f, 1, 2.5f));

        CoroutineRunner.Instance.StartCoroutine(SpawnCyclops(1f, new Vector3(0, 30, 0), spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnCyclops(1f, new Vector3(26, -12 , 0), spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnCyclops(1f, new Vector3(-26, -12, 0), spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnJunkMass(1f, new Vector3(26, 12, 0), spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnJunkMass(1f, new Vector3(-26, 12, 0), spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnJunkMass(1f, new Vector3(0, -30, 0), spawned));

        yield return new WaitForSeconds(1.1f);

        //yield return new WaitUntil(() => spawned.TrueForAll(o => o == null)); // 이게 몬스터가 다 죽었는지 확인하는 것
        yield return tracker;

        CoroutineRunner.Instance.StopCoroutine(coroutine);


        foreach (var obj in navmeshTempList) // 이건 외눈박이 작동 방식이 좀 특이해서.... 그거 죽었을 때 처리하는 코드
        {
            if (obj != null)
            {
                Object.Destroy(obj);
            }
        }
        navmeshTempList.Clear();

        Debug.Log("P8 완료");

    }

    public IEnumerator M3_P9()
    {
        var spawned = new List<GameObject>();
        var spawnEffects = new List<GameObject>();
        Player player = Player.Instance;
        Vector3 center = zonePos;
        navmeshTempList.Clear();

        int totalEnemies = 12;
        ConditionTextTransition($"적을 모두 처치\n( 0 / {totalEnemies} )");
        Coroutine tracker = CoroutineRunner.Instance.StartCoroutine(TrackEnemyKills(spawned, totalEnemies));

        Vector3 Cyclops1 = new Vector3(0,16, 0);
        Vector3 Cyclops2 = new Vector3(8, 25, 0);
        Vector3 Cyclops3 = new Vector3(-8, 25, 0);
        Vector3 JunkMass = new Vector3(0, 32, 0);

        CoroutineRunner.Instance.StartCoroutine(SpawnCyclops(1f, Cyclops1, spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnCyclops(1f, Cyclops2, spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnCyclops(1f, Cyclops3, spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnJunkMass(1f, JunkMass, spawned));

        yield return new WaitForSeconds(5f);

        CoroutineRunner.Instance.StartCoroutine(SpawnCyclops(1f, VectorRotator(Cyclops1,120f), spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnCyclops(1f, VectorRotator(Cyclops2, 120f), spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnCyclops(1f, VectorRotator(Cyclops3, 120f), spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnJunkMass(1f, VectorRotator(JunkMass, 120f), spawned));

        CoroutineRunner.Instance.StartCoroutine(SpawnCyclops(1f, VectorRotator(Cyclops1, 240f), spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnCyclops(1f, VectorRotator(Cyclops2, 240f), spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnCyclops(1f, VectorRotator(Cyclops3, 240f), spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnJunkMass(1f, VectorRotator(JunkMass, 240f), spawned));

        yield return new WaitForSeconds(1.1f);

        //yield return new WaitUntil(() => spawned.TrueForAll(o => o == null)); // 이게 몬스터가 다 죽었는지 확인하는 것
        yield return tracker;

        SpawnPom(0, 0);
        yield return new WaitForSeconds(2f);


        foreach (var obj in navmeshTempList) 
        {
            if (obj != null)
            {
                Object.Destroy(obj);
            }
        }
        navmeshTempList.Clear();

        Debug.Log("P9 완료");
    }


    public IEnumerator M3_P10()
    {
        var spawned = new List<GameObject>();
        var spawnEffects = new List<GameObject>();
        Player player = Player.Instance;
        Vector3 center = zonePos;

        ConditionTextTransition("패턴이 끝날 때까지\n생존");

        CoroutineRunner.Instance.StartCoroutine(SpawnLightningRod(1, new Vector3(0,18,0),spawned));
        yield return new WaitForSeconds(4f);
        CoroutineRunner.Instance.StartCoroutine(SpawnLightningRod(1,new Vector3(-15, -9, 0), spawned));
        yield return new WaitForSeconds(4f);
        CoroutineRunner.Instance.StartCoroutine(SpawnLightningRod(1,new Vector3(15, -9, 0), spawned));
        yield return new WaitForSeconds(9f);

        foreach (var obj in spawned) { Object.Destroy(obj); }
    }

    public IEnumerator M3_P11()
    {
        var spawned = new List<GameObject>();
        var spawnEffects = new List<GameObject>();
        Player player = Player.Instance;
        Vector3 center = zonePos;
        navmeshTempList.Clear();
        ConditionTextTransition("패턴이 끝날 때까지\n생존");

        CoroutineRunner.Instance.StartCoroutine(SpawnLightningRod(0, new Vector3(0, 18, 0), spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnLightningRod(0, new Vector3(-15, -9, 0), spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnLightningRod(0, new Vector3(15, -9, 0), spawned));

        CoroutineRunner.Instance.StartCoroutine(SpawnJunkMass(1f, new Vector3(20, 9, 0), spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnJunkMass(1f, new Vector3(-20, 9, 0), spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnJunkMass(1f, new Vector3(0, -23, 0), spawned));

        yield return new WaitForSeconds(5f);

        CoroutineRunner.Instance.StartCoroutine(SpawnCyclops(1f, new Vector3(0, 23, 0), spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnCyclops(1f, new Vector3(20, -9, 0), spawned));
        CoroutineRunner.Instance.StartCoroutine(SpawnCyclops(1f, new Vector3(-20, -9, 0), spawned));

        yield return new WaitForSeconds(13f);

        yield return new WaitForSeconds(1f);
        SpawnPom(0, 0);
        yield return new WaitForSeconds(3f);

        foreach (var obj in spawned) { Object.Destroy(obj); }
        foreach (var obj in navmeshTempList)
        {
            if (obj != null)
            {
                Object.Destroy(obj);
            }
        }

        navmeshTempList.Clear();
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlaySFX("zoneEnd1", 0.2f);
    }




    //외눈박이 소환 코드 // 당신의 외눈박이코드 문어코드로 대체되었다. - 최승혁
    IEnumerator SpawnCyclops(float spawnDelay,Vector3 spawnPosFromCenter, List<GameObject> spawnList)
    {
        SoundManager.Instance.PlaySFX("enemy_spawn3", 0.02f);
        GameObject temp;
        Vector3 spawnPoint = zonePos + new Vector3(spawnPosFromCenter.x, spawnPosFromCenter.y, 11.93487f); // 이거 z 이상한 이유는 버그땜시....

        temp = Object.Instantiate(spawnEffectWarning_Cy, spawnPoint, Quaternion.identity);
        EnemyWarning warning = temp.GetComponent<EnemyWarning>();
        warning.StartWarning(spawnPoint, spawnDelay);
        yield return new WaitForSeconds(spawnDelay);

        Debug.Log("외눈박이 스폰");
        CoroutineRunner.Instance.StartCoroutine(SpawnWithEffect(spawnPoint, cyclops, temp, spawnList, navmeshTempList));

        //navmeshTempList.Add(temp); // navmesh를 일단 저장함 
        //temp.SetActive(true);
        //temp = temp.transform.Find("ArmoredOctopus").gameObject; // 죽여야 하는 적은 navmesh 내부의 Cyclops
        //spawnList.Add(temp);

    }

    // 쓰레기덩어리 소환 코드
    IEnumerator SpawnJunkMass(float spawnDelay, Vector3 spawnPosFromCenter, List<GameObject> spawnList)
    {
        SoundManager.Instance.PlaySFX("enemy_spawn3", 0.02f);
        GameObject temp;
        Vector3 spawnPoint = zonePos + new Vector3(spawnPosFromCenter.x, spawnPosFromCenter.y, 11.93487f);

        temp = Object.Instantiate(spawnEffectWarning_Ju, spawnPoint, Quaternion.identity);
        EnemyWarning warning = temp.GetComponent<EnemyWarning>();
        warning.StartWarning(spawnPoint, spawnDelay);
        yield return new WaitForSeconds(spawnDelay);

        Debug.Log("쓰레기덩어리 스폰");
        CoroutineRunner.Instance.StartCoroutine(SpawnWithEffect(spawnPoint, junkMass, temp, spawnList, navmeshTempList));

        //navmeshTempList.Add(temp); // navmesh를 일단 저장함 
        //temp.SetActive(true);
        //temp = temp.transform.Find("JunkMass").gameObject; // 죽여야 하는 적은 navmesh 내부의 Cyclops
        //spawnList.Add(temp);
    }


    // 피뢰침 소환 코드 (소환 이펙트 임시로 다른것 넣어놓음
    IEnumerator SpawnLightningRod(float spawnDelay, Vector3 spawnPosFromCenter, List<GameObject> spawnList)
    {
        GameObject temp;
        Vector3 spawnPoint = zonePos + new Vector3(spawnPosFromCenter.x, spawnPosFromCenter.y, 0);

        temp = Object.Instantiate(spawnEffectWarning_Ju, spawnPoint, Quaternion.identity);
        EnemyWarning warning = temp.GetComponent<EnemyWarning>();
        warning.StartWarning(spawnPoint, spawnDelay);
        yield return new WaitForSeconds(spawnDelay);

        CoroutineRunner.Instance.StartCoroutine(SpawnWithEffect(spawnPoint, lightningRod, temp, spawnList, navmeshTempList));
        //spawnList.Add(temp);

        yield return null;
    }

    private void SpawnPom(float x, float y)
    {
        Vector3 spawnPoint = new Vector3(x, y, 0f);
        Object.Instantiate(pom, spawnPoint, Quaternion.identity);
        return;
    }

    IEnumerator SpawnWithEffect(Vector3 spawnPoint, GameObject enemy ,GameObject temp ,List<GameObject> spawnList, List<GameObject> navmeshTempList)
    {
        GameObject effect = Object.Instantiate(spawnEffect, spawnPoint, Quaternion.identity);
        if(enemy == cyclops)
        {
            effect.transform.localScale = new Vector3(2, 2, 1);
        }

        Animator anim = effect.GetComponent<Animator>();
        if (anim != null)
        {
            float time = anim.GetCurrentAnimatorStateInfo(0).length / 1.5f;
            yield return new WaitForSeconds(time);
        }

        temp = Object.Instantiate(enemy, spawnPoint, Quaternion.identity);
        

        if(enemy != lightningRod)
        {
            navmeshTempList.Add(temp); // navmesh를 일단 저장함 
            temp.SetActive(true);

            if (enemy == cyclops)
                temp = temp.transform.Find("ArmoredOctopus").gameObject; // 죽여야 하는 적은 navmesh 내부의 Cyclops
            else if (enemy == junkMass)
                temp = temp.transform.Find("JunkMass").gameObject; // 죽여야 하는 적은 navmesh 내부의 Cyclops
        }
            
        spawnList.Add(temp);
    }


    // 번개 생성 각도는 위 방향이 0이고, 오른쪽으로 돌아감.... 따라서 3시방향(가로 번개)은 90f임

    // 번개 하나 생성
    IEnumerator SingleLightning(Vector3 position, float angleZ, float scaleMultiplier, float warningDuration = 1f)
    {
        Quaternion rotation = Quaternion.Euler(0f, 0f, angleZ); // Z축 회전

        // 1단계: 워닝 이펙트 생성
        GameObject warning = Object.Instantiate(lightningWarner, position, rotation);
        warning.SetActive(false);
        var warningScript = warning.GetComponent<LightningWarner>();
        warningScript.Setting(scaleMultiplier, warningDuration);
        warning.SetActive(true);
        warningScript.StartWarning();

        yield return new WaitForSeconds(warningDuration); // 대기 후

        // 2단계: 번개 이펙트 생성
        GameObject lightningStrike = Object.Instantiate(lightning, position, rotation);
        lightningStrike.SetActive(false);
        lightningStrike.transform.localScale *= scaleMultiplier;
        lightningStrike.SetActive(true);
    }


    // 번개 여러개 생성. 숫자, 간격 입력해서 조정
    IEnumerator LightningStripes(Vector3 centerPos, int count, float spacing, float localScale, float angleZ = 0f, float warningDuration = 1f)
    {   
        List<Vector3> positions = new List<Vector3>();

        // 회전 각도로 방향 벡터 계산
        float radians = angleZ * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f).normalized;

        // 첫 오프셋 계산
        float totalLength = (count - 1) * spacing;
        Vector3 startOffset = -direction * (totalLength / 2f);

        // 위치 계산
        for (int i = 0; i < count; i++)
        {
            Vector3 offset = startOffset + direction * spacing * i;
            positions.Add(centerPos + offset);
        }

        Quaternion rotation = Quaternion.Euler(0f, 0f, angleZ);

        // 1단계: 워닝 이펙트 전부 생성
        foreach (var pos in positions)
        {
            GameObject warning = Object.Instantiate(lightningWarner, pos, rotation);
            warning.SetActive(false);
            var warningScript = warning.GetComponent<LightningWarner>();
            warningScript.Setting(localScale, warningDuration);
            warning.SetActive(true);
            warningScript.StartWarning();
        }

        yield return new WaitForSeconds(warningDuration);

        // 2단계: 번개 이펙트 전부 생성
        foreach (var pos in positions)
        {
            GameObject strike = Object.Instantiate(lightning, pos, rotation);
            strike.SetActive(false);
            strike.transform.localScale *= localScale;
            strike.SetActive(true);
        }
    }

    // 무한 플레이어 추적 번개
    IEnumerator InfiniteTrackingLightning(float scaleMultiplier, float warningDuration = 1f, float repeatDelay = 5f)
    {
        while(true)
        {
            Vector3 position = Player.Instance.transform.position;
            float angleZ = Random.Range(0f, 180f);
            yield return SingleLightning(position, angleZ, scaleMultiplier, warningDuration);
            yield return new WaitForSeconds(repeatDelay);
        }
    }

    // 벡터를 넣으면 각도만큼 회전한 벡터를 구해준다
    public Vector3 VectorRotator(Vector3 vec, float ang)
    {
        Vector2 original = vec;
        float angleDeg = ang;
        float rad = angleDeg * Mathf.Deg2Rad;

        float rotatedX = vec.x * Mathf.Cos(rad) - original.y * Mathf.Sin(rad);
        float rotatedY = vec.x * Mathf.Sin(rad) + original.y * Mathf.Cos(rad);

        return new Vector3(rotatedX, rotatedY, 0);
        
    }

    /// <summary>
    /// 몬스터 처치 현황을 실시간으로 추적하고 UI를 업데이트하는 코루틴입니다.
    /// 플레이어 사망 시 자동으로 중단됩니다.
    /// </summary>
    /// <param name="spawnedList">감시할 몬스터 리스트</param>
    /// <param name="totalCount">잡아야 할 총 몬스터 수</param>
    /// 
    private IEnumerator TrackEnemyKills(List<GameObject> spawnedList, int totalCount)
    {
        int killCount = 0;
        while (killCount < totalCount)
        {
            if (Player.Instance == null /* || Player.Instance.IsDead */)
            {
                Debug.Log("플레이어 사망. 몬스터 카운팅을 중단합니다.");
                yield break; // 플레이어가 죽으면 이 코루틴을 즉시 종료합니다.
            }

            int currentDeadCount = spawnedList.Count(o => o == null);
            if (currentDeadCount > killCount)
            {
                killCount = currentDeadCount;
                string text = $"적을 모두 처치\n( {killCount} / {totalCount} )";
                SetConditionText(text);
            }
            yield return null;
        }
    }
}

public class CoroutineRunner : MonoBehaviour // 여기서 코루틴을 쓸려면 이게 있어야한대요 gpt가... 왠지는모름ㅠㅠㅠ
{
    private static CoroutineRunner instance;

    public static CoroutineRunner Instance
    {
        get
        {
            if (instance == null)
            {
                var go = new GameObject("CoroutineRunner");
                DontDestroyOnLoad(go);
                instance = go.AddComponent<CoroutineRunner>();
            }
            return instance;
        }
    }
}
