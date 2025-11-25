using System.Collections.Generic;
using UnityEngine;

public class TetrahedralMesh
{
    public Tetrahedron[] tetrs;
    public TetrahedralMesh(Tetrahedron[] tetrs)
    {
        this.tetrs = tetrs;
    }
    // public TetrahedralMesh(Mesh m)
    // {
    //     tetrs = Tetrahedralize(m);
    // }

    public Mesh Slice(Plane4D p, SlicingWorldState world)
    {
        /**Outline.... 
            - Calculate the intersections for each tetrahedron with the plane
                - This has presumably already been implemented. 
                - Output: 0, 3 or 4 points?
            - For each tetrahedron, you can connect these points to either 0, 1, or 2 triangles. 
            - Boom triangle mesh
        */
        List<Vector3> verts = new();
        List<int> tri_inds = new();
        foreach (Tetrahedron tet in tetrs)
        {
            Triangle[] t = tet.Intersect(p, world);
            for (int i = 0; i < t.Length; i++)
            {
                verts.Add(t[i].vertices[0]);
                verts.Add(t[i].vertices[1]);
                verts.Add(t[i].vertices[2]);

                tri_inds.Add(tri_inds.Count);
                tri_inds.Add(tri_inds.Count);
                tri_inds.Add(tri_inds.Count);
            }
        }

        Mesh outputMesh = new Mesh();
        outputMesh.SetVertices(verts);
        outputMesh.SetTriangles(tri_inds.ToArray(), 0);
        outputMesh.RecalculateNormals();
        return outputMesh;
    }
    // /// <summary>
    // /// Expect m to be a cell of a regular polytop with faces divided into triangles:::::: super change this TODO extreme
    // /// </summary>
    // /// <param name="m"></param>
    // /// <returns></returns>
    // private Tetrahedron[] Tetrahedralize(Mesh4D m)
    // {
    //     Vector4[] verts = m.Vertices;
    //     int[] triangles = m.triangles;
    //     int start = 0;
    //     List<Tetrahedron> tetrz = new();
    //     for (int i = 0; i < m.triangles.Length-3; i+=3)
    //     {
    //         if (start == triangles[i] || start == triangles[i+1] || start == triangles[i+2]) continue;
    //         tetrz.Add(new Tetrahedron(verts[0], new Triangle(verts[triangles[i]], verts[triangles[i+1]], verts[triangles[i+2]])));
    //     }
    //     return tetrz.ToArray();
    // }

}
