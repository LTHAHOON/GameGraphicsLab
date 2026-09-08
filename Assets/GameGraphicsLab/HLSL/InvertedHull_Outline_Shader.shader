Shader "Custom/InvertedHull_Outline_Shader"
{
    Properties
    {
        [MainColor] _OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineWidth("Outline Width", Range(0, 1)) = 1
        _ExpandRatioForCamera("Expand Ratio", float) = 1.5
        _MaxMinOutlineWidth("X: Max, Y: Min, Outline Width", Vector, 2) = (0.5, 0.05, 0, 0)
    }

    SubShader
    {
        Tags 
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "UniversalMaterialType" = "Unlit"
            "Queue" = "Geometry"
        }

        Pass
        {
            Tags { "LightMode" = "UniversalForward" }
            Cull Front
            Blend One Zero
            ZWrite On

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _OutlineColor;
                float _OutlineWidth;
                float _ExpandRatioForCamera;
                float2 _MaxMinOutlineWidth;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 cameraWS = _WorldSpaceCameraPos;
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                float cameraDist = distance(positionWS, cameraWS);
                float outlineWidth = _OutlineWidth * (cameraDist * 0.05 * _ExpandRatioForCamera);
                outlineWidth = clamp(outlineWidth, _MaxMinOutlineWidth.y, _MaxMinOutlineWidth.x);
                float4 normal =  IN.normalOS * (outlineWidth * 0.1);
                IN.positionOS =  IN.positionOS + normal;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
}
