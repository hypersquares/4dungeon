using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transform4D))]
public class Transform4dEditor : Editor
{
    private Transform4D transform4d;

    private void OnEnable()
    {
        transform4d = (Transform4D) target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw default inspector first
        DrawDefaultInspector();

        // EditorGUILayout.LabelField("")
        // EditorGUILayout.Space();

        DisplayUtilityControls();

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(transform4d);
        }
    }
    private void DisplayUtilityControls()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Transform Utilities:", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset All"))
        {
            transform4d.Rotation = new Euler4();
            transform4d.Position = Vector4.zero;
            transform4d.Scale = Vector4.one;
        }

        if (GUILayout.Button("Reset Rotation"))
        {
            transform4d.Rotation = new Euler4();
        }

        if (GUILayout.Button("Reset Position"))
        {
            transform4d.Position = Vector4.zero;
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Reset Scale to (1,1,1,1)"))
        {
            transform4d.Scale = Vector4.one;
        }

        EditorGUI.indentLevel--;
    }
}