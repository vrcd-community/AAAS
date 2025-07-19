Shader "Unlit/ScreenSpaceUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100
        Cull Front

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
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv.xy / i.uv.w;
                #if UNITY_SINGLE_PASS_STEREO
                float4 scaleOffset = unity_StereoScaleOffset[unity_StereoEyeIndex];
                uv = (uv - scaleOffset.zw) / scaleOffset.xy;
                #endif
                float aspect = _ScreenParams.x / _ScreenParams.y;
                if (aspect > 1.0)
                {
                    uv.y = 0.5 + (uv.y - 0.5) / aspect;
                }
                else
                {
                    uv.x = 0.5 + (uv.x - 0.5) * aspect;
                }
                uv = TRANSFORM_TEX(uv, _MainTex);
                fixed4 col = tex2D(_MainTex, uv);
                return col;
            }
            ENDCG
        }
    }
}