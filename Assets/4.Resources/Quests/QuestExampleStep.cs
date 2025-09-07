using UnityEngine;

public class QuestExampleStep : QuestStep
{
    [SerializeField] private bool isCompleted = false;

    private void Start()
    {
        UpdateState();
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.miscEvents.onQuestExampleComplete += QuestExampleComplete;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.miscEvents.onQuestExampleComplete -= QuestExampleComplete;
    }

    private void QuestExampleComplete()
    {
        if (!isCompleted)
        {
            isCompleted = true;
            UpdateState();
            FinishQuestStep();
        }
    }

    private void UpdateState()
    {
        string state = isCompleted.ToString();
        string status = isCompleted ? "Quest step completed." : "Quest step not completed.";
        ChangeState(state, status);
    }

    protected override void SetQuestStepState(string state)
    {
        this.isCompleted = System.Boolean.Parse(state);
        UpdateState();
    }
}
