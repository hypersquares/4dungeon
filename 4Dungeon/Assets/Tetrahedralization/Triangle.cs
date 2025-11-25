using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class Triangle
{
    public Vector3[] vertices;
    private Vector3 camera_fwd;
    public Triangle(Vector3 i, Vector3 j, Vector3 k, Vector3 camera_fwd)
    {
        vertices = new Vector3[]{i, j, k};
        this.camera_fwd = camera_fwd;
    }

    /// <summary>
    /// Make facing front if true and back if false.
    /// </summary>
    /// <param name="front"></param>
    public void MakeFacing(bool front)
    {
        float sign = front ? 1f : -1f;
        // Debug.Log("gothere makefacing");
        Vector3 normal = Vector3.Cross(vertices[1] - vertices[0], vertices[2] - vertices[0]).normalized;
        Debug.Log($"{normal}, camera:{camera_fwd}");
        Debug.Log(Mathf.Sign(Vector3.Dot(normal, camera_fwd)));
        if(Mathf.Sign(Vector3.Dot(normal, camera_fwd)) != sign)
        {
            Debug.Log($"{vertices[0]}, {vertices[1]}, {vertices[2]}");
            vertices = new Vector3[]{vertices[0], vertices[2], vertices[1]};
        }
    }
}
