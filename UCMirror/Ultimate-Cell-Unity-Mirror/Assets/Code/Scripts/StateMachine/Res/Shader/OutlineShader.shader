Shader "Custom/OutlineShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth ("Outline Width", Range(0, 1)) = 0.01
        _FresnelPower ("Fresnel Power", Range(1, 10)) = 5
    }
 
    SubShader
    {
         Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
         ZWrite Off Lighting Off Cull Off Fog { Mode Off } Blend OneMinusDstColor One
         LOD 110
 
        Pass
        {
            Cull Back
            ZWrite Off
            ZTest LEqual
            Blend OneMinusSrcAlpha SrcAlpha   // 修改混合模式为透明
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
 
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };
 
            sampler2D _MainTex;
            float4 _Color;
            float4 _OutlineColor;
            float _OutlineWidth;
            float _FresnelPower;
 
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                UNITY_TRANSFER_FOG(o,o.vertex);
 
                return o;
            }
 
            fixed4 frag (v2f i) : SV_Target
            {
                // 原始颜色
                fixed4 origColor = tex2D(_MainTex, i.uv) * _Color;
 
                // 描边颜色
                fixed4 outlineColor = _OutlineColor;
 
                // 计算菲涅尔因子
                float fresnelFactor = pow(1 - dot(i.worldNormal, normalize(_WorldSpaceCameraPos.xyz)), _FresnelPower);
 
                // 根据菲涅尔因子与描边宽度进行比较来设置描边颜色
                fixed4 finalColor = origColor;
                if (fresnelFactor < _OutlineWidth)
                {
                    finalColor = outlineColor;
                }
 
                // 将描边外的区域设置为透明
                if (fresnelFactor >= _OutlineWidth && fresnelFactor <= _OutlineWidth + 0.01) // 调整一个阈值以平滑过渡
                {
                    finalColor.a = 0.0;
                }
 
                UNITY_APPLY_FOG_COLOR(i.fogCoord, finalColor, fixed4(0,0,0,0));
                return finalColor;
            }
 
            ENDCG
        }
    }
}