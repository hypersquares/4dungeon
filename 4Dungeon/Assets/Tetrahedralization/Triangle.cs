using UnityEngine;

public class Triangle
{
    public Vector4[] vertices;
    public Triangle(Vector4 i, Vector4 j, Vector4 k)
    {
        vertices = new Vector4[]{i, j, k};
    }

    /// <summary>
    /// Make facing front if true and back if false.
    /// </summary>
    /// <param name="front"></param>
    public void MakeFacing(bool front)
    {
        int sign = front ? 1 : -1;
        if(Mathf.Sign(Vector3.Cross(vertices[1] - vertices[0], vertices[0] - vertices[2]).magnitude) != sign)
        {
            vertices = new Vector4[]{vertices[0], vertices[2], vertices[1]};
        }
    }
}
