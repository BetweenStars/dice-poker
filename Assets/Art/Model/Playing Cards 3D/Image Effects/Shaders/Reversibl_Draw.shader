Shader "URP/Custom/LitCutoutDoubleSided_LightAdjust"
{
    Properties
    {
        _BaseMap("Front Texture", 2D) = "white" {}
        _BaseMapBack("Back Texture", 2D) = "white" {}
        _BaseColor("Color", Color) = (1,1,1,1)
        _Cutoff("Alpha Cutoff", Range(0, 1)) = 0.5
        _Smoothness("Smoothness", Range(0, 1)) = 0.5
        _Metallic("Metallic", Range(0, 1)) = 0
        _LightInfluence("Light Influence", Range(0, 1)) = 0.5 // ライトの影響度調整用
    }

    SubShader
    {
        Tags {
            "RenderType" = "TransparentCutout"
            "Queue" = "AlphaTest"
            "RenderPipeline" = "UniversalRenderPipeline"
        }

        Pass
        {
            Name "Universal Forward"
            Tags { "LightMode" = "UniversalForward" }

            Cull Off
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature_local _ALPHATEST_ON
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_BaseMapBack);
            SAMPLER(sampler_BaseMapBack);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float _Cutoff;
                float _Smoothness;
                float _Metallic;
                float _LightInfluence;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);

                output.positionCS = posInputs.positionCS;
                output.positionWS = posInputs.positionWS;
                output.normalWS = normalInputs.normalWS;
                output.uv = input.uv;
                return output;
            }

            half4 frag(Varyings input, bool isFrontFace : SV_IsFrontFace) : SV_Target
            {
                float2 uv = input.uv;

                // 前面と背面でテクスチャを切り替える
                half4 texColor = isFrontFace
                    ? SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv)
                    : SAMPLE_TEXTURE2D(_BaseMapBack, sampler_BaseMapBack, uv);

                half alpha = texColor.a * _BaseColor.a;
                clip(alpha - _Cutoff);

                float3 albedo = texColor.rgb * _BaseColor.rgb;

                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = albedo;
                surfaceData.metallic = _Metallic;
                surfaceData.smoothness = _Smoothness;
                surfaceData.alpha = alpha;

                InputData inputData = (InputData)0;
                inputData.positionWS = input.positionWS;
                inputData.normalWS = normalize(input.normalWS) * (isFrontFace ? 1 : -1);
                inputData.viewDirectionWS = normalize(GetCameraPositionWS() - input.positionWS);
                inputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                inputData.fogCoord = ComputeFogFactor(input.positionCS.z);

                half4 color = UniversalFragmentPBR(inputData, surfaceData);

                // ライトの影響を調整（0でUnlit相当、1で標準Lit相当）
                color.rgb = lerp(surfaceData.albedo, color.rgb, _LightInfluence);

                return color;
            }
            ENDHLSL
        }

        // 影を落とすためのShadowCaster Passを追加
        UsePass "Universal Render Pipeline/Lit/ShadowCaster"
    }
}
