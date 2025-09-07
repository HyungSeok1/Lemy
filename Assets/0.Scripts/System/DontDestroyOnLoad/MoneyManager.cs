using UnityEngine;

public class MoneyManager : PersistentSingleton<MoneyManager>, ISaveable<MoneyData>
{
    [SerializeField] private int money;
    public int Money => money;

    public void Save(ref MoneyData data)
    {
        data.amount = Money;
    }

    public void Load(MoneyData data)
    {
        money = data.amount;
    }

    public void AddMoney(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Cannot add negative amount of money.");
            return;
        }

        money += amount;
        Debug.Log($"Added {amount} money. Current balance: {money}");
    }

    public bool SpendMoney(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Cannot spend negative amount of money.");
            return false;
        }

        if (money >= amount)
        {
            money -= amount;
            Debug.Log($"Spent {amount} money. Current balance: {money}");
            return true;
        }
        else
        {
            Debug.LogWarning("Insufficient money.");
            return false;
        }
    }

    public bool HasEnoughMoney(int amount)
    {
        return money >= amount;
    }
}
