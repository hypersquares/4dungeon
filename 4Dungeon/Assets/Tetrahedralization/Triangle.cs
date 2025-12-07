using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

/// <summary>
/// A triangle. Note that while the vertices are stored as Vector4s, they should be thought of as Vector3s. 
/// we preserve the w-coordinate of the vertices in our representation so they can be used later in the pipeline
/// </summary>
public class Triangle
{
    public Vector4[] vertices;
    public Vector3 normal;
    public Camera cam;
    public Triangle(Vector4 i, Vector4 j, Vector4 k, Camera camera, bool front = true)
    {
        vertices = new Vector4[]{i, j, k};
        this.cam = camera;
        // this.FOV = camera.fieldOfView
        this.normal = CorrectFacing(vertices, camera);
    }


    /// Current issues:
    ///     Vertices are in local space, but are being treated as if in world space.
    ///     Not accounting for distance of camera plane. What even is camera_pos????
    ///     
    /// <summary>
    /// Make facing front if true and back if false. Returns computed normal
    /// </summary>
    /// <param name="front"></param>
    /// <returns>computed normal vector</returns>
    private Vector3 CorrectFacing(Vector4[] verts, Camera cam)
    {
        Vector3 normal = Vector3.Cross(verts[1] - verts[0], verts[2] - verts[0]).normalized;
        if (Vector3.Dot(normal, (Vector3) verts[0] - cam.transform.position) >= 0f)
        {
            this.vertices = new Vector4[]{verts[0], verts[2], verts[1]};
            normal = -1 * normal;
        }
        return normal;
    }
}
