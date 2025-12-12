using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Tetrahedralization
{
// [ExecuteInEditMode]
[RequireComponent(typeof(Transform4D))]
public class TetrahedralMeshSlicer : MonoBehaviour
{
    public TetrahedralMesh TetraMesh {get; private set;}

    [SerializeField] private Material material;

    [SerializeField] public Color m_baseColor = Color.red;
    [SerializeField] public Color m_FarColor = Color.black;
    ComputeShader shad;
    int kernelIndex; 

    ComputeBuffer vertsBuff = null;
    ComputeBuffer tetrsBuff = null;
    ComputeBuffer vOutBuff = null;
    ComputeBuffer normsOutBuff = null;
    private GraphicsBuffer argsBuffer;

    private Transform4D transform4D;

    uint[] args = new uint[] { 0, 1, 0, 0 };

    public void Start()
    {
        this.transform4D = gameObject.GetComponent<Transform4D>();
    }
    //must call this to set mesh
    public void SetMesh(TetrahedralMesh tetMesh)
        {
        shad = Resources.Load<ComputeShader>(Constants.SLICE_COMPUTE_PATH);
        TetraMesh = tetMesh;
        material = new Material(Resources.Load<Shader>(Constants.SLICE_MAT_SHADER_PATH));
        kernelIndex = shad.FindKernel("CSMain");
        //setup buffers (discounting _Args)
        //input 
        vertsBuff = new(TetraMesh.verts.Length, sizeof(float) * 4);
        tetrsBuff = new(TetraMesh.tetrs.Length, sizeof(int) * 4);
        vertsBuff.SetData(TetraMesh.verts);
        tetrsBuff.SetData(TetraMesh.tetrs);

        // Bind to shader
        //input
        shad.SetBuffer(kernelIndex, "Vertices", vertsBuff);
        shad.SetBuffer(kernelIndex, "Tetrahedra", tetrsBuff);

        //output
        vOutBuff = new(TetraMesh.tetrs.Length * 6, sizeof(float) * 4, ComputeBufferType.Default);
        normsOutBuff = new(TetraMesh.tetrs.Length * 6, sizeof(float) * 3, ComputeBufferType.Default);
        
        //output
        shad.SetBuffer(kernelIndex, "VerticesOut", vOutBuff);
        shad.SetBuffer(kernelIndex, "NormalsOut", normsOutBuff);

        // Setup args for draw call
        // Non-indexed
        argsBuffer = new(
            GraphicsBuffer.Target.IndirectArguments, 
            4, 
            sizeof(uint)
        );
    }

    void Update()
    {
        if (TetraMesh == null) {
            Debug.Log("no tetra mesh attached");
            return;
        }
        Slice();
    }

    void Slice()
    {
        if (TetraMesh.verts == null || TetraMesh.verts.Length == 0 ||
            TetraMesh.tetrs == null || TetraMesh.tetrs.Length == 0)
        {
            Debug.LogWarning("no data to slice!");
            return; // No data to slice
        }
        // Initialize with zeros or starting values
        args = new uint[]{0, 1, 0, 0};
        argsBuffer.SetData(args);
        // We will update _Args[0] (vertex count) from inside the compute shader, thus keeping all data on GPU.
        shad.SetBuffer(kernelIndex, "_Args", argsBuffer);

        // Rebind buffers each frame since compute shader is shared across instances
        shad.SetBuffer(kernelIndex, "Vertices", vertsBuff);
        shad.SetBuffer(kernelIndex, "Tetrahedra", tetrsBuff);
        shad.SetBuffer(kernelIndex, "VerticesOut", vOutBuff);
        shad.SetBuffer(kernelIndex, "NormalsOut", normsOutBuff);

        //uniforms
        shad.SetVector("planeNormal", GameManager.Instance.SlicingPlaneNormal);
        shad.SetVector("planePoint", GameManager.Instance.SlicingPlanePoint);
        //optional if we were to use the angle-based sort for fixing winding order
        shad.SetVector("cameraWorldPos", new Vector4(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z, 1));
        shad.SetInt("tetraCount", TetraMesh.tetrs.Length);        
        //Transform 4d stuffs
        shad.SetMatrix("rotationMatrix", transform4D.m_RotationMatrix);
        shad.SetVector("scale", transform4D.Scale);
        shad.SetVector("position", transform4D.Position);

        // Dispatch - one thread per tetrahedron
        // Note that we can handle 64**2 tetrahedra in a single mesh this way.
        int threadGroups = Mathf.CeilToInt(TetraMesh.tetrs.Length / 64.0f);

        shad.Dispatch(kernelIndex, threadGroups, 1, 1);

        material.SetBuffer("VerticesOut", vOutBuff);
        material.SetBuffer("NormalsOut", normsOutBuff);
        material.SetColor("_BaseColor", m_baseColor);
        material.SetColor("_FarColor", m_FarColor);
        var stink = new uint[] { 0, 1, 0, 0 };
        argsBuffer.GetData(stink);
        // Time for draw call
        Bounds bounds = new(Vector3.zero, Vector3.one * 10000f);
        //set the vertex buff for our custom vert shader
        Graphics.DrawProceduralIndirect(material, bounds, MeshTopology.Triangles, argsBuffer);
    }

    void OnDestroy()
{
    vertsBuff?.Dispose();
    tetrsBuff?.Dispose();
    vOutBuff?.Dispose();
    argsBuffer?.Dispose();
    normsOutBuff?.Dispose();
}

    void DebugVis(Vector4[] arr)
    {
        foreach (Vector4 vec in arr)
            {
                Debug.DrawLine(Vector3.zero, vec, Color.red, 1, false);
            }
    }




}
}