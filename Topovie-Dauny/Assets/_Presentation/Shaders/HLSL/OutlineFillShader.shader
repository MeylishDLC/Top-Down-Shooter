Shader "Unlit/OutlineFillShader"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1, 0.9, 0.2, 1)
        _OutlineThickness ("Outline Thickness", Range(0.001, 0.05)) = 0.01
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 1
        _OutlineFill ("Outline Fill (0-1)", Range(0,1)) = 1
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
            float _OutlineFill;

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

            float GetAngle(float2 uv)
            {
                float2 center = float2(0.5, 0.5);
                float2 dir = normalize(uv - center);
                float angle = atan2(dir.y, dir.x);
                angle = angle / 6.2831 + 0.5; // Normalize from -pi~pi to 0~1
                return angle;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                float alpha = col.a;

                float outline = 0.0;
                float samples = 16;
                float radius = _OutlineThickness;

                float angle = GetAngle(i.uv);

                for (int j = 0; j < samples; j++)
                {
                    float sampleAngle = 6.2831 * j / samples;
                    float2 offset = float2(cos(sampleAngle), sin(sampleAngle)) * radius;
                    float sampleAlpha = tex2D(_MainTex, i.uv + offset).a;
                    outline = max(outline, sampleAlpha);
                }

                float edge = saturate(outline - alpha);

                // Apply fill cutoff
                float visible = step(angle, _OutlineFill);
                float4 glow = _OutlineColor * edge * _GlowIntensity * visible;

                float4 baseColor = col * _Color;
                return baseColor + glow;
            }
            ENDCG
        }
    }

    FallBack "Sprites/Default"
}