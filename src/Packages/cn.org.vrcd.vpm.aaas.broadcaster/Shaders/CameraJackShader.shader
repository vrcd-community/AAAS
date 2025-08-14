// Source: https://booth.pm/zh-cn/items/4848817

// VRカメラをジャックするシェーダー

Shader "Unlit/AAAS/CameraJackShader"
{
    Properties
    {
        _MainTex("Image", 2D) = "white" {}
        _TurnOffTex("TurnOff Tex", 2D) = "white" {}
        _CameraUpdateFlag("Camera Update Flag", Float) = 1
        _Distance("Distance", Range(0,1.0)) = 1.
        _LimitFoV_H("Limit FoV Horizontal", Float) = 90.0
        _Aspect("Aspect h/v", Float) = 1.7777777
        _ForceJack("Force Jack Mode", Float) = 0.0 // デスクトップモードで強制的に有効にしたいときなどに使う。

    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Overlay+6000" "IgnoreProjector" = "True" }
            LOD 100

            Pass
            {
                ZTest Always
                ZWrite Off
                Cull Off

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #define PI 3.14159265
            #define DEG2RAD PI / 180.0f
            #define RAD2DEG 180.0f / UNITY_PI

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _TurnOffTex;
            float _CameraUpdateFlag;

            float _Distance;
            float _LimitFoV_H;
            float _ForceJack;
            float _Aspect;


            v2f vert(appdata v)
            {
                v2f o;
                // uv座標をポジションにする。
                o.vertex = float4(2 * v.uv.x + 1 - 2, 1 - 2 * v.uv.y, 1, 1);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                if (_ProjectionParams.x > 0)
                {
                    //OpenGL
                    o.uv.y = 1.0f - o.uv.y;
                }
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // オブジェクトの原点を取得する
                float4 objectOrigin = mul(unity_ObjectToWorld, float4(0.0, 0.0, 0.0, 1.0));

                float distance = length(objectOrigin.xyz - _WorldSpaceCameraPos);

                if (!(_ForceJack > 0.0f))
                {
                    // 距離でカリング
                    if (distance > _Distance) discard;

                    // VRModeは除外する
                    float fovH = atan(1 / unity_CameraProjection._m11) * RAD2DEG * 2;
                    if (fovH > _LimitFoV_H) discard;
                }

                // アスペクト比の維持
                i.uv.y *= _Aspect * _ScreenParams.y / _ScreenParams.x;
                i.uv.y -= (_Aspect * _ScreenParams.y / _ScreenParams.x - 1.0f) * 0.5f;

                // 範囲外は黒帯に
                if (i.uv.y > 1.0f || i.uv.y < 0.0f)
                {
                    return float4(0, 0, 0, 1);
                }

                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 turnOffCol = tex2D(_TurnOffTex, i.uv);

                col = lerp(turnOffCol, col, _CameraUpdateFlag);
                return col;
            }
            ENDCG
        }
        }
}
