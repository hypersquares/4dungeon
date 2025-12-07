using UnityEngine;

namespace Assets.Tetrahedralization.Old
{
public class Simplex
{
    static Vector4[] verts = new Vector4[]{
        new(0f,0f,0f,2), 
        new(-1.11803f,1.11803f,1.11803f,-0.5f),
        new(1.11803f,-1.11803f,1.11803f,-0.5f),
        new(1.11803f,1.11803f,-1.11803f,-0.5f),
        new(-1.11803f,-1.11803f,-1.11803f,-0.5f)
    };

    public static Tetrahedron Cell1 => new(new Vector4[4]{verts[0], verts[1], verts[2], verts[3]});
    public static Tetrahedron Cell2 => new(new Vector4[4]{verts[0], verts[2], verts[3], verts[4]});
    public static Tetrahedron Cell3 => new(new Vector4[4]{verts[0], verts[1], verts[3], verts[4]});
    public static Tetrahedron Cell4 => new(new Vector4[4]{verts[0], verts[1], verts[2], verts[4]});
    public static Tetrahedron Cell5 => new(new Vector4[4]{verts[1], verts[2], verts[3], verts[4]});

    public static Tetrahedron[] GetTetrahedra()
    {
        return new Tetrahedron[]{Cell1, Cell2, Cell3, Cell4, Cell5};
    }


}
    
}
