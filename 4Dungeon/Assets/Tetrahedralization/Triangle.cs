using Unity.VisualScripting;
using UnityEngine;

public class Triangle
{
    public Vector3[] vertices;
    public Vector3 normal;

    public Camera cam;
    public Triangle(Vector3 i, Vector3 j, Vector3 k, Camera camera, bool front = true)
    {
        vertices = new Vector3[]{i, j, k};
        this.cam = camera;
        // this.FOV = camera.fieldOfView
        this.normal = MakeFacing(vertices, camera, front);
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
    private Vector3 MakeFacing(Vector3[] verts, Camera cam, bool front)
    {
        Vector3 camera_fwd = cam.transform.forward;
        Vector3[] temp_verts = new Vector3[3];
        /** Adjust calculation for camera perspective... */
        float sign = front ? 1f : -1f;
        for (int i = 0; i < temp_verts.Length; i++)
        {
            // temp_verts[i] = mtx.MultiplyPoint3x4(verts[i]);
            temp_verts[i] = verts[i];
        }
        // Debug.Log(temp_verts.ToCommaSeparatedString());
        Vector3 normal = Vector3.Cross(temp_verts[1] - temp_verts[0], temp_verts[2] - temp_verts[0]).normalized;
        if(Vector3.Dot(normal, camera_fwd) * sign > 0)
        {
            this.vertices = new Vector3[]{verts[0], verts[2], verts[1]};
            normal = -1 * normal;
        }
        return normal;
    }
}
