#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapDataManager))]
public class MapDataManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MapDataManager manager = (MapDataManager)target;

        if (GUILayout.Button("Save MapData"))
        {
            manager.SaveMapData();
        }

        if (GUILayout.Button("Reset Data"))
        {
            manager.ResetData();
        }
    }
}
#endif