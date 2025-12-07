using UnityEngine;
using Unity.VisualScripting;

namespace Assets.Tetrahedralization {

[ExecuteInEditMode]
[RequireComponent(typeof(TetrahedralMeshSlicer))]
public class CreateSimplex : MonoBehaviour
{
    [SerializeField] private Mesh m_Mesh;
    [SerializeField] private Plane4D m_Plane;

    [SerializeField] private bool m_Debug4D;

    [SerializeField] private bool m_DebugTetraMesh;

    [SerializeField] private Transform4D m_Transform4D;

    private TetrahedralMeshSlicer slicer;
    private MeshFilter filter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (m_Transform4D == null)
        {
            var thing = gameObject.GetComponent<Transform4D>();
            m_Transform4D = thing == null ? gameObject.AddComponent<Transform4D>() : thing;
        }
        slicer = GetComponent<TetrahedralMeshSlicer>();
        slicer.TetraMesh = Simplex.GetTetrahedralMesh();
        slicer.plane = GameManager.Instance.slicingPlane;
    }

    // Update is called once per frame
    void Update()
    {
        Camera worldCam = Camera.main;
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            var sceneCamera = SceneViewCameraProvider.currentSceneViewCamera;
            if (sceneCamera != null) worldCam = sceneCamera;
            else Debug.LogWarning("Scene camera is null. But that's probably because the scene only just loaded.");
        }
#endif
    }
}
    
}