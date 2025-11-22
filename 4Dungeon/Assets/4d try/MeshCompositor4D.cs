using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Composites two 3D meshes into a 4D mesh by positioning them at different W coordinates.
/// The first mesh is placed at W=0, the second at W=1.
/// </summary>
[System.Serializable]
[ExecuteInEditMode]
public static class MeshCompositor4D
{
    public static Mesh4D CompositeMeshes(Mesh mesh0, Mesh mesh1)
{
    List<Vector4> vertsAtZero = new();
    foreach (Vector3 vert in mesh0.vertices)
    {
          vertsAtZero.Append(new Vector4(vert.x, vert.y, vert.z, 0));
    }

    List<Vector4> vertsAtOne = new();
    foreach (Vector3 vert in mesh1.vertices)
    {
          vertsAtZero.Append(new Vector4(vert.x, vert.y, vert.z, 0));
    }

    return ScriptableObject.CreateInstance<Mesh4D>();
}    
}