using UnityEditor;
using UnityEngine;
public class AddCube4D
{
    [MenuItem("GameObject/4D/Cube4D", false, 0)]
    static void CreateCustomObject()
    {
        // Create a new GameObject
        GameObject cube = new GameObject("New Cube4D");

        cube.AddComponent<Transform4D>();
        MeshRenderer4D renderer = cube.AddComponent<MeshRenderer4D>();
        renderer.mesh4D = ScriptableObject.CreateInstance<Cube4D>();
        cube.AddComponent<MeshFilter>();
        cube.AddComponent<MeshRenderer>();

        // Select the newly created object in the Hierarchy
        Selection.activeGameObject = cube;

        // Mark the scene as dirty to ensure changes are saved
        EditorUtility.SetDirty(cube);
    }
}
