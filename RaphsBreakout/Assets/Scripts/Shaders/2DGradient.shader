Shader "Custom/2DGradient"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _ColorTop ("Top Color", Color) = (1, 0.6, 0.6, 1)
        _ColorBottom ("Bottom Color", Color) = (0.71, 0.93, 0.84, 1)
        _BlendAxis ("Blend Axis", Range(0, 1)) = 1        
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            fixed4 _ColorTop, _ColorBottom;
            float _BlendAxis;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float blend = lerp(i.uv.x, i.uv.y, _BlendAxis);
                fixed4 gradient = lerp(_ColorBottom, _ColorTop, blend);
                return gradient * tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }    
}
