using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

    public Triangle[] Intersect(Plane4D plane, SlicingWorldState world)
    {
        HashSet<Vector3> verts = new(); //TODO FLOATING POINT PRECISION EXTREME BUG???? AAAA
        int sum = 0;
        foreach (Edge e in edges)
        {
            sum += Intersection(vertices[e.Index0], vertices[e.Index1], plane, verts);
        }
        if (verts.Count == 1) return new Triangle[0];
        Debug.Assert(verts.Count == 0 || verts.Count == 3 || verts.Count == 4);
        if (verts.Count == 0) return new Triangle[0];
        List<Vector3> l = verts.ToList();
        if (verts.Count == 3) {
            Triangle t = new(l[0], l[1], l[2], world.camera_fwd);
            t.MakeFacing(true);
            return new Triangle[1]{t};
        }
        //TODO DOES THIS FIX REALLY WORK
        else {
            Debug.Log("gothere");
            Triangle t1 = new(l[0], l[1], l[2], world.camera_fwd);
            Triangle t2 = new (l[2], l[3], l[1], world.camera_fwd);
            t1.MakeFacing(true);
            t2.MakeFacing(true);
            return new Triangle[2]{t1, t2};
        }

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
}
