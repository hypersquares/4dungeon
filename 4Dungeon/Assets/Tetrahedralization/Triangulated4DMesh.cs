using UnityEngine;

public class Triangulated4DMesh
{
    public Vector4[] verts;
    public int[] triangles;

    public Triangulated4DMesh(Vector4[] verts, int[] triangles)
    {
        this.verts = verts;
        this.triangles = triangles;
    }
}
