Shader "Unlit/OutlineGlow"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1, 0.9, 0.2, 1)
        _OutlineThickness ("Outline Thickness", Range(0.001, 0.05)) = 0.01
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 1
    }

    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _OutlineColor;
            float _OutlineThickness;
            float _GlowIntensity;

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                float alpha = col.a;

                float outline = 0.0;
                float samples = 16;
                float radius = _OutlineThickness;

                for (int j = 0; j < samples; j++)
                {
                    float angle = 6.2831 * j / samples;
                    float2 offset = float2(cos(angle), sin(angle)) * radius;
                    float sampleAlpha = tex2D(_MainTex, i.uv + offset).a;
                    outline = max(outline, sampleAlpha);
                }

                float edge = saturate(outline - alpha);

                float4 glow = _OutlineColor * edge * _GlowIntensity;
                float4 baseColor = col * _Color;

                return baseColor + glow;
            }
            ENDCG
        }
    }

    FallBack "Sprites/Default"
}
