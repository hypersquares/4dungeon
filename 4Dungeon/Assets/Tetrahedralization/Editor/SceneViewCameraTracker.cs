using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class SceneViewCameraTracker
{
    static SceneViewCameraTracker()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        SceneViewCameraProvider.currentSceneViewCamera = sceneView.camera;
    }
}
