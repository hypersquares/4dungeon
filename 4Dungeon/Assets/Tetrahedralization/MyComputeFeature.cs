// using UnityEngine;
// using UnityEngine.Rendering.Universal;
// using UnityEngine.Rendering;
// public class MyComputeFeature : ScriptableRendererFeature
// {
//     class MyComputePass : ScriptableRenderPass
//     {
//         public ComputeShader computeShader;
        
//         public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
//         {
//             CommandBuffer cmd = CommandBufferPool.Get("MyCompute");
            
//             cmd.SetComputeBufferParam(computeShader, 0, "Input", inputBuffer);
//             cmd.SetComputeBufferParam(computeShader, 0, "Output", outputBuffer);
//             cmd.DispatchCompute(computeShader, 0, threadGroups, 1, 1);
            
//             context.ExecuteCommandBuffer(cmd);
//             CommandBufferPool.Release(cmd);
//         }
//     }
    
//     MyComputePass pass;
    
//     public override void Create()
//     {
//         pass = new MyComputePass();
//         pass.renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
//     }
    
//     public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
//     {
//         renderer.EnqueuePass(pass);
//     }
// }