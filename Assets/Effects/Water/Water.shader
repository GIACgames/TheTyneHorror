Shader "Custom/Water" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _ScumTex ("Base (RGB)", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _SpecularColor ("Specular Color", Color) = (1,1,1,1)
        _Shininess ("Shininess", Range(0.01, 1)) = 0.078125
        _DepthStrength("Depth Strength", Range(0.0, 600.0)) = 0.2
        _WaveSpeed("Wave Speed", Range(0.0, 1.0)) = 0.2
        _WobblesPerSecond("Wobbles Per Second", Range(0, 100)) = 1
        _WobbleAmplitude("Wobble Amplitude", Range(0.0, 1.0)) = 0.5
        _WobblePeriod("Wobble Period", Range(0.0, 10.0)) = 1
        _FrothIntensity("Froth Intensity", Range(0.0, 1.0)) = 0.3
    }
 
    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Opaque"}
        LOD 100
 
        CGPROGRAM
        #pragma surface surf Standard
        #pragma target 3.0
        #include "UnityCG.cginc"
        
        sampler2D _MainTex;
        sampler2D _BumpMap;
        float _Shininess;
        float _DepthStrength;
        float _WaveSpeed;
        float _WobblesPerSecond;
        float _WobbleAmplitude;
        float _WobblePeriod;
        float _FrothIntensity;
        sampler2D _ScumTex;
        #define TWO_PI 6.28318
        
        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
            float3 worldNormal;
        };
 
        void surf (Input IN, inout SurfaceOutputStandard o) {
		// Depth
            float depth = (IN.worldPos.y * _DepthStrength) - 0.1;
            float wave = sin(_Time.y * _WaveSpeed * 10.0) * sin(_Time.y * _WaveSpeed  * 10.0);  //+ IN.worldPos.x, + IN.worldPos.z
            //o.Alpha = 1.0 - depth - wave;
		
            // Base texture
            float2 f = IN.uv_MainTex;
            f.x = (IN.worldPos.x * 0.04) % 1; f.y = (IN.worldPos.z * 0.04) % 1;
            if (f.x < 0) {f.x = 1 + f.x;} if (f.y < 0) {f.y = 1 + f.y;}
            f.x = (f.x + (_Time * _WaveSpeed)) % 1;
            //f.x = (f.x + IN.worldPos.z) % 1;// f.y = (f.y + IN.worldPos.z) % 1;

            float offsetInput = (_Time.y + frac(f.x)) * _WobblesPerSecond * TWO_PI + f.x / _WobblePeriod;
            f.y += sin(offsetInput) * _WobbleAmplitude * 2;
            f.x += cos(offsetInput) * _WobbleAmplitude * 2;
            //f.x = f.x % 1; f.y = f.y % 1;
            o.Albedo = tex2D(_MainTex, f).rgb;


            f.y += sin(offsetInput) * _WobbleAmplitude;
            f.x += cos(offsetInput) * _WobbleAmplitude;
            //f.x = f.x % 1; f.y = f.y % 1;
            float4 scum = tex2D(_ScumTex, f).rgba;
            //if (depth > 0) {o.Albedo += depth;}
            if (scum.a > (1 - _FrothIntensity) + (0.1 * (1 + (sin(_Time.y * _WobblesPerSecond * 5 * (_FrothIntensity * _FrothIntensity)))))) {o.Albedo += scum * scum.a * 0.8;o.Normal = 5 * scum.a;}
            else {o.Albedo += scum * scum.a * 0.3; }

            f.x = (f.x + (0.03 * sin(_Time.y * 1))) % 1; f.y = (f.y + (0.02 * sin(_Time.y * 2))) % 1;
            o.Normal = UnpackNormal(tex2D(_BumpMap, f));

            // Normal mapping
            //o.Normal = UnpackNormal(tex2D(_BumpMap, f));
           if (depth > 0.3) {//o.Albedo += depth; 
                o.Normal += ((depth - 0.3) * 0.5);
            }
            
            
            // Specular settings
            //o.Specular = _SpecularColor.rgb;
            o.Smoothness = _Shininess;
            
            
            
            // Lighting
            o.Emission = float3(0.0, 0.0, 0.0);
            o.Metallic = 0.0;
            o.Occlusion = 1.0;
            //o.AlphaClip = 0.0;
        }
        ENDCG
    }
 
    FallBack "Diffuse"
}
