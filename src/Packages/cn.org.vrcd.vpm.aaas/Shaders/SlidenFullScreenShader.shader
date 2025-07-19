Shader "ChikuwaProducts/SlidenScreenShaderFullScreen"
{
    Properties
    {
        _MainTex("Standby Texture", 2D) = "black" {}
        _Brightness("Screen Brightness", Float) = 1
        _Contrast("Contrast", Float) = 1
        [ToggleUI] _FadeEdges("Anti-alias Edges", Float) = 1
        [KeywordEnum(Normal, Overlay)] _RenderMode ("Render Mode", Float) = 0
        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Culling", Int) = 2
    }
    SubShader
    {
        Pass
        {
            Tags
            {
                "Queue"="Overlay-1"
            }

            Cull [_Cull]
            ZTest Always

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _RENDERMODE_NORMAL _RENDERMODE_OVERLAY
            #pragma shader_feature_local _CLIP_BORDERS
            #include "UnityCG.cginc"
            #include "Packages/dev.architech.protv/Resources/Shaders/ProTVCore.cginc"

            float _Contrast;

            float2 getScreenUV(float4 screenPos)
            {
                float2 uv = screenPos / (screenPos.w + 0.0000000001);
                return uv;
            }

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.screenPos = ComputeNonStereoScreenPos(o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                FragmentProcessingData data = InitializeData(getScreenUV(i.screenPos));
                data.outputAspect = _ScreenParams.x / _ScreenParams.y;
                fixed4 col = ProcessFragment(data);
                col.rgb = pow(col.rgb, _Contrast);
                return col;
            }
            ENDCG
        }


    }
    FallBack Off
}