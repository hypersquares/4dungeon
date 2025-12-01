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
        Camera worldCam;
        var sceneCamera = SceneViewCameraProvider.currentSceneViewCamera;
        worldCam = Camera.main;
        if (sceneCamera != null) worldCam = sceneCamera;
        else Debug.LogWarning("Scene camera is null. But that's probably because the scene only just loaded.");
#endif
        if (Application.isPlaying) worldCam = Camera.main;
        m_Mesh = t.Slice(m_Plane, worldCam);
        m_Filter.mesh = m_Mesh;
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
