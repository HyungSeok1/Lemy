using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System.IO;

public class MapDataManager : PersistentSingleton<MapDataManager>, ISaveable<MapDataWrapper>
{
    [SerializeField] private EntityLoader entityLoader;

    // 맵별 데이터 넣어놓기
    public List<MapData> mapdataList;
    [HideInInspector] public MapData currentMapData;


    // (chapter,map,number) 키
    private readonly Dictionary<MapKey, MapData> _mapIndex = new();

    // 숫자 3개 추출
    private static readonly Regex SceneKeyRegex = new Regex(@"^Scene(?<c>\d+)_(?<m>\d+)_(?<n>\d+)$", RegexOptions.Compiled);

    public void Save(ref MapDataWrapper data)
    {
        data.mapList = mapdataList;
    }

    public void Load(MapDataWrapper data)
    {
        mapdataList = data.mapList;
    }

    protected override void Awake()
    {
        base.Awake();
        RebuildIndex();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// 맨 처음에 맵 Dict 만들어줘서, 이 다음부터는 O(1)에 접근하도록 하기
    /// </summary>
    private void RebuildIndex()
    {
        _mapIndex.Clear();

        foreach (var md in mapdataList)
        {
            var key = new MapKey(md.chapter, md.map, md.number);
            if (_mapIndex.ContainsKey(key))
                Debug.LogWarning($"{key} 중복됨. 다시 제대로 값들을 넣으세요");
            _mapIndex[key] = md;
        }
    }

    /// <summary>
    ///  씬 로드되었을때 엔티티 스폰 등 하는 역할
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!TryParseMapKeyFromSceneName(scene.name, out var key))
        {
            Debug.LogWarning($"{scene.name}에서 추출 실패함\nOR MainMenu 등의 씬임.");
            return;
        }

        if (!_mapIndex.TryGetValue(key, out var mapData))
        {
            Debug.LogError($"[EntityLoader] MapData 없음: {key} (씬: {scene.name}).\nData를 MapDataManager에 넣으세요");
            return;
        }

        currentMapData = mapData;
        entityLoader.SpawnEntities(currentMapData);
    }

    /// <summary>
    /// 씬 이름에서 chapter, map, number 읽어와서 key 반환하는 함수
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    private static bool TryParseMapKeyFromSceneName(string sceneName, out MapKey key)
    {
        key = default;

        var m = SceneKeyRegex.Match(sceneName);
        if (!m.Success) return false;

        int chapter = int.Parse(m.Groups["c"].Value);
        int map = int.Parse(m.Groups["m"].Value);
        int number = int.Parse(m.Groups["n"].Value);

        key = new MapKey(chapter, map, number);
        return true;
    }



    private string SavePath => Path.Combine(Application.persistentDataPath, "mapdataForEditor.json");

    public void SaveMapData()
    {
        var wrapper = new MapDataWrapper { mapList = mapdataList };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"MapData 저장됨: {SavePath}");
    }

    /// <summary>
    /// 지금 있는것들 순회하면서 spawnFlag, executionFlag 전부 true로 만든다. <para>
    /// 저장은 안 해줌. </para>
    /// </summary>
    public void ResetData()
    {
        foreach (MapData data in mapdataList)
        {
            foreach (var entityGuide in data.entityGuideList)
                entityGuide.spawnFlag = true;
            foreach (var sectionEntry in data.challengeZoneList)
                sectionEntry.executionFlag = true;
        }
    }


}
