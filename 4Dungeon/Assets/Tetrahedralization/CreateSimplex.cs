using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using Unity.Mathematics;
using Unity.VisualScripting;
[ExecuteInEditMode]
public class CreateSimplex : MonoBehaviour
{
    [SerializeField] private Mesh m_Mesh;
    [SerializeField] private Plane4D m_Plane;

    [SerializeField] private bool m_Debug4D;

    [SerializeField] private bool m_DebugTetraMesh;

    [SerializeField] private Transform4D m_Transform4D;
    private MeshFilter m_Filter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (m_Transform4D == null)
        {
            var thing = gameObject.GetComponent<Transform4D>();
            m_Transform4D = thing == null ? gameObject.AddComponent<Transform4D>() : thing;
        }
    }

    // Update is called once per frame
    void Update()
    {
        TetrahedralMesh t = new(Simplex.GetTetrahedra(), m_Transform4D);   
        if (m_Debug4D) DrawDebug(t);
        if (m_DebugTetraMesh)
        {
            m_DebugTetraMesh = !m_DebugTetraMesh;
            string tetra_string = "";
            for(int i = 0; i < t.tetrs.Length; i++)
            {
                tetra_string += $"t[{i}] :" + t.tetrs[i].vertices.ToCommaSeparatedString() + "\n";
            }
            Debug.Log(tetra_string);
        }
        m_Filter = gameObject.GetComponent<MeshFilter>();
        Camera worldCam = Camera.main;
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            var sceneCamera = SceneViewCameraProvider.currentSceneViewCamera;
            if (sceneCamera != null) worldCam = sceneCamera;
            else Debug.LogWarning("Scene camera is null. But that's probably because the scene only just loaded.");
        }
#endif
        m_Mesh = t.Slice(m_Plane, worldCam);
        m_Filter.mesh = m_Mesh;
    }

    void DrawDebug(TetrahedralMesh t)
    {
        foreach (Tetrahedron teet in t.tetrs)
        {
            foreach (Edge e in teet.edges)
            {
                Debug.DrawLine((Vector3) teet.vertices[e.Index0], (Vector3) teet.vertices[e.Index1] + gameObject.transform.position);
            }
        }
    }
}
