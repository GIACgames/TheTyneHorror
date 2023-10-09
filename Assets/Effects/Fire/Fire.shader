Shader "Unlit/Fire"
{
    Properties
    {
        _MainTex ("MainNoise", 2D) = "white" {}
        _Gradient ("Gradient", 2D) = "white" {}
        _ScrollSpeed("ScrollSpeed", float) = 1
        _Intensity("Intensity", float) = 1
        _Scale("Scale", float) = 1
    }
    SubShader
    {
        Tags {"IgnoreProjector"="True" "RenderType"="Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull back 
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

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
            sampler2D _Gradient;
            float _ScrollSpeed;
            float _Intensity;
            float _Scale;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float2 f = i.uv;
                f.x = (f.x * _Scale) % 1; f.y = (f.y* _Scale) % 1;
                fixed4 g = tex2D(_Gradient, f);
                f.y = (f.y + (_Time.y * _ScrollSpeed)) % 1;
                fixed4 col = tex2D(_MainTex, f);
                
                float vari = ((sin(_Time.y * 10) + 1) * 0.1 * _Intensity);
                if (g.a < _Intensity - vari)
                {
                col = col * (1 + (_Intensity));//step(g, col);
                col.a = 1;
                }
                else {col.a = 0;}
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
