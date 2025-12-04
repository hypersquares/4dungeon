using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using System;
using Mono.Cecil;
using System.Runtime.InteropServices.WindowsRuntime;
public class Tetrahedron
{
    private const float EPS = 1e-6f;
    public Edge[] edges = new Edge[]{
            new(0, 1), new(0, 2), new(0, 3), 
            new(1, 2), new(2, 3), new(3, 1)
    };
    public Vector4[] vertices;

    public Tetrahedron(Vector4[] vec)
    {
        vertices = vec; //TODO BUG?
    }

    public Tetrahedron(Vector4 i, Vector4 j, Vector4 k, Vector4 l)
    {
        vertices = new Vector4[]{i, j, k, l};
    }
    public Tetrahedron(Vector4 i, Triangle t)
    {
        vertices = new Vector4[]{i, t.vertices[0], t.vertices[1], t.vertices[2]};
    }

    public Triangle[] Intersect(Plane4D plane, Camera cam)
    {
        HashSet<Vector3> verts = new(); //TODO FLOATING POINT PRECISION EXTREME BUG???? AAAA
        int sum = 0;
        foreach (Edge e in edges)
        {
            sum += Intersection(vertices[e.Index0], vertices[e.Index1], plane, verts);
        }
        if (verts.Count == 1)  {
            Debug.LogWarning("we had an intersection output one vertex. just a heads up bro");
            return new Triangle[0];
        }
        Debug.Assert(verts.Count == 0 || verts.Count == 3 || verts.Count == 4);
        if (verts.Count == 0) return new Triangle[0];
        List<Vector3> l = verts.ToList();
        if (verts.Count == 3) {
            Triangle t = new(l[0], l[1], l[2], cam);
            return new Triangle[1]{t};
        }
        //TODO DOES THIS FIX REALLY WORK
        else {
            ReorderVerts(l);
            Triangle t1 = new(l[0], l[1], l[2], cam);
            Triangle t2 = new (l[1], l[2], l[3], cam);
            return new Triangle[2]{t1, t2};
        }

    }

    /// <summary>
    /// reorders the given array of four vertices such that 0, 1, 2 and 1, 2, 3 form non-intersecting triangles. 
    /// </summary>
    /// <param name="verts"></param>
    private void ReorderVerts(List<Vector3> verts)
    {
        var o1 = Vector3.Cross(verts[1] - verts[2], verts[1] - verts[0]);
        var o2 = Vector3.Cross(verts[1] - verts[2], verts[1] - verts[3]);
        if (o1.normalized == o2.normalized)
        {
            o1 = Vector3.Cross(verts[2] - verts[0], verts[2] - verts[1]);
            o2 = Vector3.Cross(verts[2] - verts[0], verts[2] -verts[3]);
            if (o1.normalized == o2.normalized)
            {
                //then e23 must be central. So swap 3 and 1. 
                (verts[1], verts[3]) = (verts[3], verts[1]);
            } else
            {
                //then e02 is central, so swap 0 and 1. 
                (verts[0], verts[1]) = (verts[1], verts[0]);
            }
        }
        // if e12 is already central, we are done. 

    }
     /// <summary>
    /// Computes intersection of a line segment with the hyperplane.
    /// Returns the number of intersection points added. TODO Untested?
    /// </summary>
    private int Intersection(Vector4 v0, Vector4 v1, Plane4D plane, HashSet<Vector3> out_verts)
    {
        float d0 = Vector4.Dot(plane.normal, v0 - plane.point);
        float d1 = Vector4.Dot(plane.normal, v1 - plane.point);
        // Both points on the same side of the plane
        if (d0 * d1 > 0)
        {
            return 0;
        }

        // Both points on the plane
        if (Mathf.Abs(d0) < EPS && Mathf.Abs(d1) < EPS)
        {
            out_verts.Add(v0);
            out_verts.Add(v1);
            return 2;
        }

        // One point on the plane
        if (Mathf.Abs(d0) < EPS)
        {
            out_verts.Add(v0);
            return 1;
        }

        if (Mathf.Abs(d1) < EPS)
        {
            out_verts.Add(v1);
            return 1;
        }

        // One intersection
        float t = d0 / (d0 - d1);
        Vector4 x = v0 + (v1 - v0) * t;
        out_verts.Add(x);
        return 1;
    }
    /// <summary>
    /// Applies the given affine 4D transform to the vertices of this tetrahedron.
    /// </summary>
    public void ApplyTransform(Transform4D transform)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = transform.Transform(vertices[i]);
        }
    }
}
