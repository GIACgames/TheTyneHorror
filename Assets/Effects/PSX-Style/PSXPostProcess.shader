Shader "Unlit/PSXPostProcess"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _XPixelSize("PixelSizeX", float) = 0.02
        _YPixelSize("PixelSizeY", float) = 0.02
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
            float _XPixelSize;
            float _YPixelSize;

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
                f.x = floor(f.x / _XPixelSize) * _XPixelSize;
                f.y = floor(f.y / _YPixelSize) * _YPixelSize;
                fixed4 col = tex2D(_MainTex, f);
                //col -= 0.1;
                //col.r = 1;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
