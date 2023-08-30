Shader "Game/Dissolve"
// https://twitter.com/minionsart/status/1005590210476900352
{
	Properties {
		_DisLineWidth("DisLineWidth", Float) = 1.0
		_DisAmount("DisAmount", Float) = 0.0
		_Amplitude("Amplitude", Float) = 1.0
        _NScale("Normal Scale", Float) = 1.0
		_Porcess("Porcess", Range(0,1)) = 0.0
        _DisLineColor("Line Color", Color) = (1,1,1,1)  
        _OffsetTiling("OffsetTiling", Vector) = (1, 1, 0, 0)
       
        

		_Cutoff ("Shadow alpha cutoff", Range(0,1)) = 0.1
        _BlendTex ("Blend Texture", 2D) = "black" {}
		_MainTex ("Main Texture", 2D) = "black" {}
        _NoiseTex("Noise Texture", 2D) = "black" {}
		
		[Toggle(_STRAIGHT_ALPHA_INPUT)] _StraightAlphaInput("Straight Alpha Texture", Int) = 0
		[HideInInspector] _StencilRef("Stencil Reference", Float) = 1.0
		[HideInInspector][Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comparison", Float) = 8 // Set to Always as default

		// Outline properties are drawn via custom editor.
		[HideInInspector] _OutlineWidth("Outline Width", Range(0,8)) = 3.0
		[HideInInspector] _OutlineColor("Outline Color", Color) = (1,1,0,1)
		[HideInInspector] _OutlineReferenceTexWidth("Reference Texture Width", Int) = 1024
		[HideInInspector] _ThresholdEnd("Outline Threshold", Range(0,1)) = 0.25
		[HideInInspector] _OutlineSmoothness("Outline Smoothness", Range(0,1)) = 1.0
		[HideInInspector][MaterialToggle(_USE8NEIGHBOURHOOD_ON)] _Use8Neighbourhood("Sample 8 Neighbours", Float) = 1
		[HideInInspector] _OutlineOpaqueAlpha("Opaque Alpha", Range(0,1)) = 1.0
		[HideInInspector] _OutlineMipLevel("Outline Mip Level", Range(0,3)) = 0
	}

	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }

		Fog { Mode Off }
		Cull Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		Lighting Off

		Stencil {
			Ref[_StencilRef]
			Comp[_StencilComp]
			Pass Keep
		}

		Pass {
			Name "Normal"

			CGPROGRAM
			#define PI 3.14159265358
			#pragma shader_feature _ _STRAIGHT_ALPHA_INPUT
			#pragma multi_compile_instancing // 单个材质多属性修改
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Assets/Plugins/Spine/Base/Runtime/spine-unity/Shaders/CGIncludes/Spine-Common.cginc"
			sampler2D _MainTex;
            sampler2D _BlendTex;
            sampler2D _NoiseTex;
            float4 _DisLineColor;
            float4 _OffsetTiling;
			fixed _DisLineWidth;
			fixed _DisAmount;
			fixed _Porcess;
            fixed _NScale;
			fixed _Amplitude;

			struct VertexInput {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
                half3 normal  : NORMAL;
			};

			struct VertexOutput {
				float4 pos : SV_POSITION;
				float4 objectPos:COLOR1;
				float2 uv : TEXCOORD0;
                half3 normal : TEXCOORD1;
				float4 vertexColor : COLOR;
			};
            
			VertexOutput vert (VertexInput v) {
				VertexOutput o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.vertexColor = PMAGammaToTargetSpace(v.vertexColor);
				o.objectPos = v.vertex;
				return o;
			}

            Texture2D _CameraDepthTexture;
            float4 _CameraDepthTexture_TexelSize;

            float3 rayFromScreenUV(in float2 uv, in float4x4 InvMatrix)
            {
              float x = uv.x * 2.0 - 1.0;
              float y = uv.y * 2.0 - 1.0;
              float4 position_s = float4(x, y, 1.0, 1.0);
              return mul(InvMatrix, position_s * _ProjectionParams.z);
            }
 
            float3 viewSpacePosAtPixelPosition(float2 pos)
            {
                float rawDepth = _CameraDepthTexture.Load(int3(pos, 0)).r;
                float2 uv = pos * _CameraDepthTexture_TexelSize.xy;
                float3 ray = rayFromScreenUV(uv, unity_CameraInvProjection);
                return ray * Linear01Depth(rawDepth);
            }
			float4 frag (VertexOutput i) : SV_Target {
                // triplanar noise
                float3 vpl = viewSpacePosAtPixelPosition(i.pos.xy + float2(-1, 0));
                float3 vpr = viewSpacePosAtPixelPosition(i.pos.xy + float2( 1, 0));
                float3 vpd = viewSpacePosAtPixelPosition(i.pos.xy + float2( 0,-1));
                float3 vpu = viewSpacePosAtPixelPosition(i.pos.xy + float2( 0, 1));
 
                float3 viewNormal = normalize(-cross(vpu - vpd, vpr - vpl));
                float3 WorldNormal = mul((float3x3)unity_MatrixInvV, viewNormal);
                float3 blendNormal = saturate(pow(WorldNormal * 1.4,4));
                half4 nSide1 = tex2D(_NoiseTex, (i.objectPos.xy + _Time.x) * _NScale); 
                half4 nSide2 = tex2D(_NoiseTex, (i.objectPos.xz - _Time.x) * _NScale);
                half4 nTop = tex2D(_NoiseTex, (i.objectPos.yz + _Time.x) * _NScale);
                float3 noisetexture = nSide1;
                noisetexture = lerp(noisetexture, nTop, blendNormal.x);
                noisetexture = lerp(noisetexture, nSide2, blendNormal.y);
                float3 DissolveLine = step(noisetexture - _DisLineWidth, _DisAmount) * step(_DisAmount,noisetexture) ;
                DissolveLine *= _DisLineColor; // color the line
                
                // new
                float4 texColor = tex2D(_MainTex, i.uv* _OffsetTiling.xy + _OffsetTiling.zw);
                float4 texColor2 = tex2D(_BlendTex, i.uv* _OffsetTiling.xy + _OffsetTiling.zw);
                float DissolveLineMask = saturate(smoothstep(_Porcess+DissolveLine.x-0.1,_Porcess+DissolveLine.x+0.1,i.uv.x ));
                _Porcess = lerp(DissolveLineMask,saturate(_Porcess)+0.5,smoothstep(_Porcess-0.2,_Porcess+0.2,i.uv.x-0.1));
                // mix
                float3 primaryTex = (step(noisetexture - _DisLineWidth,_DisAmount) *texColor.rgb);
                float3 secondaryTex = (step(_DisAmount, noisetexture) * texColor2.rgb);
               // float3 resultTex = lerp(texColor,texColor2,_Porcess);
                float3 resultTex = lerp(texColor , texColor2 ,  _Porcess);
                //resultTex = float3(1.0);
                texColor.rgb = resultTex;
				#if defined(_STRAIGHT_ALPHA_INPUT)
				
				
                


				#endif
				
				return (texColor * i.vertexColor);
			}
			ENDCG
		}

		Pass {
			Name "Caster"
			Tags { "LightMode"="ShadowCaster" }
			Offset 1, 1
			ZWrite On
			ZTest LEqual

			Fog { Mode Off }
			Cull Off
			Lighting Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			fixed _Cutoff;

			struct VertexOutput {
				V2F_SHADOW_CASTER;
				float4 uvAndAlpha : TEXCOORD1;
			};

			VertexOutput vert (appdata_base v, float4 vertexColor : COLOR) {
				VertexOutput o;
				o.uvAndAlpha = v.texcoord;
				o.uvAndAlpha.a = vertexColor.a;
				TRANSFER_SHADOW_CASTER(o)
				return o;
			}

			float4 frag (VertexOutput i) : SV_Target {
				fixed4 texcol = tex2D(_MainTex, i.uvAndAlpha.xy);
				clip(texcol.a * i.uvAndAlpha.a - _Cutoff);
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
	}
	CustomEditor "SpineShaderWithOutlineGUI"
}
