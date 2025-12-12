using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Tetrahedralization
{
// [ExecuteInEditMode]
public class TetrahedralMeshSlicer : MonoBehaviour
{
    private TetrahedralMesh TetraMesh;

    [SerializeField] private Material material;
    ComputeShader shad;
    int kernelIndex; 

    ComputeBuffer vertsBuff = null;
    ComputeBuffer tetrsBuff = null;
    ComputeBuffer vOutBuff = null;
    ComputeBuffer normsOutBuff = null;
    private GraphicsBuffer argsBuffer;

    uint[] args = new uint[] { 0, 1, 0, 0 };

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
        Camera camera = Camera.main;
        if (camera != null) Slice(camera);
    }

        void Slice(Camera camera)
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
            
        //uniforms
        shad.SetVector("planeNormal", GameManager.Instance.SlicingPlaneNormal);
        shad.SetVector("planePoint", GameManager.Instance.SlicingPlanePoint);
        //optional if we were to use the angle-based sort for fixing winding order
        shad.SetVector("cameraWorldPos", new Vector4(camera.transform.position.x, camera.transform.position.y, camera.transform.position.z, 1));
        shad.SetInt("tetraCount", TetraMesh.tetrs.Length);        
        // Dispatch - one thread per tetrahedron
        // Note that we can handle 64**2 tetrahedra in a single mesh this way.
        int threadGroups = Mathf.CeilToInt(TetraMesh.tetrs.Length / 64.0f);

        shad.Dispatch(kernelIndex, threadGroups, 1, 1);
        
        material.SetBuffer("VerticesOut", vOutBuff);
        material.SetBuffer("NormalsOut", normsOutBuff);
        material.SetColor("_BaseColor", Color.red);
        var stink = new uint[] { 0, 1, 0, 0 };
        argsBuffer.GetData(stink);

        Debug.Log(args.ToCommaSeparatedString());
        uint a = args[0];
        Vector4[] arr = new Vector4[20];
        vOutBuff.GetData(arr);
        Debug.Log(arr.ToCommaSeparatedString());
        DebugVis(arr);
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
    void CreateMeshFromSlice(Vector4[] vertices4D, int[] triangles)
    {
        if (vertices4D.Length == 0 || triangles.Length == 0) return;

        // Create or update mesh
        Mesh mesh = new Mesh();
        mesh.vertices = vertices4D.Select(v => (Vector3)v).ToArray();
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // Render the mesh (you'll need to set this up based on your needs)
        // For example: GetComponent<MeshFilter>().mesh = mesh;
        // Or use Graphics.DrawMesh for immediate rendering
    }



}
}