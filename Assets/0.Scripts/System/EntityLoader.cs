using UnityEngine;

public class EntityLoader : MonoBehaviour
{
    public void SpawnEntities(MapData mapData)
    {
        GameObject entityParent = new GameObject($"EntityParent_{mapData.chapter}_{mapData.map}_{mapData.number}");
        foreach (var entityGuide in mapData.entityGuideList)
        {
            GameObject entity;
            if (entityGuide.spawnFlag)
            {
                entity = Instantiate(entityGuide.prefab, entityGuide.position, Quaternion.identity, entityParent.transform);
                entity.GetComponentInChildren<LoadTargetEntity>().myEntityGuide = entityGuide;
            }
        }
    }
}
