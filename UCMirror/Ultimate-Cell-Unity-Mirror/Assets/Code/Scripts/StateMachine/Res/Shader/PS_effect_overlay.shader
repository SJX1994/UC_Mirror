Shader "Photoshop/Overlay"
{
     Properties
     {
         _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
         _Color ("Tint", Color) = (1,1,1,1)
     }
 
     SubShader
     {
         Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
         ZWrite Off Lighting Off Cull Off Fog { Mode Off } Blend OneMinusDstColor One
         LOD 110
     
         Pass
         {
             CGPROGRAM
             #pragma vertex vert_vct
             #pragma fragment frag_mult
             #pragma fragmentoption ARB_precision_hint_fastest
             #include "UnitySprites.cginc"
             
             struct vin_vct
             {
                 float4 vertex : POSITION;
                 float4 color : COLOR;
                 float2 texcoord : TEXCOORD0;
             };
             struct v2f_vct
             {
                 float4 vertex : POSITION;
                 fixed4 color : COLOR;
                 half2 texcoord : TEXCOORD0;
             };
             v2f_vct vert_vct(vin_vct v)
             {
                 v2f_vct o;
                 o.vertex = UnityObjectToClipPos(v.vertex);
                 o.color = v.color;
                 o.texcoord = v.texcoord;
                 return o;
             }
             float4 frag_mult(v2f_vct i) : COLOR
             {
                 float4 tex = tex2D(_MainTex, i.texcoord);
             
                 float4 final;            
                 final.rgba = i.color.rgba * tex.rgba * i.color.a * tex.a;
                //  final.a = i.color.a * tex.a;
                 return final;
             
             }
             ENDCG
         
         }
     
       
     }
 
}