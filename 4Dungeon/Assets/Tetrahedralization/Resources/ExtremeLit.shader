Shader "Custom/ExtremeLit"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _NearColor("Near W Color", Color) = (1, 1, 1, 1)
        _FarColor("Far W Color", Color) = (0, 0, 0, 1)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Tags { "LightMode" = "UniversalForward" }

            ZWrite On
            ZTest LEqual

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"


            StructuredBuffer<float4> VerticesOut;
            StructuredBuffer<float3> NormalsOut;

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float wValue : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _NearColor;
                half4 _FarColor;
            CBUFFER_END

            Varyings vert(uint id : SV_VertexID)
            {
                Varyings OUT;
                float4 vertex = VerticesOut[id];
                OUT.positionHCS = TransformWorldToHClip(vertex.xyz);
                OUT.normalWS = NormalsOut[id];
                OUT.wValue = vertex.w;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 normalWS = normalize(IN.normalWS);
                Light mainLight = GetMainLight();
                float NdotL = saturate(dot(normalWS, mainLight.direction));

                float3 ambient = 0.1;
                float3 diffuse = NdotL * mainLight.color;

                half4 color = lerp(_FarColor.rgba, _BaseColor.rgba, saturate(IN.wValue));
                color.rgb *= (ambient + diffuse);


                return color;
            }
            ENDHLSL
        }
    }
}
