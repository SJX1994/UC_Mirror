Shader "Spine/Skeleton-processing" {
	Properties {
		[PerRendererData]_Color("Color", Color) = (1,1,1,1)
		[PerRendererData]_BeenAttackedColor("BeenAttackedColor", Color) = (0,0,0,0)
		[PerRendererData]_SelectOutlineColor ("Outline Color", Color) = (0,0,0,0)
        _SelectOutlineWidth ("Outline Width", Range(0, 1)) = 0.005
		_MaxPorcess("MaxPorcess", Float) = 1.0
		_MinPorcess("MinPorcess", Float) = 0.0
		_Amplitude("Amplitude", Float) = 1.0
		[PerRendererData]_Alpha("Alpha", Range(0,1)) = 0.0
		[PerRendererData]_Porcess("Porcess", Range(0,1)) = 1.0

		[PerRendererData]_Foreshadow("Foreshadow", Range(0,1)) = 0.0
		_ForeshadowWidth ("Foreshadow Width", Range(0.01, 1)) = 0.1
        _ForeshadowFrequency ("Foreshadow Frequency", Range(1, 10)) = 5
		_ForeshadowStrength ("Foreshadow Strength", Range(0, 1)) = 0.3
		_Cutoff ("Shadow alpha cutoff", Range(0,1)) = 0.1
		[NoScaleOffset] _MainTex ("Main Texture", 2D) = "black" {}
		
		[Toggle(_STRAIGHT_ALPHA_INPUT)] _StraightAlphaInput("Straight Alpha Texture", Int) = 0
		[HideInInspector] _StencilRef("Stencil Reference", Float) = 1.0
		[HideInInspector][Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comparison", Float) = 8 // Set to Always as default

		// Outline properties are drawn via custom editor.
		[HideInInspector] _OutlineWidth("Outline Width", Range(0,8)) = 3.0
		[HideInInspector] _OutlineColor("Outline Color", Color) = (1,1,1,1)
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
			Tags
			{
				"RenderPipeline" = "UniversalPipeline"
				"LightMode" = "UniversalForward"
			}
			CGPROGRAM
			#define PI 3.14159265358
			#pragma shader_feature _ _STRAIGHT_ALPHA_INPUT
			#pragma multi_compile_instancing // 单个材质多属性修改
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "CGIncludes/Spine-Common.cginc"
			sampler2D _MainTex;
			fixed _Foreshadow;
			fixed _ForeshadowWidth;
			fixed _ForeshadowFrequency;
			fixed _ForeshadowStrength;
			fixed4 _BeenAttackedColor;
			fixed _MaxPorcess;
			fixed _MinPorcess;
			float4 _SelectOutlineColor;
			float _SelectOutlineWidth;
			fixed _Porcess;
			fixed _Amplitude;
			fixed _Alpha;
			fixed4 _Color;
			
			struct VertexInput {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
			};

			struct VertexOutput {
				float4 pos : SV_POSITION;
				float4 objectPos:COLOR1;
				float2 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
			};
			// 计算条纹的透明度
        	float CalculateStripeAlpha(float2 uv, float width, float frequency)
        	{
        	    float stripe = floor(uv.x * frequency);
        	    float d = distance(uv.x * frequency,  stripe);
        	    float alpha = step(width, d);
        	    return saturate(alpha);
        	}
			VertexOutput vert (VertexInput v) {
				VertexOutput o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.vertexColor = PMAGammaToTargetSpace(v.vertexColor);
				o.objectPos = v.vertex;
				return o;
			}

			float4 frag (VertexOutput i) : SV_Target {
				float4 texColor = tex2D(_MainTex, i.uv);
				float objectYcolor = smoothstep(_MinPorcess,_MaxPorcess,i.objectPos.y) ;
				
				float m_sinTime = _Amplitude*sin((2.0*PI/1.0)*(i.objectPos.x+_Time*10.0)) ;
				objectYcolor += m_sinTime *0.05;
				
				objectYcolor = 1.0-step(_Porcess,objectYcolor);
				float3 color = texColor.rgb;
				float dotRes = dot( color , float3( 0.2126 , 0.7152 , 0.0722 ) );
				float3 gray = float3( dotRes,dotRes,dotRes );
				float3 texNoColor = lerp( color , gray , 1.0 ) ;
				texColor.rgb = lerp(texNoColor,texColor.rgb,objectYcolor);
				texColor.rgb *= _Color;
				float foreshadow = CalculateStripeAlpha(i.objectPos, _ForeshadowWidth * _Foreshadow, _ForeshadowFrequency);
				texColor.rgb += texColor.a * _Foreshadow * foreshadow * _ForeshadowStrength;
				texColor.rgba *= foreshadow;
				texColor.rgba *= _Alpha;
				texColor.rgb += _BeenAttackedColor.rgb * texColor.a;
				// 插值检查
				//texColor.rgb = step(_MinPorcess,i.objectPos.y);
				#if defined(_STRAIGHT_ALPHA_INPUT)
				texColor.rgb *= texColor.a *_Alpha ;
				#endif
				
				return (texColor * i.vertexColor);
			}
			ENDCG
		}
		Pass {
			Name "OutLine"
			Tags
			{
				"RenderPipeline" = "UniversalPipeline"
				// 黑客行为
			}
			ZWrite Off
			CGPROGRAM
			#define PI 3.14159265358
			#pragma shader_feature _ _STRAIGHT_ALPHA_INPUT
			#pragma multi_compile_instancing // 单个材质多属性修改
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "CGIncludes/Spine-Common.cginc"
			sampler2D _MainTex;
			float4 _SelectOutlineColor;
			float _SelectOutlineWidth;
			fixed _Alpha;
			fixed4 _Color;
			struct VertexInput {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
			};

			struct VertexOutput {
				float4 pos : SV_POSITION;
				float4 objectPos:COLOR1;
				float2 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
			};
			// 缩放矩阵
        	float4 OutLine(float4 vertPos,float outLine)
        	{
        	    float4x4 scaleMat;
				scaleMat[0][0] = 1.0 + outLine;
				scaleMat[0][1] = 0.0;
				scaleMat[0][2] = 0.0;
				scaleMat[0][3] = 0.0;
				scaleMat[1][0] = 0.0;
				scaleMat[1][1] = 1.0 + outLine;
				scaleMat[1][2] = 0.0;
				scaleMat[1][3] = 0.0;
				scaleMat[2][0] = 0.0;
				scaleMat[2][1] = 0.0;
				scaleMat[2][2] = 1.0 + outLine;
				scaleMat[2][3] = 0.0;
				scaleMat[3][0] = 0.0;
				scaleMat[3][1] = 0.0;
				scaleMat[3][2] = 0.0;
				scaleMat[3][3] = 1.0 + outLine;
        	    return mul(scaleMat,vertPos);
        	}
			VertexOutput vert (VertexInput v) {
				VertexOutput o;
				o.pos = UnityObjectToClipPos(OutLine(v.vertex,_SelectOutlineWidth));
				o.uv = v.uv;
				return o;
			}

			float4 frag (VertexOutput i) : SV_Target {
				float4 texColor = tex2D(_MainTex, i.uv);
				// float4 finalColor = smoothstep(texColor.a + 0.1f,texColor.a,_SelectOutlineColor);
				float4 finalColor = texColor.a * _SelectOutlineColor * _Alpha;
				return (finalColor);
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
