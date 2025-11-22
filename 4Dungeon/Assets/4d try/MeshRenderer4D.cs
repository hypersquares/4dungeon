using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class MeshRenderer4D : MonoBehaviour
{
    public Transform4D transform4d;

    [Header("Hyperplane (for slicing)")]
    public Vector4 planePoint;
    public Vector4 planeNorm = new Vector4(0, 0, 0, 1);

    [Header("Mesh")]
    public MeshFilter meshFilter;
    private Mesh mesh3;

    private const float EPS = 1e-6f;
    public bool debug3d = false;
    public bool debug4d = false;

    void Start()
    {
        if (transform4d == null)
        {
            var trans = gameObject.GetComponent<Transform4D>();
            if (trans != null) this.transform4d = trans;
        }
        if (meshFilter == null)
        {
            var filter = gameObject.GetComponent<MeshFilter>();
            if (filter != null) this.meshFilter = filter;
        }
    }

    private void Update()
    {
        if (transform4d.mesh4D != null)
        {
            Intersect();
        }
    }

    public void Intersect()
    {
        Mesh4D mesh4D = transform4d.mesh4D;
        Vector4 norm = planeNorm.normalized;
        List<Vector4> vertices = new List<Vector4>();

        for (int i = 0; i < mesh4D.Edges.Length; i++)
        {
            Edge edge = mesh4D.Edges[i];
            Vector4 v0 = transform4d.vertices[edge.Index0];
            Vector4 v1 = transform4d.vertices[edge.Index1];
            Intersection(vertices, v0, v1);
        }

        // Remove duplicate vertices
        for (int i = 0; i < vertices.Count; i++)
            for (int j = vertices.Count - 1; j > i; j--)
                if ((vertices[i] - vertices[j]).sqrMagnitude < EPS)
                    vertices.RemoveAt(j);

        if (vertices.Count > 3)
        {
            // Normal case: convex polygon with more than 3 vertices
            mesh3 = ConvexHullWrapper.ConstructMesh(vertices, transform4d);
            meshFilter.mesh = mesh3;
        } else if (vertices.Count == 3)
        {
            // Degenerate case: triangle
            mesh3 = new Mesh();
            for (int i = 0; i < 3; i++)
            {
                mesh3.vertices[i] = transform4d.ProjectTo3D(vertices[i]);
            }
            mesh3.triangles = new int[] { 0, 1, 2 };
            mesh3.RecalculateNormals();
            meshFilter.mesh = mesh3;
        } else
        {
            Debug.Log("Less than 3 intersection points");
            mesh3 = null;
            meshFilter.mesh = null;
        }

        if (debug3d)
        {
            for (int i = 0; i < mesh3.triangles.Length; i += 3)
            {
                Vector3 v0 = mesh3.vertices[mesh3.triangles[i]];
                Vector3 v1 = mesh3.vertices[mesh3.triangles[i + 1]];
                Vector3 v2 = mesh3.vertices[mesh3.triangles[i + 2]];
                Debug.DrawLine(v0, v1, Color.red);
                Debug.DrawLine(v1, v2, Color.red);
                Debug.DrawLine(v2, v0, Color.red);
            }
        }

        if (debug4d)
        {
            for (int i = 0; i < mesh4D.Edges.Length; i++)
            {
                Edge edge = mesh4D.Edges[i];
                Vector4 v0 = transform4d.vertices[edge.Index0];
                Vector4 v1 = transform4d.vertices[edge.Index1];
                Debug.DrawLine(transform4d.ProjectTo3D(v0), transform4d.ProjectTo3D(v1), Color.green);
            }
        }
    }

    // Returns number of intersection points
    private int Intersection(List<Vector4> vertices, Vector4 v0, Vector4 v1)
    {
        float d0 = Vector4.Dot(planeNorm.normalized, v0 - planePoint);
        float d1 = Vector4.Dot(planeNorm.normalized, v1 - planePoint);

        // Both points on the same side of the plane
        if (d0 * d1 > 0)
        {
            return 0;
        }

        // Both points on the plane
        if (Mathf.Abs(d0) < EPS && Mathf.Abs(d1) < EPS)
        {
            vertices.Add(v0);
            vertices.Add(v1);
            return 2;
        }

        // One point on the plane
        if (Mathf.Abs(d0) < EPS)
        {
            vertices.Add(v0);
            return 1;
        }

        if (Mathf.Abs(d1) < EPS)
        {
            vertices.Add(v1);
            return 1;
        }

        // One intersection
        float t = d0 / (d0 - d1);
        Vector4 x = v0 + (v1 - v0) * t;
        vertices.Add(x);
        return 1;
    }
}
