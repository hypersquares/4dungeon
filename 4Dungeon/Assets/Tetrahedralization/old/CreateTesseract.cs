using UnityEngine;
using Unity.VisualScripting;

namespace Assets.Tetrahedralization.Old
{
 
[ExecuteInEditMode]
public class CreateTesseract : MonoBehaviour
{
    [SerializeField] private Mesh m_Mesh;
    // [SerializeField] private Plane4D m_Plane;

    [SerializeField] private bool m_Debug4D;

    [SerializeField] private bool m_DebugTetraMesh;

    [SerializeField] private Transform4D m_Transform4D;
    private MeshFilter filter;
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
        Old.TetrahedralMesh t = new(Tesseract.GetTetrahedra(), m_Transform4D);   
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
        Camera worldCam = Camera.main;
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            var sceneCamera = SceneViewCameraProvider.currentSceneViewCamera;
            if (sceneCamera != null) worldCam = sceneCamera;
            else Debug.LogWarning("Scene camera is null. But that's probably because the scene only just loaded.");
        }
#endif
        filter = gameObject.GetComponent<MeshFilter>();
        m_Mesh = t.Slice(GameManager.Instance.slicingPlane, worldCam);
        filter.mesh = m_Mesh;
        var collider = gameObject.GetComponent<MeshCollider>();
        collider.sharedMesh = m_Mesh;
    }

    void DrawDebug(Old.TetrahedralMesh t)
    {
        foreach (Tetrahedron teet in t.tetrs)
        {
            foreach (Assets.Tetrahedralization.Edge e in teet.edges)
            {
                Debug.DrawLine((Vector3) teet.vertices[e.Index0], (Vector3) teet.vertices[e.Index1] + gameObject.transform.position);
            }
        }
    }
}
   
}