Shader "Custom/HUD/HUDBatchShader"
{
    Properties
    {
        _FontTex("Font Texture", 2D) = "white" {}
    	_Color ("Main Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma enable_d3d11_debug_symbols
            #include "UnityCG.cginc"

            sampler2D _FontTex;
            float4 _FontTex_ST;

            fixed4 _Color;  

            struct appdata
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 vertexColor : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _FontTex);
                o.vertexColor = v.color;
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 color = fixed4(0,0,0,0);
                if (i.vertexColor.a > 1.0)
                {
                    color = i.vertexColor;
                }
                else
                {
                    color = tex2D(_FontTex, i.uv);
                }
            	return color;
            }
            ENDCG
        }
    }
}

