using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transform4D))]
public class Transform4dEditor : Editor
{
    private Transform4D m_Transform4D;
    private SerializedProperty m_Mesh4DProperty;
    private Mesh4D m_PreviousMesh4D;

    private void OnEnable()
    {
        m_Transform4D = (Transform4D)target;
        m_Mesh4DProperty = serializedObject.FindProperty("mesh4D");
        m_PreviousMesh4D = m_Transform4D.mesh4D;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        // Draw default inspector first
        DrawDefaultInspector();

        bool changed = EditorGUI.EndChangeCheck();

        // Apply changes first so the component has the new values
        serializedObject.ApplyModifiedProperties();

        // Check if mesh4D changed (after applying properties)
        Mesh4D currentMesh = m_Transform4D.mesh4D;
        if (currentMesh != m_PreviousMesh4D)
        {
            m_PreviousMesh4D = currentMesh;
            RefreshPipeline();
        }

        DisplayUtilityControls();

        if (changed)
        {
            EditorUtility.SetDirty(m_Transform4D);
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
            m_Transform4D.Rotation = new Euler4();
            m_Transform4D.Position = Vector4.zero;
            m_Transform4D.Scale = Vector4.one;
        }

        if (GUILayout.Button("Reset Rotation"))
        {
            m_Transform4D.Rotation = new Euler4();
        }

        if (GUILayout.Button("Reset Position"))
        {
            m_Transform4D.Position = Vector4.zero;
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Reset Scale to (1,1,1,1)"))
        {
            m_Transform4D.Scale = Vector4.one;
        }
        m_Transform4D.RefreshMesh();
        EditorGUILayout.Space();
        if (GUILayout.Button("Refresh Mesh Pipeline"))
        {
            RefreshPipeline();
        }

        EditorGUI.indentLevel--;
    }

    /// <summary>
    /// Refreshes the entire 4D rendering pipeline.
    /// </summary>
    private void RefreshPipeline()
    {
        m_Transform4D.RefreshMesh();
        EditorUtility.SetDirty(m_Transform4D);

        // Trigger MeshRenderer4D to re-slice
        MeshRenderer4D renderer4D = m_Transform4D.GetComponent<MeshRenderer4D>();
        if (renderer4D != null && m_Transform4D.mesh4D != null)
        {
            renderer4D.Intersect();
            EditorUtility.SetDirty(renderer4D);

            if (renderer4D.meshFilter != null)
            {
                EditorUtility.SetDirty(renderer4D.meshFilter);
            }
        }

        SceneView.RepaintAll();
    }
}
