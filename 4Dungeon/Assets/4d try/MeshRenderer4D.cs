using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

/// <summary>
/// Renders a 4D mesh by slicing it with a hyperplane and displaying the 3D cross-section.
/// Owns the Mesh4D data and applies transforms from Transform4D before slicing.
/// </summary>
[ExecuteInEditMode]
public class MeshRenderer4D : MonoBehaviour
{
    [Header("4D Mesh")]
    [SerializeField] private Mesh4D m_Mesh4D;

    [Header("Transform")]
    [SerializeField] private Transform4D m_Transform4D;

    [Header("Output Mesh")]
    [SerializeField] private MeshFilter m_MeshFilter;
    private Mesh m_Mesh3;

    [Header("Debug")]
    [SerializeField] private bool m_Debug3D = false;
    [SerializeField] private bool m_Debug4D = false;

    private const float EPS = 1e-6f;

    /// <summary>
    /// The 4D mesh to render.
    /// </summary>
    public Mesh4D mesh4D
    {
        get => m_Mesh4D;
        set => m_Mesh4D = value;
    }

    /// <summary>
    /// The Transform4D component used to transform vertices before slicing.
    /// </summary>
    public Transform4D transform4D
    {
        get => m_Transform4D;
        set => m_Transform4D = value;
    }

    /// <summary>
    /// The MeshFilter to output the sliced 3D mesh to.
    /// </summary>
    public MeshFilter meshFilter
    {
        get => m_MeshFilter;
        set => m_MeshFilter = value;
    }

    /// <summary>
    /// Debug flag for drawing 3D slice edges.
    /// </summary>
    public bool debug3d
    {
        get => m_Debug3D;
        set => m_Debug3D = value;
    }

    /// <summary>
    /// Debug flag for drawing 4D mesh edges.
    /// </summary>
    public bool debug4d
    {
        get => m_Debug4D;
        set => m_Debug4D = value;
    }

    private void OnValidate()
    {
        MigrateFromTransform4D();
    }

    private void Start()
    {
        MigrateFromTransform4D();

        if (m_Transform4D == null)
        {
            m_Transform4D = gameObject.GetComponent<Transform4D>();
            if (m_Transform4D == null)
            {
                m_Transform4D = gameObject.AddComponent<Transform4D>();
            }
        }
        if (m_MeshFilter == null)
        {
            m_MeshFilter = gameObject.GetComponent<MeshFilter>();
        }
    }

    /// <summary>
    /// Migrates mesh4D data from Transform4D (old location) to MeshRenderer4D (new location).
    /// This runs on validate and start to handle both editor and runtime migration.
    /// </summary>
    private void MigrateFromTransform4D()
    {
        if (m_Mesh4D == null && m_Transform4D != null && m_Transform4D.deprecatedMesh4D != null)
        {
            m_Mesh4D = m_Transform4D.deprecatedMesh4D;
            m_Transform4D.deprecatedMesh4D = null;
            Debug.Log($"Migrated Mesh4D from Transform4D to MeshRenderer4D on '{gameObject.name}'");
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.EditorUtility.SetDirty(m_Transform4D);
#endif
        }
        Intersect();
    }

    //private void OnEnable()
    //{
    //    GameManager.OnWChanged += (_) => Intersect();
    //}

    //private void OnDisable()
    //{
    //    GameManager.OnWChanged -= (_) => Intersect();
    //}

    void Update()
    {
        if (m_Mesh4D != null)
        {
            Intersect();
        }

    }

    /// <summary>
    /// Sets the 4D mesh to render.
    /// </summary>
    public void SetMesh(Mesh4D m)
    {
        m_Mesh4D = m;
    }

    /// <summary>
    /// Performs the hyperplane intersection and generates the 3D output mesh.
    /// </summary>
    public void Intersect()
    {
        if (m_Mesh4D == null || m_Mesh4D.Vertices == null || m_Mesh4D.Edges == null)
        {
            return;
        }

        // Transform vertices using Transform4D (if available)
        Vector4[] transformedVertices;
        if (m_Transform4D != null)
        {
            transformedVertices = m_Transform4D.Transform(m_Mesh4D.Vertices);
        }
        else
        {
            transformedVertices = m_Mesh4D.Vertices;
        }

        List<Vector4> intersectionVertices = new List<Vector4>();

        for (int i = 0; i < m_Mesh4D.Edges.Length; i++)
        {
            Edge edge = m_Mesh4D.Edges[i];
            Vector4 v0 = transformedVertices[edge.Index0];
            Vector4 v1 = transformedVertices[edge.Index1];
            Intersection(intersectionVertices, v0, v1);
        }

        // Remove duplicate vertices
        for (int i = 0; i < intersectionVertices.Count; i++)
        {
            for (int j = intersectionVertices.Count - 1; j > i; j--)
            {
                if ((intersectionVertices[i] - intersectionVertices[j]).sqrMagnitude < EPS)
                {
                    intersectionVertices.RemoveAt(j);
                }
            }
        }

        if (intersectionVertices.Count > 3)
        {
            // Normal case: convex polygon with more than 3 vertices
            m_Mesh3 = ConvexHullWrapper.ConstructMesh(intersectionVertices, m_Transform4D);
            m_MeshFilter.mesh = m_Mesh3;
        }
        else if (intersectionVertices.Count == 3)
        {
            // Degenerate case: triangle
            m_Mesh3 = new Mesh();
            Vector3[] verts = new Vector3[3];
            for (int i = 0; i < 3; i++)
            {
                verts[i] = ProjectTo3D(intersectionVertices[i]);
            }
            m_Mesh3.vertices = verts;
            m_Mesh3.triangles = new int[] { 0, 1, 2 };
            m_Mesh3.RecalculateNormals();
            m_MeshFilter.mesh = m_Mesh3;
        }
        else
        {
            Debug.Log("Less than 3 intersection points");
            m_Mesh3 = null;
            m_MeshFilter.mesh = null;
        }

        // TODO: Maybe move this to a separate file
        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = m_Mesh3;
        meshCollider.convex = true;


        if (m_Debug3D && m_Mesh3 != null)
        {
            for (int i = 0; i < m_Mesh3.triangles.Length; i += 3)
            {
                Vector3 v0 = m_Mesh3.vertices[m_Mesh3.triangles[i]];
                Vector3 v1 = m_Mesh3.vertices[m_Mesh3.triangles[i + 1]];
                Vector3 v2 = m_Mesh3.vertices[m_Mesh3.triangles[i + 2]];
                Debug.DrawLine(v0, v1, Color.red);
                Debug.DrawLine(v1, v2, Color.red);
                Debug.DrawLine(v2, v0, Color.red);
            }
        }

        if (m_Debug4D)
        {
            for (int i = 0; i < m_Mesh4D.Edges.Length; i++)
            {
                Edge edge = m_Mesh4D.Edges[i];
                Vector4 v0 = transformedVertices[edge.Index0];
                Vector4 v1 = transformedVertices[edge.Index1];
                Debug.DrawLine(ProjectTo3D(v0), ProjectTo3D(v1), Color.green);
            }
        }
    }

    /// <summary>
    /// Projects a 4D point to 3D by dropping the W component.
    /// </summary>
    private Vector3 ProjectTo3D(Vector4 v)
    {
        return new Vector3(v.x, v.y, v.z);
    }

    /// <summary>
    /// Computes intersection of a line segment with the hyperplane.
    /// Returns the number of intersection points added.
    /// </summary>
    private int Intersection(List<Vector4> vertices, Vector4 v0, Vector4 v1)
    {
        float d0 = Vector4.Dot(GameManager.Instance.SlicingPlaneNormal, v0 - GameManager.Instance.SlicingPlanePoint);
        float d1 = Vector4.Dot(GameManager.Instance.SlicingPlaneNormal, v1 - GameManager.Instance.SlicingPlanePoint);

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
