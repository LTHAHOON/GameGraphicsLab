Shader "Custom/Water_02"
{
    Properties
    {
        _Alpha("Alpha", Range(0.0, 1.0)) = 1.0
        [HDR] _WaterColor("Water Color", Color) = (1, 1, 1, 1)
        [MainTexture] _Water_Tex("Water Texture", 2D) = "white" {}
        _Water_OffsetSpeed("Water Offset Speed", Vector, 2) = (0, 0, 0, 0)
        _Wave_Height("Wave Height", Range(0.0, 0.5)) = 0.05
        _Wave_Frequency("Wave Frequency", Range(0.0, 20.0)) = 8.0
        _Wave_Speed("Wave Speed", Range(0.0, 5.0)) = 1.5
        _Alpha("Alpha", Range(0.0, 1.0)) = 1.0
        _Steam_Tex("Steam Texture", 2D) = "white" {}
        _Steam_Amount("Steam Amount", Range(0.0, 1.0)) = 0.0
        _Steam_Power("Steam Power", float) = 1
        _Steam_Intensity("Steam Intensity", float) = 1
        _Steam_OffsetSpeed("Steam Offset Speed", Vector, 2) = (0, 0, 0, 0)
        _Rim_Power("Rim Power", float) = 1
        _Rim_Intensity("Rim Intensity", float) = 1
    }

    SubShader
    {
        Tags 
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
        }

        Pass
        {
            Name "ForwardUnlit"

            Tags { "LightMode" = "UniversalForward" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 uvData : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
            };

            TEXTURE2D(_Water_Tex);
            SAMPLER(sampler_Water_Tex);
            TEXTURE2D(_Steam_Tex);
            SAMPLER(sampler_Steam_Tex);

            CBUFFER_START(UnityPerMaterial)
                float _Alpha;
                half4 _WaterColor;
                float4 _Water_Tex_ST;
                float2 _Water_OffsetSpeed;
                float _Wave_Height;
                float _Wave_Frequency;
                float _Wave_Speed;
                float4 _Steam_Tex_ST;
                float2 _Steam_OffsetSpeed;
                float _Steam_Amount;
                float _Steam_Power;
                float _Steam_Intensity;
                float _Rim_Power;
                float _Rim_Intensity;
            CBUFFER_END

            float WaterWaveHeight(float3 positionOS)
            {
                float time = _Time.y * _Wave_Speed;
                float waveX = sin(positionOS.x * _Wave_Frequency + time);
                float waveZ = cos(positionOS.z * _Wave_Frequency * 0.8 - time * 1.2);
                return (waveX + waveZ) * 0.5 * _Wave_Height;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 positionOS = IN.positionOS.xyz;
                positionOS += IN.normalOS * WaterWaveHeight(positionOS);
                OUT.positionHCS = TransformObjectToHClip(positionOS);
                float3 positionWS = TransformObjectToWorld(positionOS);
                OUT.uvData.xy = TRANSFORM_TEX(IN.uv, _Water_Tex);
                OUT.uvData.zw = TRANSFORM_TEX(IN.uv, _Steam_Tex);
                OUT.viewDir = GetWorldSpaceViewDir(positionWS);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                return OUT;
            }

            float OneMinus(float x)
            {
                return 1.0 - x;
            }

            float Rim_Lighting(float3 viewDir, float3 normal, float rimPower, float rimIntensity)
            {
                viewDir = normalize(viewDir);
                normal = normalize(normal);
                float fresnel = OneMinus(saturate(dot(viewDir, normal)));
                float rim = pow(fresnel, rimPower);
                rim *= rimIntensity;
                return rim;
            }

            void UVOffset_Move(inout float2 uv, float2 speed)
            {
                uv += _Time.y * speed;
            }

            void UVWave_Distort(inout float2 uv, float amplitude, float frequency, float speed)
            {
                float time = _Time.y * speed;
                float waveX = sin(uv.y * frequency + time);
                float waveY = cos(uv.x * frequency * 0.8 - time * 1.2);
                uv += float2(waveX, waveY) * amplitude;
            }


            half4 frag(Varyings IN) : SV_TARGET
            {
                float rim = Rim_Lighting(IN.viewDir, IN.normalWS,_Rim_Power, _Rim_Intensity);
                rim = (rim * 0.5f) + 0.5f;

                float2 waterUV = IN.uvData.xy;
                UVOffset_Move(waterUV, _Water_OffsetSpeed);
                UVWave_Distort(waterUV, 0.025, 8.0, 1.5);
                half4 color = SAMPLE_TEXTURE2D(_Water_Tex, sampler_Water_Tex, waterUV) * _WaterColor * rim;

                float2 blendUV = IN.uvData.zw;
                UVOffset_Move(blendUV, _Steam_OffsetSpeed);
                half4 blendColor = SAMPLE_TEXTURE2D(_Steam_Tex, sampler_Steam_Tex, blendUV) * _Steam_Intensity;
                blendColor.rgb = dot(blendColor.rgb, half3(0.299h, 0.587h, 0.114h));
                blendColor.rgb = pow(blendColor.rgb, _Steam_Power);
                color = lerp(color, blendColor, _Steam_Amount);
                color.a = _Alpha;
                return color;
            }

            ENDHLSL
        }
    }
}
