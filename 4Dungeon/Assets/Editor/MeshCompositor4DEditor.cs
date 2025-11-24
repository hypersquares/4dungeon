using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshCompositor4D))]
public class MeshCompositor4DEditor : Editor
{
    private MeshCompositor4D m_Compositor;
    private SerializedProperty m_Mesh0Property;
    private SerializedProperty m_Mesh1Property;
    private SerializedProperty m_MeshRenderer4DProperty;
    private SerializedProperty m_ConvergeToPointProperty;
    private SerializedProperty m_UseCustomPointOfConvergenceProperty;
    private SerializedProperty m_PointOfConvergenceProperty;
    private SerializedProperty m_TransformStartProperty;
    private SerializedProperty m_TransformEndProperty;

    // Cached transform values for change detection
    private Transform4D m_CachedTransformStart;
    private Transform4D m_CachedTransformEnd;
    private Euler4 m_CachedStartRotation;
    private Vector4 m_CachedStartPosition;
    private Vector4 m_CachedStartScale;
    private Euler4 m_CachedEndRotation;
    private Vector4 m_CachedEndPosition;
    private Vector4 m_CachedEndScale;

    private void OnEnable()
    {
        m_Compositor = (MeshCompositor4D)target;
        m_Mesh0Property = serializedObject.FindProperty("m_Mesh0");
        m_Mesh1Property = serializedObject.FindProperty("m_Mesh1");
        m_MeshRenderer4DProperty = serializedObject.FindProperty("m_MeshRenderer4D");
        m_ConvergeToPointProperty = serializedObject.FindProperty("m_ConvergeToPoint");
        m_UseCustomPointOfConvergenceProperty = serializedObject.FindProperty("m_UseCustomPointOfConvergence");
        m_PointOfConvergenceProperty = serializedObject.FindProperty("m_PointOfConvergence");
        m_TransformStartProperty = serializedObject.FindProperty("m_TransformStart");
        m_TransformEndProperty = serializedObject.FindProperty("m_TransformEnd");
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
        EditorGUILayout.PropertyField(m_TransformStartProperty, new GUIContent("Transform Start", "Transform applied to Mesh 0 vertices"));

        if (!m_ConvergeToPointProperty.boolValue)
        {
            EditorGUILayout.PropertyField(m_Mesh1Property, new GUIContent("Mesh 1", "The second mesh for 4D composition (w=1)"));
            EditorGUILayout.PropertyField(m_TransformEndProperty, new GUIContent("Transform End", "Transform applied to Mesh 1 vertices"));
        }

        bool meshesChanged = EditorGUI.EndChangeCheck();

        // Detect changes to the Transform4D components themselves
        bool transformsChanged = DetectTransformChanges();

        // Draw the MeshRenderer4D field (read-only display)
        EditorGUILayout.Space();
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(m_MeshRenderer4DProperty);
        EditorGUI.EndDisabledGroup();

        serializedObject.ApplyModifiedProperties();

        // Call CompositeMesh when required meshes are assigned and values changed
        if (meshesChanged || transformsChanged)
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
            serializedObject.Update();
            m_MeshRenderer4DProperty = serializedObject.FindProperty("m_MeshRenderer4D");
        }
    }

    /// <summary>
    /// Composites the mesh and refreshes the entire 4D rendering pipeline.
    /// </summary>
    private void CompositeAndRefresh()
    {
        m_Compositor.CompositeMesh();
        EditorUtility.SetDirty(m_Compositor);

        MeshRenderer4D renderer4D = m_MeshRenderer4DProperty.objectReferenceValue as MeshRenderer4D;
        if (renderer4D == null)
        {
            renderer4D = m_Compositor.GetComponent<MeshRenderer4D>();
        }

        if (renderer4D != null)
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

    /// <summary>
    /// Detects if the Transform4D components have changed since last frame.
    /// </summary>
    private bool DetectTransformChanges()
    {
        bool changed = false;

        Transform4D transformStart = m_TransformStartProperty.objectReferenceValue as Transform4D;
        Transform4D transformEnd = m_TransformEndProperty.objectReferenceValue as Transform4D;

        // Check if transform references changed
        if (transformStart != m_CachedTransformStart || transformEnd != m_CachedTransformEnd)
        {
            m_CachedTransformStart = transformStart;
            m_CachedTransformEnd = transformEnd;
            CacheTransformValues();
            return true;
        }

        // Check start transform values
        if (transformStart != null)
        {
            if (!m_CachedStartRotation.Equals(transformStart.Rotation) ||
                m_CachedStartPosition != transformStart.Position ||
                m_CachedStartScale != transformStart.Scale)
            {
                changed = true;
            }
        }

        // Check end transform values
        if (transformEnd != null)
        {
            if (!m_CachedEndRotation.Equals(transformEnd.Rotation) ||
                m_CachedEndPosition != transformEnd.Position ||
                m_CachedEndScale != transformEnd.Scale)
            {
                changed = true;
            }
        }

        if (changed)
        {
            CacheTransformValues();
        }

        return changed;
    }

    /// <summary>
    /// Caches current transform values for change detection.
    /// </summary>
    private void CacheTransformValues()
    {
        Transform4D transformStart = m_TransformStartProperty.objectReferenceValue as Transform4D;
        Transform4D transformEnd = m_TransformEndProperty.objectReferenceValue as Transform4D;

        if (transformStart != null)
        {
            m_CachedStartRotation = transformStart.Rotation;
            m_CachedStartPosition = transformStart.Position;
            m_CachedStartScale = transformStart.Scale;
        }

        if (transformEnd != null)
        {
            m_CachedEndRotation = transformEnd.Rotation;
            m_CachedEndPosition = transformEnd.Position;
            m_CachedEndScale = transformEnd.Scale;
        }
    }
}
