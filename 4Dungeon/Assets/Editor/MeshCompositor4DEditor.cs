using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshCompositor4D))]
public class MeshCompositor4DEditor : Editor
{
    private MeshCompositor4D m_Compositor;
    private SerializedProperty m_Mesh0Property;
    private SerializedProperty m_Mesh1Property;
    private SerializedProperty m_TransformProperty;
    private SerializedProperty m_ConvergeToPointProperty;
    private SerializedProperty m_UseCustomPointOfConvergenceProperty;
    private SerializedProperty m_PointOfConvergenceProperty;

    private void OnEnable()
    {
        m_Compositor = (MeshCompositor4D)target;
        m_Mesh0Property = serializedObject.FindProperty("m_mesh0");
        m_Mesh1Property = serializedObject.FindProperty("m_mesh1");
        m_TransformProperty = serializedObject.FindProperty("m_transform");
        m_ConvergeToPointProperty = serializedObject.FindProperty("m_ConvergeToPoint");
        m_UseCustomPointOfConvergenceProperty = serializedObject.FindProperty("m_UseCustomPointOfConvergence");
        m_PointOfConvergenceProperty = serializedObject.FindProperty("m_PointOfConvergence");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(m_ConvergeToPointProperty, new GUIContent("Converge To Point", "When enabled, the mesh converges to a single point instead of using Mesh 1"));

        if (m_ConvergeToPointProperty.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_UseCustomPointOfConvergenceProperty, new GUIContent("Use Custom Convergence Point", "When enabled, use a custom point instead of the mesh center"));

            if (m_UseCustomPointOfConvergenceProperty.boolValue)
            {
                EditorGUILayout.PropertyField(m_PointOfConvergenceProperty, new GUIContent("Point Of Convergence", "The custom 4D point to converge to"));
            }
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.PropertyField(m_Mesh0Property, new GUIContent("Mesh 0", "The first mesh for 4D composition (w=0)"));

        if (!m_ConvergeToPointProperty.boolValue)
        {
            EditorGUILayout.PropertyField(m_Mesh1Property, new GUIContent("Mesh 1", "The second mesh for 4D composition (w=1)"));
        }

        bool meshesChanged = EditorGUI.EndChangeCheck();

        // Draw the transform field (read-only display)
        EditorGUILayout.Space();
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(m_TransformProperty);
        EditorGUI.EndDisabledGroup();

        serializedObject.ApplyModifiedProperties();

        // Call CompositeMesh when required meshes are assigned and values changed
        if (meshesChanged)
        {
            bool convergeToPoint = m_ConvergeToPointProperty.boolValue;
            bool hasRequiredMeshes = m_Mesh0Property.objectReferenceValue != null &&
                                     (convergeToPoint || m_Mesh1Property.objectReferenceValue != null);
            if (hasRequiredMeshes)
            {
                CompositeAndRefresh();
            }
        }

        // Status display
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Status", EditorStyles.boldLabel);

        bool mesh0Set = m_Mesh0Property.objectReferenceValue != null;
        bool mesh1Set = m_Mesh1Property.objectReferenceValue != null;
        bool converge = m_ConvergeToPointProperty.boolValue;

        if (converge && mesh0Set)
        {
            EditorGUILayout.HelpBox("Mesh 0 assigned. Converging to point.", MessageType.Info);
        }
        else if (mesh0Set && mesh1Set)
        {
            EditorGUILayout.HelpBox("Both meshes assigned. Composite mesh has been generated and assigned.", MessageType.Info);
        }
        else
        {
            string missing = "";
            if (!mesh0Set) missing += "Mesh 0";
            if (!converge && !mesh0Set && !mesh1Set) missing += " and ";
            if (!converge && !mesh1Set) missing += "Mesh 1";
            EditorGUILayout.HelpBox($"Missing: {missing}", MessageType.Warning);
        }

        // Manual recomposite button
        EditorGUILayout.Space();
        bool canComposite = mesh0Set && (converge || mesh1Set);
        EditorGUI.BeginDisabledGroup(!canComposite);
        if (GUILayout.Button("Recomposite Mesh"))
        {
            CompositeAndRefresh();
        }
        EditorGUI.EndDisabledGroup();

        // Quick setup button if components are missing
        EditorGUILayout.Space();
        if (GUILayout.Button("Setup All 4D Components"))
        {
            Object4DSetup.SetupObject4D(m_Compositor.gameObject);
            // Refresh our serialized properties
            serializedObject.Update();
            m_TransformProperty = serializedObject.FindProperty("m_transform");
        }
    }

    /// <summary>
    /// Composites the mesh and refreshes the entire 4D rendering pipeline.
    /// </summary>
    private void CompositeAndRefresh()
    {
        m_Compositor.CompositeMesh();
        EditorUtility.SetDirty(m_Compositor);

        // Refresh Transform4D to reinitialize vertices array
        Transform4D transform4D = m_TransformProperty.objectReferenceValue as Transform4D;
        if (transform4D != null)
        {
            transform4D.RefreshMesh();
            EditorUtility.SetDirty(transform4D);

            // Trigger MeshRenderer4D to re-slice
            MeshRenderer4D renderer4D = m_Compositor.GetComponent<MeshRenderer4D>();
            if (renderer4D != null)
            {
                renderer4D.Intersect();
                EditorUtility.SetDirty(renderer4D);

                // Also mark the mesh filter dirty
                if (renderer4D.meshFilter != null)
                {
                    EditorUtility.SetDirty(renderer4D.meshFilter);
                }
            }
        }

        // Force scene view to repaint
        SceneView.RepaintAll();
    }
}
