Shader "Custom/AttactionLine"
{
    Properties
    {
        [MainTexture] _BaseMap ("Texture", 2D) = "white" {}
        [MainColor]_Color("Color", Color) = (1,1,1,1)
        _FlowSpeed ("Flow Speed", Range(0, 10)) = 1
        _DistortionStrength ("Distortion Strength", Range(0, 1)) = 0.5
        _AlphaTest("Alpha",Range(0,1)) = 1.0
    }

    SubShader
    {
        Tags { 
            "RenderPipeline"="UniversalRenderPipeline"
            "RenderType"="Opaque" 
        }

        Pass {
            Name "AttactionLine"
           
            HLSLPROGRAM
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

            #pragma vertex vert
            #pragma fragment frag

            float _FlowSpeed;
            float _DistortionStrength;
            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half _AlphaTest;
                float4 _BaseMap_ST;
            CBUFFER_END

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            
            struct Varyings
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            
            
           

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.vertex = TransformObjectToHClip(v.vertex);
                float time = _Time.y * _FlowSpeed;
                float2 uvFlow = TRANSFORM_TEX(float2(v.uv.x - time, v.uv.y), _BaseMap);
                o.uv = uvFlow;
                o.color = v.color * _Color;
                return o;

            }
            
            half4 frag (Varyings i) : SV_Target
            {
                half4  tex = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,i.uv);
                clip(tex.a - _AlphaTest);
                return tex *  i.color;
            }
            ENDHLSL
            
        }
        
    }
}