using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor utility for setting up 4D objects with all required components properly wired.
/// </summary>
public static class Object4DSetup
{
    private const string MENU_PATH_COMPOSITE = "GameObject/4D/Composite Object/";
    private const string MENU_PATH_SIMPLE = "GameObject/4D/Simple Object/";

    /// <summary>
    /// Adds all 4D rendering components (including compositor) to the selected GameObject.
    /// </summary>
    [MenuItem(MENU_PATH_COMPOSITE + "Add Components to Selected", false, 10)]
    public static void AddAll4DComponents()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("No Selection", "Please select a GameObject first.", "OK");
            return;
        }

        Undo.SetCurrentGroupName("Add All 4D Components");
        int undoGroup = Undo.GetCurrentGroup();

        SetupCompositeObject4D(selected);

        Undo.CollapseUndoOperations(undoGroup);

        EditorUtility.SetDirty(selected);
        Debug.Log($"Added all 4D components to '{selected.name}'");
    }

    [MenuItem(MENU_PATH_COMPOSITE + "Add Components to Selected", true)]
    private static bool AddAll4DComponentsValidate()
    {
        return Selection.activeGameObject != null;
    }

    /// <summary>
    /// Creates a new 4D composite object with all components pre-configured.
    /// </summary>
    [MenuItem(MENU_PATH_COMPOSITE + "Create New", false, 11)]
    public static void CreateComposite4DObject()
    {
        GameObject obj = new GameObject("4D Composite Object");
        Undo.RegisterCreatedObjectUndo(obj, "Create 4D Composite Object");

        SetupCompositeObject4D(obj);

        Selection.activeGameObject = obj;
        EditorUtility.SetDirty(obj);
        Debug.Log("Created new 4D Composite Object with all components");
    }

    // ==================== Simple Object (no compositor) ====================

    /// <summary>
    /// Adds 4D rendering components (without compositor) to the selected GameObject.
    /// </summary>
    [MenuItem(MENU_PATH_SIMPLE + "Add Components to Selected", false, 20)]
    public static void AddSimple4DComponents()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("No Selection", "Please select a GameObject first.", "OK");
            return;
        }

        Undo.SetCurrentGroupName("Add Simple 4D Components");
        int undoGroup = Undo.GetCurrentGroup();

        SetupSimpleObject4D(selected);

        Undo.CollapseUndoOperations(undoGroup);

        EditorUtility.SetDirty(selected);
        Debug.Log($"Added simple 4D components to '{selected.name}'");
    }

    [MenuItem(MENU_PATH_SIMPLE + "Add Components to Selected", true)]
    private static bool AddSimple4DComponentsValidate()
    {
        return Selection.activeGameObject != null;
    }

    /// <summary>
    /// Creates a new simple 4D object (without compositor) for manual Mesh4D assignment.
    /// </summary>
    [MenuItem(MENU_PATH_SIMPLE + "Create New", false, 21)]
    public static void CreateSimple4DObject()
    {
        GameObject obj = new GameObject("4D Object");
        Undo.RegisterCreatedObjectUndo(obj, "Create Simple 4D Object");

        SetupSimpleObject4D(obj);

        Selection.activeGameObject = obj;
        EditorUtility.SetDirty(obj);
        Debug.Log("Created new simple 4D Object - assign a Mesh4D to MeshRenderer4D");
    }

    // ==================== Setup Methods ====================

    /// <summary>
    /// Sets up simple 4D rendering components (no compositor) for manual Mesh4D assignment.
    /// </summary>
    /// <param name="obj">The GameObject to set up.</param>
    public static void SetupSimpleObject4D(GameObject obj)
    {
        // 1. Add or get MeshFilter (needed by MeshRenderer and MeshRenderer4D)
        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = Undo.AddComponent<MeshFilter>(obj);
        }

        // 2. Add or get MeshRenderer (needed to display the 3D slice)
        MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = Undo.AddComponent<MeshRenderer>(obj);
            meshRenderer.sharedMaterial = GetDefaultMaterial();
        }

        // 3. Add or get Transform4D (core 4D transform)
        Transform4D transform4D = obj.GetComponent<Transform4D>();
        if (transform4D == null)
        {
            transform4D = Undo.AddComponent<Transform4D>(obj);
        }

        // 4. Add or get MeshRenderer4D (owns mesh4D, slices 4D mesh into 3D)
        MeshRenderer4D meshRenderer4D = obj.GetComponent<MeshRenderer4D>();
        if (meshRenderer4D == null)
        {
            meshRenderer4D = Undo.AddComponent<MeshRenderer4D>(obj);
        }
        // Wire up references
        meshRenderer4D.transform4D = transform4D;
        meshRenderer4D.meshFilter = meshFilter;

        // Mark components dirty
        EditorUtility.SetDirty(transform4D);
        EditorUtility.SetDirty(meshRenderer4D);
        EditorUtility.SetDirty(meshFilter);
        EditorUtility.SetDirty(meshRenderer);
    }

    /// <summary>
    /// Sets up all 4D components including compositor on a GameObject.
    /// </summary>
    /// <param name="obj">The GameObject to set up.</param>
    public static void SetupCompositeObject4D(GameObject obj)
    {
        // First set up the base components
        SetupSimpleObject4D(obj);

        // Get references for wiring
        MeshRenderer4D meshRenderer4D = obj.GetComponent<MeshRenderer4D>();

        // Add or get MeshCompositor4D (creates 4D mesh from two 3D meshes)
        MeshCompositor4D compositor = obj.GetComponent<MeshCompositor4D>();
        if (compositor == null)
        {
            compositor = Undo.AddComponent<MeshCompositor4D>(obj);
        }
        // Wire up references via SerializedObject to access private field
        SerializedObject serializedCompositor = new SerializedObject(compositor);
        SerializedProperty meshRenderer4DProp = serializedCompositor.FindProperty("m_MeshRenderer4D");
        meshRenderer4DProp.objectReferenceValue = meshRenderer4D;
        serializedCompositor.ApplyModifiedPropertiesWithoutUndo();

        EditorUtility.SetDirty(compositor);
    }

    /// <summary>
    /// Sets up all 4D components including compositor. Alias for SetupCompositeObject4D.
    /// </summary>
    public static void SetupObject4D(GameObject obj)
    {
        SetupCompositeObject4D(obj);
    }

    private static Material GetDefaultMaterial()
    {
        // Try to get the default material
        Material mat = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");
        if (mat == null)
        {
            // Fallback: create a simple material
            mat = new Material(Shader.Find("Standard"));
        }
        return mat;
    }
}
