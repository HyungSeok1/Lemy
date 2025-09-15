using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
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
    private string GetPathForSlot(int slot) => Path.Combine(Application.persistentDataPath, $"save{slot}.json"); // 로컬 O.  그러나 .json은 이미 있어야 함
    public string GetMetaPathForSlot(int slot) => Path.Combine(Application.persistentDataPath, $"save{slot}.meta.json"); // 로컬 O.  그러나 .json은 이미 있어야 함
    public string GetLastSlotPath() => Path.Combine(Application.persistentDataPath, $"lastslot.txt"); // 로컬 O, 그러나 .txt는 이미 있어야 함

#if UNITY_EDITOR
    protected override void Awake()
    {
        base.Awake();
    }
#else
    protected override void Awake()
    {
        base.Awake();

        if (IsFirstRun())
        {
            InitializeGameFiles();
            MarkAsNotFirstRun();
        }
    }
#endif

    private GameSaveData saveData;

    public int CurrentSlot
    {
        get => currentSlot;
    }
    private int currentSlot;

    public void SaveGame(int slot)
    {
        File.WriteAllText(GetLastSlotPath(), slot.ToString());

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

        print($"slot {slot} Data Saved");

        SaveMeta saveMeta = new SaveMeta();
        saveMeta.isEmpty = false;
        saveMeta.stateData = saveData.playerData.stateData;
        saveMeta.playtimeData = saveData.playerData.playtimeData;

        string metaJson = JsonUtility.ToJson(saveMeta, true); // pretty print : true
        string metaPath = GetMetaPathForSlot(slot);
        File.WriteAllText(metaPath, metaJson);
    }

    public void LoadGame(int slot)
    {
        Player.Instance.GetComponent<SpriteRenderer>().enabled = true; // 임시로 비활성화했던것, 다시 활성화

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
        Player.Instance.inventory.Load(data.playerData.inventoryData);
        MoneyManager.Instance.Load(data.playerData.moneyData);
        // TODO: NPC Container도 넣어주기. 지금은 항상 null일 것

        MapDataManager.Instance.Load(data.mapDataWrapper);

        Action inputCallback = () => Player.Instance.playerInputController.EnablePlayerActionMap();

        SceneTransitionManager.Instance.StartTransition(data.playerData.stateData, data.playerData.positionData, inputCallback); // 여기서 알아서 씬이동후 포지션 적용

        Player.Instance.playerSkillController.Load(data.playerData.skillData);
    }


    /// <summary>
    /// New Game 눌렀을때 실행.
    /// </summary>
    /// <param name="slot"></param>
    public void LoadNewGame(int slot)
    {
        SoundManager.Instance.StopBGM(); // 동현아 미안 여기 음악멈추는 코드좀 넣을게

        Player.Instance.GetComponent<SpriteRenderer>().enabled = true; // 임시로 비활성화했던것, 다시 활성화

        currentSlot = slot;

        string path = GetPathForSlot(slot);

        if (!File.Exists(path))
        {
            Debug.LogWarning("path에 json 파일이 없음");
            return;
        }

        string json = File.ReadAllText(path);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json); // 깡통 파일 (ResetData됨)

        // TODO: Scene1_1_1 시작지점 직접넣어주기.
        data.playerData.positionData = new PositionData(new Vector3(-25, -5, 0));


        GameStateManager.Instance.Load(data.playerData.stateData);
        Player.Instance.health.Load(data.playerData.healthData);
        Player.Instance.inventory.Load(data.playerData.inventoryData);
        MoneyManager.Instance.Load(data.playerData.moneyData);
        // TODO: NPC Container도 넣어주기. 지금은 항상 null일 것

        MapDataManager.Instance.Load(data.mapDataWrapper);

        Action inputCallback = () => Player.Instance.playerInputController.EnablePlayerActionMap();

        SceneTransitionManager.Instance.StartTransition(data.playerData.stateData, data.playerData.positionData, inputCallback); // 여기서 알아서 씬이동후 포지션 적용

        Player.Instance.playerSkillController.Load(data.playerData.skillData);
    }

    /// <summary>
    /// 유틸
    /// </summary>
    /// <returns></returns>
    public GameSaveData GetCurrentData()
    {
#if UNITY_EDITOR
        string path;
        if (CurrentSlot == 0)
            path = GetPathForSlot(1);
        else
            path = GetPathForSlot(CurrentSlot);
#else
        string path = GetPathForSlot(CurrentSlot);
#endif

        if (!File.Exists(path))
        {
            Debug.LogWarning("path에 json 파일이 없음");
            return null;
        }

        string json = File.ReadAllText(path);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

        return data;
    }

#if UNITY_EDITOR
    /// <summary>
    /// 현재슬롯꺼 직빵으로 PureMapdata.asset으로 변환
    /// </summary>
    /// <param name="slot"></param>
    public void ConvertPureMapdataToAsset(int slot)
    {
        // 현재 세이브데이터 가져오기
        string path = GetPathForSlot(slot);
        if (!File.Exists(path))
        {
            Debug.LogWarning("path에 json 파일이 없음");
            return;
        }
        string json = File.ReadAllText(path);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
        MapDataWrapper pureMapdata = data.mapDataWrapper;

        string assetPath = "Assets/Resources/pureMapData.asset";

        // 기존 에셋이 있는지 확인
        PureMapdataAsset existingAsset = AssetDatabase.LoadAssetAtPath<PureMapdataAsset>(assetPath);

        if (existingAsset != null)
        {
            // 기존 에셋 업데이트
            existingAsset.data = pureMapdata;
            EditorUtility.SetDirty(existingAsset);
        }
        else
        {
            // 새 에셋 생성
            PureMapdataAsset newAsset = ScriptableObject.CreateInstance<PureMapdataAsset>();
            newAsset.data = pureMapdata;
            AssetDatabase.CreateAsset(newAsset, assetPath);
        }

        AssetDatabase.SaveAssets();
    } 
#endif

    public void ResetData(int slot)
    {
        saveData = new GameSaveData(); // isEmpty true
        saveData.playerData = new PlayerData();
        string[] skillList = new string[] { "Dash", "Heal", "Slash" };
        saveData.playerData.skillData = new PlayerSkillData(skillList, "Dash", "Heal", null, "Slash");

        // Pure Map data 가져오기
        PureMapdataAsset asset = Resources.Load<PureMapdataAsset>("pureMapData");
        if (asset != null) saveData.mapDataWrapper = asset.data;
        else Debug.LogError("dasfsdfafds");

        string json = JsonUtility.ToJson(saveData, true); // pretty print : true
        string path = GetPathForSlot(slot);
        File.WriteAllText(path, json);

        print($"slot {slot} Data Reset");

        SaveMeta saveMeta = new SaveMeta();
        saveMeta.isEmpty = true;
        saveMeta.stateData = saveData.playerData.stateData;
        saveMeta.playtimeData = saveData.playerData.playtimeData;

        string metaJson = JsonUtility.ToJson(saveMeta, true); // pretty print : true
        File.WriteAllText(GetMetaPathForSlot(slot), metaJson);
    }

#if !UNITY_EDITOR
    #region json/txt 파일 초기화 (빌드버전 최초 실행시에만.)
    private const string FIRST_RUN_KEY = "GameFirstRun";
   
    private bool IsFirstRun()
    {
        return PlayerPrefs.GetInt(FIRST_RUN_KEY, 1) == 1;
    }

    private void MarkAsNotFirstRun()
    {
        PlayerPrefs.SetInt(FIRST_RUN_KEY, 0);
        PlayerPrefs.Save();
    }

    private void InitializeGameFiles()
    {
        for (int slot = 1; slot <= 3; slot++)
            ResetData(slot);
        File.WriteAllText(GetLastSlotPath(), "1");
    }

    #endregion

#endif
}

