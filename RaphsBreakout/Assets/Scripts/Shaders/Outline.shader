Shader "Custom/Outline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineSize ("Outline Size", Range(1,10)) = 1
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        
        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;                
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float2 screenuv : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 _Color;
            fixed4 _OutlineColor;
            float _OutlineSize;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color;

                #ifdef PIXELSNAP_ON
                o.vertex = UnityPixelSnap(o.vertex);
                #endif
                
                return o;
            }

            float2 uvPerWorldUnit(float2 uv, float2 worldPos)
            {
                float2 uvPerPixelX = abs(ddx(uv));
                float2 uvPerPixelY = abs(ddy(uv));
                float unitsPerPixelX = length(ddx(worldPos));
                float unitsPerPixelY = length(ddy(worldPos));
                return uvPerPixelX / unitsPerPixelX + uvPerPixelY / unitsPerPixelY;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.uv) * IN.color;

                if (c.a > 0.99) return c;

                float max_alpha = 0;
                float2 texel_size = _MainTex_TexelSize.xy * _OutlineSize;

                const float2 directions[8] =
                    {
                        float2(1, 0), float2(-1, 0), // Right, Left
                        float2(0, 1), float2(0, -1), // Up, Down
                        float2(1, 1) * 0.7071, float2(-1, 1) * 0.7071, // Diagonals
                        float2(-1, -1) * 0.7071, float2(1, -1) * 0.7071
                    };
                
                [unroll]
                for (int i = 0; i < 8; i++)
                {
                    float2 offset = directions[i] * texel_size;
                    max_alpha = max(max_alpha, tex2D(_MainTex, IN.uv + offset).a);
                }

                //float outline_factor = saturate(max_alpha - c.a);
                float outline_factor = smoothstep(0, 0.5, max_alpha - c.a);
                fixed4 result = lerp(c, _OutlineColor, outline_factor);
                result.a = max(c.a, max_alpha * _OutlineColor.a);
                return result;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
