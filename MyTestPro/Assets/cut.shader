// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Disappear"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color",Color) = (1,1,1,1)
        _Height("Height",Float) = 0.5
        _ColorRange("Color Range",Float) = 1
    }
 
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
             
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
                float height:TEXCOORD1;
            };
 
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _ColorRange;
            float _Height;
 
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                 
                o.height = v.vertex.y; 
 
                return o;
            }
             
            fixed4 frag (v2f i) : SV_Target
            {               
                fixed4 col = tex2D(_MainTex, i.uv);
                 
                clip(_Height-i.height);
                float fac = sign(saturate(_Height-_ColorRange-i.height));
 
                col.rgb = (1-fac)*_Color+col.rgb*fac;
                 
                return col;
            }
            ENDCG
        }
    }
}