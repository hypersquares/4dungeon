using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Creates a mesh connecting two 
/// </summary>
[System.Serializable]
[ExecuteInEditMode]
public class MeshCompositor4D : MonoBehaviour
{
    [SerializeField] private Transform4D m_transform;
    [SerializeField] private Mesh m_mesh0;
    [SerializeField] private Mesh m_mesh1;
    [SerializeField] private bool m_ConvergeToPoint;
    [SerializeField] private bool m_UseCustomPointOfConvergence;
    [SerializeField] private Vector4 m_PointOfConvergence = new Vector4 (0, 0, 0, 1);

    [SerializeField] private Transform4D m_TransformEnd; //unused currently

    void Start()
    {
        CompositeMesh();
    }

    /// <summary>
    /// Composites mesh0 and mesh1 into a 4D mesh and assigns it to the Transform4D component.
    /// </summary>
    public void CompositeMesh()
    {
        if (m_transform == null)
        {
            var trans = gameObject.GetComponent<Transform4D>();
            if (trans == null)
            {
                trans = gameObject.AddComponent<Transform4D>();
            }
            this.m_transform = trans;
        }
        if (!m_ConvergeToPoint && m_mesh0 != null && m_mesh1 != null)
        {
            m_transform.mesh4D = CompositeMeshes(m_mesh0, m_mesh1);
        } else if (m_ConvergeToPoint && m_mesh0 != null)
        {
            if (m_UseCustomPointOfConvergence) m_transform.mesh4D = CompositeMeshToPoint(m_mesh0, m_PointOfConvergence);
            else
            {
                Assert.NotNull(m_PointOfConvergence);
                Debug.Log("gothere");
                m_transform.mesh4D = CompositeMeshToPoint(m_mesh0, null);
            } 
            }
    } 

    public static Mesh4D CompositeMeshToPoint(Mesh mesh0, Vector4? convergencePointOpt)
    {
        Vector4 cP;
        List<Vector3> m0v = new();
        mesh0.GetVertices(m0v);
        //Average verts to find centerpoint of object (??)
        Vector3 avg = new();
        List<Vector4> allVerts = new();
        foreach (Vector3 vert in m0v)
        {
                avg += vert;
                allVerts.Add(new Vector4(vert.x, vert.y, vert.z, 0));
        }
        avg /= m0v.Count;
        if (!convergencePointOpt.HasValue) cP = new Vector4(avg.x, avg.y, avg.z, 1);
        else cP = convergencePointOpt.Value;
        
        allVerts.Add(new Vector4(cP.x, cP.y, cP.z, cP.w));
        Edge[] edges0 = GetMeshEdges(mesh0);
        Edge[] edgesToPoint = new Edge[edges0.Length + mesh0.vertices.Length];
        for (int i = 0; i < mesh0.vertices.Length; i++)
        {
            edgesToPoint[edges0.Length + i] = new Edge(i, mesh0.vertices.Length);
        }
        Mesh4D m4D = ScriptableObject.CreateInstance<Mesh4D>();
        m4D.Vertices = allVerts.ToArray();
        m4D.Edges = edgesToPoint;
        return m4D;

        
    }
    public static Mesh4D CompositeMeshes(Mesh mesh0, Mesh mesh1)
{
    Assert.AreEqual(mesh0.vertices.Length, mesh1.vertices.Length);
    List<Vector3> m0v = new();
    mesh0.GetVertices(m0v);
    Debug.Log(m0v.ToCommaSeparatedString());
    List<Vector3> m1v = new();
    mesh1.GetVertices(m1v);
    List<Vector4> allVerts = new();
    foreach (Vector3 vert in m0v)
    {
        allVerts.Add(new Vector4(vert.x, vert.y, vert.z, 0));
    }

    foreach (Vector3 vert in m1v)
    {
        allVerts.Add(new Vector4(vert.x, vert.y, vert.z, 1));
    }

    Edge[] edges0 = GetMeshEdges(mesh0);
    Edge[] edges1 = GetMeshEdges(mesh1, m0v.Count);
    int num_edges = edges0.Length;
    Edge[] allEdges = new Edge[num_edges * 2 + m0v.Count];
    edges0.CopyTo(allEdges, 0);
    edges1.CopyTo(allEdges, num_edges);
    for (int i = 0; i < m0v.Count; i++)
    {
            allEdges[2*num_edges + i] = new Edge(i, m0v.Count + i);
    }
    Mesh4D m4D = ScriptableObject.CreateInstance<Mesh4D>();
    m4D.Vertices = allVerts.ToArray();
    m4D.Edges = allEdges;
    return m4D;
}
    public static Edge[] GetMeshEdges(Mesh m, int start_idx = 0)
    {
        List<int> triangles = new();
        if (m.subMeshCount != 1)
        {
            Debug.LogWarning("When getting triangles, only accessed submesh 0. You have multiple submeshes, so you should probably check that the output looks correct.");
        }
        m.GetTriangles(triangles, 0);
        HashSet<Edge> edges = new();
        for (int i = 0; i < triangles.Count - 3; i += 3)
        {
            int v0 = triangles[i] + start_idx;
            int v1 = triangles[i + 1] + start_idx;
            int v2 = triangles[i + 2] + start_idx;
            edges.Add(new Edge(v0, v1));
            edges.Add(new Edge(v1, v2));
            edges.Add(new Edge(v2, v0));
        }
        return edges.ToArray();
    } 
}