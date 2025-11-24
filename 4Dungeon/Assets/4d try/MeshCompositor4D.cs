using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Creates a 4D mesh by connecting two 3D meshes along the W axis.
/// </summary>
[System.Serializable]
[ExecuteInEditMode]
public class MeshCompositor4D : MonoBehaviour
{
    [SerializeField] private MeshRenderer4D m_MeshRenderer4D;
    [SerializeField] private Mesh m_Mesh0;
    [SerializeField] private Mesh m_Mesh1;
    [SerializeField] private bool m_ConvergeToPoint;
    [SerializeField] private bool m_UseCustomPointOfConvergence;
    [SerializeField] private Vector4 m_PointOfConvergence = new Vector4(0, 0, 0, 1);

    [SerializeField] private Transform4D m_TransformStart;
    [SerializeField] private Transform4D m_TransformEnd;

    // Cached transform values for change detection (works even when inspector is collapsed)
    private Euler4 m_CachedStartRotation;
    private Vector4 m_CachedStartPosition;
    private Vector4 m_CachedStartScale;
    private Euler4 m_CachedEndRotation;
    private Vector4 m_CachedEndPosition;
    private Vector4 m_CachedEndScale;

    // Deprecated: Old reference was to Transform4D, now we reference MeshRenderer4D directly.
    // Kept for migration from old scenes.
    [SerializeField, HideInInspector]
    [FormerlySerializedAs("m_transform")]
    private Transform4D m_DeprecatedTransform;

    /// <summary>
    /// The MeshRenderer4D to assign the composite mesh to.
    /// </summary>
    public MeshRenderer4D meshRenderer4D
    {
        get => m_MeshRenderer4D;
        set => m_MeshRenderer4D = value;
    }

    private void OnValidate()
    {
        MigrateFromDeprecatedTransform();
        EnsureTransforms();
    }

    private void Start()
    {
        MigrateFromDeprecatedTransform();
        EnsureTransforms();
        CacheTransformValues();
        CompositeMesh();
    }

    private void Update()
    {
        if (DetectTransformChanges())
        {
            CompositeMesh();
        }
    }

    /// <summary>
    /// Detects if the Transform4D components have changed since last frame.
    /// This allows recompositing even when the inspector is collapsed.
    /// </summary>
    private bool DetectTransformChanges()
    {
        bool changed = false;

        // Check start transform values
        if (m_TransformStart != null)
        {
            if (!m_CachedStartRotation.Equals(m_TransformStart.Rotation) ||
                m_CachedStartPosition != m_TransformStart.Position ||
                m_CachedStartScale != m_TransformStart.Scale)
            {
                changed = true;
            }
        }

        // Check end transform values
        if (m_TransformEnd != null && !m_ConvergeToPoint)
        {
            if (!m_CachedEndRotation.Equals(m_TransformEnd.Rotation) ||
                m_CachedEndPosition != m_TransformEnd.Position ||
                m_CachedEndScale != m_TransformEnd.Scale)
            {
                changed = true;
            }
        }

        if (changed)
        {
            CacheTransformValues();
        }

        return changed;
    }

    /// <summary>
    /// Caches current transform values for change detection.
    /// </summary>
    private void CacheTransformValues()
    {
        if (m_TransformStart != null)
        {
            m_CachedStartRotation = m_TransformStart.Rotation;
            m_CachedStartPosition = m_TransformStart.Position;
            m_CachedStartScale = m_TransformStart.Scale;
        }

        if (m_TransformEnd != null)
        {
            m_CachedEndRotation = m_TransformEnd.Rotation;
            m_CachedEndPosition = m_TransformEnd.Position;
            m_CachedEndScale = m_TransformEnd.Scale;
        }
    }

    /// <summary>
    /// Adds Transform4D components to this GameObject for m_TransformStart and m_TransformEnd if meshes are assigned but transforms are null.
    /// </summary>
    private void EnsureTransforms()
    {
        if (m_Mesh0 != null && m_TransformStart == null)
        {
            m_TransformStart = gameObject.AddComponent<Transform4D>();
            m_TransformStart.label = "Start Transform";
            Debug.Log($"Added Transform4D component (Start) to '{gameObject.name}'");
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.EditorUtility.SetDirty(m_TransformStart);
#endif
        }

        if (m_Mesh1 != null && m_TransformEnd == null && !m_ConvergeToPoint)
        {
            m_TransformEnd = gameObject.AddComponent<Transform4D>();
            m_TransformEnd.label = "End Transform";
            Debug.Log($"Added Transform4D component (End) to '{gameObject.name}'");
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.EditorUtility.SetDirty(m_TransformEnd);
#endif
        }
    }

    /// <summary>
    /// Migrates from old m_transform (Transform4D) reference to m_MeshRenderer4D.
    /// This runs on validate and start to handle both editor and runtime migration.
    /// </summary>
    private void MigrateFromDeprecatedTransform()
    {
        if (m_DeprecatedTransform != null && m_MeshRenderer4D == null)
        {
            // Try to find the MeshRenderer4D on the same object as the old Transform4D reference
            m_MeshRenderer4D = m_DeprecatedTransform.GetComponent<MeshRenderer4D>();
            if (m_MeshRenderer4D == null)
            {
                // Fallback: look on this object
                m_MeshRenderer4D = GetComponent<MeshRenderer4D>();
            }
            m_DeprecatedTransform = null;
            Debug.Log($"Migrated MeshCompositor4D reference from Transform4D to MeshRenderer4D on '{gameObject.name}'");
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }

    /// <summary>
    /// Composites mesh0 and mesh1 into a 4D mesh and assigns it to the MeshRenderer4D component.
    /// </summary>
    public void CompositeMesh()
    {
        if (m_MeshRenderer4D == null)
        {
            m_MeshRenderer4D = gameObject.GetComponent<MeshRenderer4D>();
            if (m_MeshRenderer4D == null)
            {
                m_MeshRenderer4D = gameObject.AddComponent<MeshRenderer4D>();
            }
        }

        if (!m_ConvergeToPoint && m_Mesh0 != null && m_Mesh1 != null)
        {
            m_MeshRenderer4D.mesh4D = CompositeMeshes(m_Mesh0, m_Mesh1, m_TransformStart, m_TransformEnd);
        }
        else if (m_ConvergeToPoint && m_Mesh0 != null)
        {
            if (m_UseCustomPointOfConvergence)
            {
                m_MeshRenderer4D.mesh4D = CompositeMeshToPoint(m_Mesh0, m_PointOfConvergence, m_TransformStart);
            }
            else
            {
                m_MeshRenderer4D.mesh4D = CompositeMeshToPoint(m_Mesh0, null, m_TransformStart);
            }
        }
    }

    /// <summary>
    /// Connect the given 3D mesh to a point at a given convergence point, or the (mesh centerpoint, 1) if no convergence point is present.
    /// </summary>
    /// <param name="mesh0"></param>
    /// <param name="convergencePointOpt"></param>
    /// <param name="transformStart"></param>
    /// <returns></returns>
    public static Mesh4D CompositeMeshToPoint(Mesh mesh0, Vector4? convergencePointOpt, Transform4D transformStart = null)
    {
        Vector4 cP;
        List<Vector3> m0v = new();
        mesh0.GetVertices(m0v);

        Vector3 avg = new();
        List<Vector4> allVerts = new();
        foreach (Vector3 vert in m0v)
        {
            avg += vert;
            Vector4 vert4 = new Vector4(vert.x, vert.y, vert.z, 0);
            if (transformStart != null)
            {
                vert4 = transformStart.Transform(vert4);
            }
            allVerts.Add(vert4);
        }
        avg /= m0v.Count;

        if (!convergencePointOpt.HasValue)
        {
            cP = new Vector4(avg.x, avg.y, avg.z, 1);
        }
        else
        {
            cP = convergencePointOpt.Value;
        }

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

    public static Mesh4D CompositeMeshes(Mesh mesh0, Mesh mesh1, Transform4D transformStart = null, Transform4D transformEnd = null)
    {
        Assert.AreEqual(mesh0.vertices.Length, mesh1.vertices.Length);

        List<Vector3> m0v = new();
        mesh0.GetVertices(m0v);
        List<Vector3> m1v = new();
        mesh1.GetVertices(m1v);

        List<Vector4> allVerts = new();
        foreach (Vector3 vert in m0v)
        {
            Vector4 vert4 = new(vert.x, vert.y, vert.z, 0);
            if (transformStart != null)
            {
                vert4 = transformStart.Transform(vert4);
            }
            allVerts.Add(vert4);
        }

        foreach (Vector3 vert in m1v)
        {
            Vector4 vert4 = new(vert.x, vert.y, vert.z, 0);
            if (transformEnd != null)
            {
                vert4 = transformEnd.Transform(vert4);
            }
            allVerts.Add(vert4);
        }

        Edge[] edges0 = GetMeshEdges(mesh0);
        Edge[] edges1 = GetMeshEdges(mesh1, m0v.Count);
        int num_edges = edges0.Length;
        Edge[] allEdges = new Edge[num_edges * 2 + m0v.Count];
        edges0.CopyTo(allEdges, 0);
        edges1.CopyTo(allEdges, num_edges);
        for (int i = 0; i < m0v.Count; i++)
        {
            allEdges[2 * num_edges + i] = new Edge(i, m0v.Count + i);
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
