using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SaveLoadManager))]
public class SaveLoadManagerEditor : Editor
{
    int slot = 1;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SaveLoadManager manager = (SaveLoadManager)target;
        slot = EditorGUILayout.IntSlider("Slot", slot, 1, 3);

        if (GUILayout.Button("Save GameData"))
        {
            manager.SaveGame(slot);
        }

        if (GUILayout.Button("Load GameData"))
        {
            manager.LoadGame(slot);
        }

        if (GUILayout.Button("Reset GameData"))
        {

        }
    }
}
