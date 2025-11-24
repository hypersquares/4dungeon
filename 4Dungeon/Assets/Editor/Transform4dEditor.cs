using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transform4D))]
public class Transform4dEditor : Editor
{
    private Transform4D m_Transform4D;

    private void OnEnable()
    {
        m_Transform4D = (Transform4D)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Display label as header if set
        if (!string.IsNullOrEmpty(m_Transform4D.label))
        {
            EditorGUILayout.LabelField(m_Transform4D.label, EditorStyles.boldLabel);
            EditorGUILayout.Space(2);
        }

        EditorGUI.BeginChangeCheck();

        DrawDefaultInspector();

        bool changed = EditorGUI.EndChangeCheck();

        serializedObject.ApplyModifiedProperties();

        DisplayUtilityControls();

        if (changed)
        {
            EditorUtility.SetDirty(m_Transform4D);

            // Trigger MeshRenderer4D to re-slice if present
            MeshRenderer4D renderer4D = m_Transform4D.GetComponent<MeshRenderer4D>();
            if (renderer4D != null && renderer4D.mesh4D != null)
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
            MarkDirtyAndRefresh();
        }

        if (GUILayout.Button("Reset Rotation"))
        {
            m_Transform4D.Rotation = new Euler4();
            MarkDirtyAndRefresh();
        }

        if (GUILayout.Button("Reset Position"))
        {
            m_Transform4D.Position = Vector4.zero;
            MarkDirtyAndRefresh();
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Reset Scale to (1,1,1,1)"))
        {
            m_Transform4D.Scale = Vector4.one;
            MarkDirtyAndRefresh();
        }

        EditorGUI.indentLevel--;
    }

    private void MarkDirtyAndRefresh()
    {
        EditorUtility.SetDirty(m_Transform4D);

        MeshRenderer4D renderer4D = m_Transform4D.GetComponent<MeshRenderer4D>();
        if (renderer4D != null && renderer4D.mesh4D != null)
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
