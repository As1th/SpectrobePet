Shader "Custom/UnlitTransparentCutout"
{
    Properties {
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" { }
        _Cutoff ("Alpha cutoff", Range(0.000000,1.000000)) = 0.500000
    }
    SubShader {
        Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" }
        Pass {
            // Disable backface culling
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float _Cutoff;

            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target {
                half4 texColor = tex2D(_MainTex, i.uv);
                if (texColor.a < _Cutoff)
                    discard;
                return texColor;
            }
            ENDCG
        }
    }
    FallBack "Unlit/Transparent Cutout"
}
