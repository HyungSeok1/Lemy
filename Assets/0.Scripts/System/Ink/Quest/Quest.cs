using UnityEngine;
using System.Collections.Generic;
using System;
[Serializable]
public class Quest
{
    public QuestInfoSO info;

    public QuestState state;

    public Dictionary<string, int> questIntVariables = new Dictionary<string, int>();

    public QuestLogic questLogicInstance;

    public Quest(QuestInfoSO questInfo)
    {
        this.info = questInfo;
        this.state = QuestState.REQUIREMENTS_NOT_MET;
    }

    #region Quest Variables Functions
    // Get integer variable value, returns defaultValue when missing
    public int GetIntVar(string key, int defaultValue = 0)
    {
        if (string.IsNullOrEmpty(key)) return defaultValue;
        int value;
        return questIntVariables.TryGetValue(key, out value) ? value : defaultValue;
    }

    // Set integer variable value
    public void SetIntVar(string key, int value)
    {
        if (string.IsNullOrEmpty(key)) return;
        int oldValue = GetIntVar(key, 0);
        if (oldValue == value) return; // no change
        questIntVariables[key] = value;
        // notify quest logic
        try
        {
            questLogicInstance?.OnIntVarChanged(key, oldValue, value);
        }
        catch (Exception e)
        {
            Debug.LogError($"OnIntVarChanged error in quest '{info?.id}' key '{key}': {e.Message}");
        }
    }

    // Add delta to integer variable (creates it if missing) and return new value
    public int AddIntVar(string key, int delta)
    {
        if (string.IsNullOrEmpty(key)) return 0;
        int oldValue = GetIntVar(key, 0);
        int newValue = oldValue + delta;
        if (newValue == oldValue) return newValue; // no change
        questIntVariables[key] = newValue;
        // notify quest logic
        try
        {
            questLogicInstance?.OnIntVarChanged(key, oldValue, newValue);
        }
        catch (Exception e)
        {
            Debug.LogError($"OnIntVarChanged error in quest '{info?.id}' key '{key}': {e.Message}");
        }
        return newValue;
    }

    // Try get integer variable
    public bool TryGetIntVar(string key, out int value)
    {
        if (string.IsNullOrEmpty(key))
        {
            value = 0;
            return false;
        }
        return questIntVariables.TryGetValue(key, out value);
    }

    // Compare variable to value with operator string. Supported: >,>=,<,<=,==,!=
    public bool CompareIntVar(string key, string op, int rhs)
    {
        int lhs = GetIntVar(key, 0);
        switch (op)
        {
            case ">": return lhs > rhs;
            case ">=": return lhs >= rhs;
            case "<": return lhs < rhs;
            case "<=": return lhs <= rhs;
            case "==": return lhs == rhs;
            case "!=": return lhs != rhs;
            default:
                Debug.LogWarning($"Unsupported comparison operator '{op}' for quest '{info.id}', key '{key}'.");
                return false;
        }
    }
    #endregion
}
