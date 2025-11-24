using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Simplex
{
    static Vector4[] verts = new Vector4[]{
        new(0f,0f,0f,2), 
        new(-1.11803f,1.11803f,1.11803f,-0.5f),
        new(1.11803f,-1.11803f,1.11803f,-0.5f),
        new(1.11803f,1.11803f,-1.11803f,-0.5f),
        new(-1.11803f,-1.11803f,-1.11803f,-0.5f)
    };

    public static Tetrahedron cell1 {get => new Tetrahedron(new Vector4[4]{verts[0], verts[1], verts[2], verts[3]});}
    public static Tetrahedron cell2 {get => new Tetrahedron(new Vector4[4]{verts[0], verts[2], verts[3], verts[4]});}
    public static Tetrahedron cell3 {get => new Tetrahedron(new Vector4[4]{verts[0], verts[1], verts[3], verts[4]});}
    public static Tetrahedron cell4 {get => new Tetrahedron(new Vector4[4]{verts[0], verts[1], verts[2], verts[4]});}
    public static Tetrahedron cell5 {get => new Tetrahedron(new Vector4[4]{verts[1], verts[2], verts[3], verts[4]});}

    public static Tetrahedron[] GetTetrahedra()
    {
        return new Tetrahedron[]{cell1, cell2, cell3, cell4, cell5};
    }


}

