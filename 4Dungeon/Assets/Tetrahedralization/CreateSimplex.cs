using UnityEngine;
using UnityEditor;
[ExecuteInEditMode]
public class CreateSimplex : MonoBehaviour
{
    [SerializeField] private Mesh m_Mesh;
    [SerializeField] private Plane4D m_Plane;

    [SerializeField] private bool m_Debug4D;
    private MeshFilter m_Filter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TetrahedralMesh t = new(Simplex.GetTetrahedra());
        if (m_Debug4D) DrawDebug(t);
        m_Filter = gameObject.GetComponent<MeshFilter>();
#if UNITY_EDITOR
        SlicingWorldState worldState;
        var sceneCamera = SceneViewCameraProvider.currentSceneViewCamera;
        worldState = new(Vector3.forward);
        if (sceneCamera != null) worldState = new(sceneCamera.transform.forward);
        else Debug.LogWarning("Scene camera is null. But that's probably because the scene only just loaded.");
#endif
        if (Application.isPlaying) worldState = new(Camera.main.transform.forward);
        m_Mesh = t.Slice(m_Plane, worldState);
        m_Filter.mesh = m_Mesh;
// #if UNITY_EDITOR 
//         // Material mat = GetComponent<MeshRenderer>().material;
//         Material mat = new(Shader.Find("Standard"));
//         mat.SetPass(0);
//         Graphics.DrawMeshNow(m_Mesh, gameObject.transform.position, gameObject.transform.rotation, 0);
// #endif
    }

    void DrawDebug(TetrahedralMesh t)
    {
        foreach (Tetrahedron teet in t.tetrs)
        {
            foreach (Edge e in teet.edges)
            {
                Debug.DrawLine(teet.vertices[e.Index0], teet.vertices[e.Index1]);
            }
        }
    }
}
