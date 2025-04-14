Shader "Custom/2DToon"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white"{}
        _Ramp ("Ramp Texture", 2D) = "white" {}
        _PosterizeSteps ("Posterize Steps", Range(1,10)) = 3
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"            
        }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;                
            };

            sampler2D _MainTex, _Ramp;
            float _PosterizeSteps;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                col.rgb = floor(col.rgb * _PosterizeSteps) / _PosterizeSteps;
                return col;
            }
            ENDCG
        }
        
    }
    FallBack "Diffuse"
}
