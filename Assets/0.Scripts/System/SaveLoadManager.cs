using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


/// <summary>
/// 각각의 ISaveable을 참조하여 데이터를 저장해놓고,
/// 
/// 로드시켜주는 클래스입니다.
/// 
///  save 시 여기에 있는 filepath에 저장될 예정입니다.
/// 
/// </summary>
public class SaveLoadManager : PersistentSingleton<SaveLoadManager>
{
    private string GetPathForSlot(int slot) => Path.Combine(Application.persistentDataPath, $"save{slot}.json");
    private string GetMetaPathForSlot(int slot) => Path.Combine(Application.persistentDataPath, $"save{slot}.meta.json");

    private GameSaveData saveData;

    public int CurrentSlot
    {
        get => currentSlot;
    }
    private int currentSlot;

    public void SaveGame(int slot)
    {
        saveData = new GameSaveData();
        saveData.playerData = new PlayerData();
        saveData.mapDataWrapper = new MapDataWrapper();

        GameStateManager.Instance.Save(ref saveData.playerData.stateData);
        Player.Instance.Save(ref saveData.playerData.positionData);
        Player.Instance.health.Save(ref saveData.playerData.healthData);
        Player.Instance.playerSkillController.Save(ref saveData.playerData.skillData);
        Player.Instance.inventory.Save(ref saveData.playerData.inventoryData);
        MoneyManager.Instance.Save(ref saveData.playerData.moneyData);
        // TODO: NPC Container도 넣어주기. 지금은 항상 null일 것

        MapDataManager.Instance.Save(ref saveData.mapDataWrapper);

        string json = JsonUtility.ToJson(saveData, true); // pretty print : true
        string path = GetPathForSlot(slot);
        File.WriteAllText(path, json);
    }

    public void LoadGame(int slot)
    {
        currentSlot = slot;

        string path = GetPathForSlot(slot);

        if (!File.Exists(path))
        {
            Debug.LogWarning("path에 json 파일이 없음");
            return;
        }

        string json = File.ReadAllText(path);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);


        GameStateManager.Instance.Load(data.playerData.stateData);
        Player.Instance.health.Load(data.playerData.healthData);
        Player.Instance.playerSkillController.Load(data.playerData.skillData);
        Player.Instance.inventory.Load(data.playerData.inventoryData);
        MoneyManager.Instance.Load(data.playerData.moneyData);
        // TODO: NPC Container도 넣어주기. 지금은 항상 null일 것

        MapDataManager.Instance.Load(data.mapDataWrapper);

        // TODO: 씬 로딩 기능 추가 필요 (transitionManager)

        // TODO: position은 씬 로딩 후 적용해야 함. 변경 필요.
        Player.Instance.Load(data.playerData.positionData);
    }

    public GameSaveData GetCurrentData()
    {
        string path = GetPathForSlot(CurrentSlot);

        if (!File.Exists(path))
        {
            Debug.LogWarning("path에 json 파일이 없음");
            return null;
        }

        string json = File.ReadAllText(path);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

        return data;
    }

    public SaveMeta PeekMeta(int slot)
    {
        if (slot < 1 || slot > 3)
            throw new ArgumentOutOfRangeException(nameof(slot));

        var metaPath = GetMetaPathForSlot(slot);
        if (File.Exists(metaPath))
        {
            var meta = JsonUtility.FromJson<SaveMeta>(File.ReadAllText(metaPath));
            return meta;
        }
        else
        {
            return null;
        }

    }

    // slot 0: 디버그용 슬롯
    public PlayerData PlayerData => saveData.playerData;

}

