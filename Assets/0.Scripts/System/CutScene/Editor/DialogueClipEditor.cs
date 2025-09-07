#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(DialogueClip))]
public class DialogueClipEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty clipTypeProp = serializedObject.FindProperty("clipType");
        EditorGUILayout.PropertyField(clipTypeProp);

        // enum 값 검사
        if ((DialogueClipType)clipTypeProp.enumValueIndex == DialogueClipType.StartPoint)
        {
            SerializedProperty knotNameProp = serializedObject.FindProperty("startDialogueKnotName");
            EditorGUILayout.PropertyField(knotNameProp);
        }

        EditorGUILayout.Space();
        SerializedProperty endingProp = serializedObject.FindProperty("isEndingPoint");
        EditorGUILayout.PropertyField(endingProp);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
