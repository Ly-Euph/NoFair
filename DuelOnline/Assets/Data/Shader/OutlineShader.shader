Shader "Custom/OutlineShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Main Color", Color) = (1,1,1,1)
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth("Outline Width", Range(0.0, 0.1)) = 0.03
    }

        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200

            // === ① アウトライン描画パス ===
            Pass
            {
                Name "Outline"
                Cull Front           // 裏面のみ描く（外向き法線で膨張）
                ZWrite On
                ZTest LEqual

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                };

                struct v2f
                {
                    float4 pos : SV_POSITION;
                };

                float _OutlineWidth;
                float4 _OutlineColor;

                v2f vert(appdata v)
                {
                    v2f o;
                    // 頂点を法線方向に押し出す
                    float3 norm = normalize(v.normal);
                    v.vertex.xyz += norm * _OutlineWidth;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    return _OutlineColor;
                }
                ENDCG
            }

            // === ② 通常の本体描画パス ===
            CGPROGRAM
            #pragma surface surf Standard fullforwardshadows

            sampler2D _MainTex;
            fixed4 _Color;

            struct Input
            {
                float2 uv_MainTex;
            };

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = c.rgb;
                o.Alpha = c.a;
            }
            ENDCG
        }

            FallBack "Diffuse"
}
