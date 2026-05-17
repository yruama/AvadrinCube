Shader "UnityLibrary/Standard/Effects/SeeThroughWalls_Full"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _BlurSize("BlurSize", float) = 10
        _SeeThroughOpacity("SeeThroughOpacityAdjust", Range(0,1)) = 0.5
    }

    SubShader
    {
        // -----------------------------
        // Pass normal
        // -----------------------------
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG

        // -----------------------------
        // Pass see-through
        // -----------------------------
        Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest Greater

        CGPROGRAM
        #pragma surface surf Standard alpha:fade
        #pragma target 3.0

        sampler2D _MainTex;
        float4 _MainTex_TexelSize;
        float _BlurSize;
        float _SeeThroughOpacity;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // flou simple 4-taps
            fixed4 cR = tex2D(_MainTex, float2(IN.uv_MainTex.x + _MainTex_TexelSize.x * _BlurSize, IN.uv_MainTex.y));
            fixed4 cL = tex2D(_MainTex, float2(IN.uv_MainTex.x - _MainTex_TexelSize.x * _BlurSize, IN.uv_MainTex.y));
            fixed4 cT = tex2D(_MainTex, float2(IN.uv_MainTex.x, IN.uv_MainTex.y + _MainTex_TexelSize.y * _BlurSize));
            fixed4 cB = tex2D(_MainTex, float2(IN.uv_MainTex.x, IN.uv_MainTex.y - _MainTex_TexelSize.y * _BlurSize));
            fixed4 c = (cR + cL + cT + cB) * 0.25;

            // préserve la couleur du matériau
            c.rgb *= _Color.rgb;

            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;

            // alpha semi-transparent
            o.Alpha = c.a * _SeeThroughOpacity;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
