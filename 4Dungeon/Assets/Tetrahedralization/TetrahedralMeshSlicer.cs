using System;
using System.Drawing;
using System.Security.Cryptography;
using Assets.Tetrahedralization;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Tetrahedralization
{
public class TetrahedralMeshSlicer : MonoBehaviour
{
    public TetrahedralMesh TetraMesh;
    public Plane4D plane;
    ComputeShader shad;
    int kernelIndex; 

    void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        shad = Resources.Load<ComputeShader>(Constants.SLICE_SHADER_PATH);
        kernelIndex = shad.FindKernel("CSMain");
    }

    void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
    }

    void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (camera == Camera.main) Slice();
    }

    void Slice()
    {
        ComputeBuffer vertsBuff = new(TetraMesh.verts.Length, sizeof(float) * 4);
        ComputeBuffer tetrsBuff = new(TetraMesh.tetrs.Length, sizeof(int) * 4);
        ComputeBuffer trisOutBuff = new(TetraMesh.tetrs.Length * 2, sizeof(int) * 3, ComputeBufferType.Append);
        ComputeBuffer vOutBuff = new(TetraMesh.tetrs.Length * 2 * 3, sizeof(float) * 4, ComputeBufferType.Append);
        vertsBuff.SetData(TetraMesh.verts); 
        tetrsBuff.SetData(TetraMesh.tetrs);

        // Bind to shader
        shad.SetBuffer(kernelIndex, "Vertices", vertsBuff);
        shad.SetBuffer(kernelIndex, "Tetrahedra", tetrsBuff);
        shad.SetBuffer(kernelIndex, "VerticesOut", vOutBuff);
        shad.SetBuffer(kernelIndex, "TrianglesOut", trisOutBuff);

        shad.SetVector(Shader.PropertyToID("planeNormal"), plane.normal);
        shad.SetFloat(Shader.PropertyToID("planeOffset"), plane.offset);
        // Dispatch - one thread per tetrahedron
        // Note that we can handle 64**2 tetrahedra in a single mesh this way. 
        int threadGroups = Mathf.CeilToInt(TetraMesh.tetrs.Length / 64.0f);
        shad.Dispatch(kernelIndex, threadGroups, 1, 1);
    }



}
}