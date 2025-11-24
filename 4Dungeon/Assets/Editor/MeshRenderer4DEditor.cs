using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(MeshRenderer4D))]
public class MeshRenderer4DEditor : Editor
{
    private MeshRenderer4D m_Renderer;

    private void OnEnable()
    {
        m_Renderer = (MeshRenderer4D)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // Access the private plane field via reflection
        FieldInfo planeField = typeof(MeshRenderer4D).GetField("m_Plane", BindingFlags.NonPublic | BindingFlags.Instance);
        if (planeField != null)
        {
            Plane4D plane = planeField.GetValue(m_Renderer) as Plane4D;
            if (plane != null)
            {
                EditorGUILayout.Space();
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Vector4Field("Plane Point (computed)", plane.point);
                EditorGUI.EndDisabledGroup();
            }
        }
    }
}
