using UnityEngine;

/// <summary>
/// Bridge class to provide scene view camera from editor scripts to runtime scripts.
/// </summary>
public static class SceneViewCameraProvider
{
    public static Camera currentSceneViewCamera { get; set; }
}
