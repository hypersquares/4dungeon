using UnityEditor;
using UnityEngine;
public class AddCube4d 
{
    
        // Add a new menu item under "GameObject/My Custom Menu"
        [MenuItem("GameObject/4D/Cube4D", false, 0)]
        static void CreateCustomObject()
        {
            // Create a new GameObject
            GameObject cube = new GameObject("New Cube4D");

            Transform4D trans = cube.AddComponent<Transform4D>();
            trans.mesh4D = ScriptableObject.CreateInstance<Cube4D>();
            cube.AddComponent<MeshRenderer4D>();
            cube.AddComponent<MeshFilter>();
            cube.AddComponent<MeshRenderer>();

            // Select the newly created object in the Hierarchy
            Selection.activeGameObject = cube;

            // Mark the scene as dirty to ensure changes are saved
            EditorUtility.SetDirty(cube);
        }
}
