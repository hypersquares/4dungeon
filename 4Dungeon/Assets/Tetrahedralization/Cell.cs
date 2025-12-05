using UnityEngine;

/** A cell of a 4D solid */
public class Cell
{
    public Vector4[] verts;
    public Edge[] edges;
    public Cell(Vector4[] verts, Edge[] edges)
    {
        this.verts = verts;
        this.edges = edges;
    }
}
