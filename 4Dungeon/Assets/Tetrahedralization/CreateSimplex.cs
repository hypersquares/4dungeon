using UnityEngine;

namespace Assets.Tetrahedralization {

[ExecuteInEditMode]
[RequireComponent(typeof(TetrahedralMeshSlicer))]
public class CreateSimplex : MonoBehaviour
{

    [SerializeField] private bool m_Debug4D;

    [SerializeField] private bool m_DebugTetraMesh;

    [SerializeField] private Transform4D m_Transform4D;

    private TetrahedralMeshSlicer slicer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (m_Transform4D == null)
        {
            var thing = gameObject.GetComponent<Transform4D>();
            m_Transform4D = thing == null ? gameObject.AddComponent<Transform4D>() : thing;
        }
        slicer = gameObject.GetComponent<TetrahedralMeshSlicer>();
        slicer.SetMesh(Simplex.GetTetrahedralMesh());
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
        if (slicer.TetraMesh == null) {
            Debug.Log("gothere");
            slicer.SetMesh(Simplex.GetTetrahedralMesh());
        }
    }
}
    
}