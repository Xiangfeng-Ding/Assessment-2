Shader "Custom/URP_SimpleVegetation"
{
    Properties
    {
        _BaseMap ("漫反射贴图", 2D) = "white" {}
        _NormalMap ("法线贴图", 2D) = "bump" {}
        _Metallic ("金属度", Range(0, 1)) = 0.0
        _Smoothness ("光滑度", Range(0, 1)) = 0.0
        _AlphaCutoff ("镂空阈值", Range(0, 1)) = 0.6
        _WindAmplitude ("摆动幅度", Range(0, 2)) = 0.3
        _WindSpeed ("风速", Range(0, 5)) = 0.3
    }
    SubShader
    {
        Tags
        {
            "RenderType"="TransparentCutout" "Queue"="AlphaTest" "RenderPipeline"="UniversalPipeline"
        }
        LOD 100
        Cull Off

        Pass
        {
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
                float3 normalOS     : NORMAL;
                float4 color        : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float3 positionWS   : TEXCOORD1;
                float3 normalWS     : TEXCOORD2;
            };

            CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;
            float4 _NormalMap_ST;
            float _Metallic;
            float _Smoothness;
            float _AlphaCutoff;
            float _WindAmplitude;
            float _WindSpeed;
            CBUFFER_END

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);

            Varyings vert (Attributes input)
            {
                Varyings output;
                // 简单风吹顶点动画
                float windOffset = sin(_Time.y * _WindSpeed + input.positionOS.x + input.positionOS.z) * _WindAmplitude * input.color.r;
                float3 modifiedPos = input.positionOS.xyz + float3(windOffset, windOffset * 0.5, windOffset * 0.3);
                
                output.positionHCS = TransformObjectToHClip(modifiedPos);
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.positionWS = TransformObjectToWorld(modifiedPos);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                half4 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                clip(albedo.a - _AlphaCutoff);

                Light mainLight = GetMainLight();
                half diffuse = saturate(dot(normalize(input.normalWS), mainLight.direction));
                half3 finalColor = albedo.rgb * (mainLight.color * diffuse + unity_ambientSky);

                return half4(finalColor, albedo.a);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}