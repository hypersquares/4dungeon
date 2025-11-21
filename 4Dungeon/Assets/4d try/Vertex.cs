using MIConvexHull;
using UnityEngine;

// 3D Vertex equivalent of a 3D Vector for MIConvexHull library
public class Vertex : IVertex
{
    public double[] Position { get; set; }

    public Vertex(Vector3 v)
    {
        Position = new double[3] { v.x, v.y, v.z };
    }

    public static implicit operator Vertex(Vector3 v)
    {
        return new Vertex(v);
    }

    public static implicit operator Vector3(Vertex v)
    {
        return new Vector3((float)v.Position[0], (float)v.Position[1], (float)v.Position[2]);
    }
}