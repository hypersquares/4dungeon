using UnityEngine;

namespace Assets.Tetrahedralization.Old
{

public class Tesseract
{
    static Vector4[] verts = new Vector4[]
    {
        new(-1,-1,-1,-1),
        new(-1,-1,-1, 1),
        new(-1,-1, 1,-1),
        new(-1,-1, 1, 1),
        new(-1, 1,-1,-1),
        new(-1, 1,-1, 1),
        new(-1, 1, 1,-1),
        new(-1, 1, 1, 1),
        new( 1,-1,-1,-1),
        new( 1,-1,-1, 1),
        new( 1,-1, 1,-1),
        new( 1,-1, 1, 1),
        new( 1, 1,-1,-1),
        new( 1, 1,-1, 1),
        new( 1, 1, 1,-1),
        new( 1, 1, 1, 1),
    };

    public static Tetrahedron Cell1  => new(new Vector4[]{ verts[0],  verts[1],  verts[3],  verts[7] });
    public static Tetrahedron Cell2  => new(new Vector4[]{ verts[0],  verts[3],  verts[2],  verts[7] });
    public static Tetrahedron Cell3  => new(new Vector4[]{ verts[0],  verts[2],  verts[6],  verts[7] });
    public static Tetrahedron Cell4  => new(new Vector4[]{ verts[0],  verts[6],  verts[4],  verts[7] });
    public static Tetrahedron Cell5  => new(new Vector4[]{ verts[0],  verts[4],  verts[5],  verts[7] });

    public static Tetrahedron Cell6  => new(new Vector4[]{ verts[8],  verts[9],  verts[11], verts[15] });
    public static Tetrahedron Cell7  => new(new Vector4[]{ verts[8],  verts[11], verts[10], verts[15] });
    public static Tetrahedron Cell8  => new(new Vector4[]{ verts[8],  verts[10], verts[14], verts[15] });
    public static Tetrahedron Cell9  => new(new Vector4[]{ verts[8],  verts[14], verts[12], verts[15] });
    public static Tetrahedron Cell10 => new(new Vector4[]{ verts[8],  verts[12], verts[13], verts[15] });

    public static Tetrahedron Cell11 => new(new Vector4[]{ verts[1],  verts[9],  verts[11], verts[13] });
    public static Tetrahedron Cell12 => new(new Vector4[]{ verts[1],  verts[11], verts[3],  verts[13] });
    public static Tetrahedron Cell13 => new(new Vector4[]{ verts[1],  verts[3],  verts[7],  verts[13] });
    public static Tetrahedron Cell14 => new(new Vector4[]{ verts[1],  verts[7],  verts[5],  verts[13] });
    public static Tetrahedron Cell15 => new(new Vector4[]{ verts[1],  verts[5],  verts[9],  verts[13] });

    public static Tetrahedron Cell16 => new(new Vector4[]{ verts[4],  verts[12], verts[13], verts[5] });
    public static Tetrahedron Cell17 => new(new Vector4[]{ verts[4],  verts[13], verts[15], verts[5] });
    public static Tetrahedron Cell18 => new(new Vector4[]{ verts[4],  verts[15], verts[14], verts[6] });
    public static Tetrahedron Cell19 => new(new Vector4[]{ verts[4],  verts[14], verts[12], verts[6] });
    public static Tetrahedron Cell20 => new(new Vector4[]{ verts[4],  verts[6],  verts[12], verts[2] });

    public static Tetrahedron Cell21 => new(new Vector4[]{ verts[2],  verts[10], verts[11], verts[3] });
    public static Tetrahedron Cell22 => new(new Vector4[]{ verts[2],  verts[11], verts[15], verts[3] });
    public static Tetrahedron Cell23 => new(new Vector4[]{ verts[2],  verts[15], verts[14], verts[6] });
    public static Tetrahedron Cell24 => new(new Vector4[]{ verts[2],  verts[14], verts[10], verts[6] });
    public static Tetrahedron Cell25 => new(new Vector4[]{ verts[2],  verts[6],  verts[10], verts[0] });

    public static Tetrahedron Cell26 => new(new Vector4[]{ verts[0],  verts[8],  verts[9],  verts[1] });
    public static Tetrahedron Cell27 => new(new Vector4[]{ verts[0],  verts[9],  verts[1],  verts[3] });
    public static Tetrahedron Cell28 => new(new Vector4[]{ verts[0],  verts[1],  verts[5],  verts[4] });
    public static Tetrahedron Cell29 => new(new Vector4[]{ verts[0],  verts[5],  verts[4],  verts[6] });
    public static Tetrahedron Cell30 => new(new Vector4[]{ verts[0],  verts[6],  verts[2],  verts[4] });

    public static Tetrahedron Cell31 => new(new Vector4[]{ verts[2],  verts[10], verts[14], verts[6] });
    public static Tetrahedron Cell32 => new(new Vector4[]{ verts[3],  verts[11], verts[15], verts[7] });
    public static Tetrahedron Cell33 => new(new Vector4[]{ verts[3],  verts[15], verts[11], verts[7] });
    public static Tetrahedron Cell34 => new(new Vector4[]{ verts[5],  verts[13], verts[15], verts[7] });
    public static Tetrahedron Cell35 => new(new Vector4[]{ verts[6],  verts[14], verts[15], verts[7] });

    public static Tetrahedron Cell36 => new(new Vector4[]{ verts[8],  verts[10], verts[11], verts[15] });
    public static Tetrahedron Cell37 => new(new Vector4[]{ verts[9],  verts[11], verts[3],  verts[1] });
    public static Tetrahedron Cell38 => new(new Vector4[]{ verts[12], verts[13], verts[5],  verts[4] });
    public static Tetrahedron Cell39 => new(new Vector4[]{ verts[14], verts[6],  verts[2],  verts[10] });
    public static Tetrahedron Cell40 => new(new Vector4[]{ verts[4],  verts[12], verts[14], verts[6] });

    public static Tetrahedron[] GetTetrahedra()
    {
        return new Tetrahedron[]
        {
            Cell1,  Cell2,  Cell3,  Cell4,  Cell5,
            Cell6,  Cell7,  Cell8,  Cell9,  Cell10,
            Cell11, Cell12, Cell13, Cell14, Cell15,
            Cell16, Cell17, Cell18, Cell19, Cell20,
            Cell21, Cell22, Cell23, Cell24, Cell25,
            Cell26, Cell27, Cell28, Cell29, Cell30,
            Cell31, Cell32, Cell33, Cell34, Cell35,
            Cell36, Cell37, Cell38, Cell39, Cell40
        };
    }
}
}
