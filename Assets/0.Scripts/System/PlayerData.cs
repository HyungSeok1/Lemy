using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSaveData
{
    public PlayerData playerData;
    public MapDataWrapper mapDataWrapper;
}

[Serializable]
public class PlayerData
{
    public PositionData positionData;
    public StateData stateData;
    public HealthData healthData;
    public PlayerSkillData skillData;
    public InventoryData inventoryData;
    public MoneyData moneyData;
    public NPCContainer npcContainer;

    public PlayerData()
    {
        positionData = new PositionData(Vector3.zero);
        stateData = new StateData(-1, -1, -1);
        healthData = new HealthData(0);
        skillData = new PlayerSkillData(null, null, null, null);
        inventoryData = new InventoryData();
        moneyData = new MoneyData(0);
        npcContainer = new NPCContainer();
    }
}

[Serializable]
public class PositionData
{
    public Vector3 pos;
    public PositionData(Vector3 position)
    {
        this.pos = position;
    }

}

[Serializable]
public class StateData
{
    public int chapter;
    public int map;
    public int number;

    public StateData(int chapter, int map, int number)
    {
        this.chapter = chapter;
        this.map = map;
        this.number = number;
    }
}

[Serializable]
public class HealthData
{
    public float health;
    public HealthData(float health)
    {
        this.health = health;
    }
}

[Serializable]
public class PlayerSkillData
{
    public List<string> learnedSkills;
    public string[] currentSkills = new string[4];
    public PlayerSkillData(string s0, string s1, string s2, string s3)
    {
        this.learnedSkills = new List<string>(); // null이면 빈 리스트
        currentSkills[0] = s0;
        currentSkills[1] = s1;
        currentSkills[2] = s2;
        currentSkills[3] = s3;
    }
}


[Serializable]
public class InventoryData
{
    public List<InventorySlotDTO> slots;

    public InventoryData()
    {
        this.slots = new List<InventorySlotDTO>();
    }
}

[Serializable]
public class MoneyData
{
    public int amount;

    public MoneyData(int amount)
    {
        this.amount = amount;
    }
}
[Serializable]
public class NPCContainer
{

    public NPCContainer()
    {

    }
}

[Serializable]
public class MapDataWrapper
{
    public List<MapData> mapList;

    public MapDataWrapper()
    {
        mapList = new();
    }
}

[Serializable]
public class MapData
{
    public int chapter, map, number;
    //엔티티 생성할지말지 등을 결정
    public List<EntityGuide> entityGuideList;
    // 제약구간 활성화여부 등 처리. 제약구간에 딱 들어가면 켜질지말지 결정함
    public List<ChallengeZoneEntry> challengeZoneList;

    // 맵 오브젝트 활성화여부 등 처리. 시작하자마자 어떻게 할지 결정
    public List<MapObjectEntry> saveableMapObjectList;
}

[Serializable]
public class EntityGuide
{
    public Vector3 position;
    public GameObject prefab;
    public bool spawnFlag;
}

[Serializable]
public class ChallengeZoneEntry
{
    //true면 id에 해당하는 제약구간 실행함. 제약구간 클리어하거나, 경우에따라 제약구간 탈출하면 이 플래그를 false로 바꿈
    public string id;
    public bool executionFlag;
}

/// <summary>
/// 실행된거 저장 - ex. Door 열림
/// </summary>
[Serializable]
public class MapObjectEntry
{
    public string id;
    public bool isActivated;
}

public class SaveMeta
{
    public int slot;
    public int chapter;
    public int map;
}
