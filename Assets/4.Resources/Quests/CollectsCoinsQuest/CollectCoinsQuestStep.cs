using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollectCoinsQuestStep : QuestStep
{
    [SerializeField] private int coinsCollected = 0;
    [SerializeField] private int coinsToComplete = 5;

    private void Start()
    {
        UpdateState();
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.miscEvents.onCoinCollected += CoinCollected;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.miscEvents.onCoinCollected -= CoinCollected;
    }

    private void CoinCollected()
    {
        if (coinsCollected < coinsToComplete)
        {
            coinsCollected++;
            UpdateState();
        }

        if (coinsCollected >= coinsToComplete)
        {
            FinishQuestStep();
        }
    }

    private void UpdateState()
    {
        string state = coinsCollected.ToString();
        string status = "Collected " + coinsCollected + " / " + coinsToComplete + " coins.";
        ChangeState(state, status);
    }

    protected override void SetQuestStepState(string state)
    {
        this.coinsCollected = System.Int32.Parse(state);
        UpdateState();
    }
}
