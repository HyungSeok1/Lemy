Shader "Custom/RadialFillSprite"
{
    Properties
    {
        _MainTex ("Sprite", 2D) = "white" {}
        _FillAmount ("Fill Amount", Range(0,1)) = 1
        _Color ("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "PreviewType"="Plane" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off
        Fog { Mode Off }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _FillAmount;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 center = float2(0.5, 0.5); // 원형 기준
                float2 offset = uv - center;

                float angle = atan2(offset.x, -offset.y); // -PI ~ PI
                angle = angle / (2 * 3.14159265) + 0.5; // 0 ~ 1로 정규화 (시계방향)
                angle = 1.0 - angle;

                // Radial Fill: 0~1 범위에서 fill된 영역만 통과
                if (angle > _FillAmount)
                    discard;

                float4 col = tex2D(_MainTex, i.uv) * _Color;
                return col;
            }
            ENDCG
        }
    }
}
