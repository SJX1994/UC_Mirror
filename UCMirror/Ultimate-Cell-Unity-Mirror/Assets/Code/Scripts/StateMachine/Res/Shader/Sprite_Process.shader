Shader "Custom/Sprite_Process_Shader"
{
    Properties
    {
        _Color ("Tint", Color) = (1,1,1,1)
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _MaxPorcess("MaxPorcess", Float) = 1.0
		_MinPorcess("MinPorcess", Float) = 0.0
        _Amplitude("Amplitude", Float) = 1.0
        [PerRendererData] _Porcess("Porcess", Range(0,1)) = 1.0
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "PreviewType"="Plane"}
        Fog { Mode Off }
		Cull Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		Lighting Off

        Pass
        {
            CGPROGRAM
            #define PI 3.14159265358
            #pragma multi_compile_instancing
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color    : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                fixed4 color    : COLOR;
                float4 vertex : SV_POSITION;
                float4 objectPos: COLOR1;
            };

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _Color;
            fixed _MaxPorcess;
			fixed _MinPorcess;
            fixed _Amplitude;
            fixed _Porcess;
            
            float FadeToGray(float3 baseColor)
        	{
        	    float3 color = baseColor;
				float dotRes = dot( color , float3( 0.2126 , 0.7152 , 0.0722 ) );
				float3 gray = float3( dotRes,dotRes,dotRes );
                float3 texNoColor = lerp( color , gray , 1.0 ) ;
        	    return saturate(texNoColor);
        	}
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color;
                o.objectPos = v.vertex;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                fixed4 noise = tex2D(_NoiseTex, i.uv/0.5);
                float objectYcolor = smoothstep(_MinPorcess,_MaxPorcess,i.objectPos.y);
                float m_sinTime = _Amplitude * sin( 3 * ( 2.0 * PI / 1.0 ) * (i.objectPos.x + _Time * 5.0)) * noise.r;
				objectYcolor += m_sinTime *0.05;
                objectYcolor = 1.0-step(_Porcess,objectYcolor);
                col.rgb *= col.a; 
                float3 texNoColor = FadeToGray(col.rgb);
                col.rgb = lerp(texNoColor,col.rgb,objectYcolor);
                return col;
            }
            ENDCG
        }
    }
}