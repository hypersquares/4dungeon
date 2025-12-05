using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.ProBuilder;

public class TetrahedralMesh
{
    public Tetrahedron[] tetrs;
    public Transform transform;

    public TetrahedralMesh(Tetrahedron[] tetrs)
    {
        this.tetrs = tetrs;
    }
    public TetrahedralMesh(Tetrahedron[] tetrs, Transform4D transform)
    {
        this.tetrs = tetrs;
        this.ApplyTransform(transform);
    }
    // public TetrahedralMesh(Mesh m)
    // {
    //     tetrs = Tetrahedralize(m);
    // }

    public Mesh Slice(Plane4D p, Camera cam)
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
        List<Vector3> norms = new();
        List<Color> colors = new();
        Func<Vector4, Color> VertexToColor = v => Color.Lerp(Color.white, Color.black, (v[3] + 2) / 4);
        foreach (Tetrahedron tet in tetrs)
        {
            Triangle[] t = tet.Intersect(p, cam);
            for (int i = 0; i < t.Length; i++)
            {
                verts.Add(t[i].vertices[0]);
                colors.Add(VertexToColor(t[i].vertices[0]));
                verts.Add(t[i].vertices[1]);
                colors.Add(VertexToColor(t[i].vertices[1]));                
                verts.Add(t[i].vertices[2]);
                colors.Add(VertexToColor(t[i].vertices[2]));

                tri_inds.Add(tri_inds.Count);
                tri_inds.Add(tri_inds.Count);
                tri_inds.Add(tri_inds.Count);
                norms.Add(t[i].normal);
                norms.Add(t[i].normal);
                norms.Add(t[i].normal);
            }
        }

        Mesh outputMesh = new Mesh();
        outputMesh.SetVertices(verts);
        outputMesh.SetTriangles(tri_inds.ToArray(), 0);
        outputMesh.SetNormals(norms);
        outputMesh.SetColors(colors);
        // outputMesh.RecalculateNormals();
        outputMesh.RecalculateTangents();
        // DebugCompareNorms(norms, outputMesh.normals);
        return outputMesh;
    }

    private void DebugCompareNorms(List<Vector3> calc_norms, Vector3[] unity_norms)
    {
        bool same = true;
        for (int i = 0; i < unity_norms.Length; i++)
        {
            if (calc_norms[i] != unity_norms[i]) {
                Debug.LogWarning($"Yours: {calc_norms[i]}, Unity: {unity_norms[i]}");
                same = false;
            }
        }
        Debug.Log($"Were your normals the same as unity's? {same}");
    }
    
    /// <summary>
    /// Applies the given Transform4D to the tetrahedra of this mesh.
    /// </summary>
    /// <param name="transform"></param>
    public void ApplyTransform(Transform4D transform)
    {
        foreach (var tetr in tetrs)
        {
            tetr.ApplyTransform(transform);
        }
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
