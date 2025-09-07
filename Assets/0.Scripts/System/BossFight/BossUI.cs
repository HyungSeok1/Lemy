using TMPro;
using UnityEngine;
/// <summary>
/// 체력바, Text 등 관리
/// </summary>
public class BossUI : PersistentSingleton<BossUI>   
{
    private BossBase boss => BossFightSystem.Instance.boss;

    [SerializeField] private BossHealthBarShrink bossHealthBar;
    [SerializeField] private TMP_Text bossText;

    public void OnStartBossFight(BossBase bossRef)
    {
        // 이렇게 안하고, 체력바랑 텍스트가 내려올수도?
        bossHealthBar.gameObject.SetActive(true);
        bossText.gameObject.SetActive(true);


        bossHealthBar.InitBar(boss);
        bossText.text = boss.bossName;
    }

    public void OnEndBossFight()
    {
        bossHealthBar.gameObject.SetActive(false);
        bossText.gameObject.SetActive(false);
    }
}
