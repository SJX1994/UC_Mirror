Shader "Custom/URPProgressBarShader"
{
    Properties
    {
        _Progress("Progress", Range(0, 1)) = 1
        _Color("Color", Color) = (1, 1, 1, 1)
        _Alpha("Alpha", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha // 使用混合模式来实现半透明效果
        LOD 100

        Pass
        {
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            
            float _Progress;
            float4 _Color;
            float _Alpha;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 将进度值限制在0到1之间
                float clampedProgress = clamp(_Progress, 0, 1);

                // 如果像素位于进度条范围内，则使用进度条颜色与透明度，否则使用原始颜色与透明度
            
                fixed4 finalColor = step(i.uv.y,clampedProgress) * _Color;
                finalColor.a *= _Alpha;
                return finalColor;
            }
            ENDCG
        }
    }
}