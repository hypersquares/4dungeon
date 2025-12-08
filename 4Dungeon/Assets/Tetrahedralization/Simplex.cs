using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Tetrahedralization
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

    public static Tetrahedron Cell1 => new(0, 1, 2, 3);
    public static Tetrahedron Cell2 => new(0, 2, 3, 4);
    public static Tetrahedron Cell3 => new(0, 1, 3, 4);
    public static Tetrahedron Cell4 => new(0, 1, 2, 4);
    public static Tetrahedron Cell5 => new(1, 2, 3, 4);

    public static TetrahedralMesh GetTetrahedralMesh()
    {
        return new TetrahedralMesh(verts, new Tetrahedron[]{Cell1, Cell2, Cell3, Cell4, Cell5});
    }


}
    
}
