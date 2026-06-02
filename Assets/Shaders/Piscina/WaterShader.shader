Shader "Custom/WaterShader"
{
    Properties
    {
        _DeepColor("Deep Water Color", Color) = (0,0.25,0.8,1)
        _ShallowColor("Shallow Water Color", Color) = (0.4,0.8,1,1)
        _FresnelColor("Fresnel Color", Color) = (1,1,1,1)
        _FresnelPower("Fresnel Power", Range(0.1,5.0)) = 2.0
        _Transparency("Transparency", Range(0,1)) = 0.8
        _Amplitude1("Amplitude1", Range(0,1)) = 0.1
        _Frequency1("Frequency1", Range(0,10)) = 3
        _Speed1("Speed1", Range(0,10)) = 1
        _Direction1("Direction1 (X,Z)", Vector) = (1,0,0,0)
        _Amplitude2("Amplitude2", Range(0,1)) = 0.06
        _Frequency2("Frequency2", Range(0,10)) = 2
        _Speed2("Speed2", Range(0,10)) = 1.3
        _Direction2("Direction2 (X,Z)", Vector) = (0,1,0,0)
        _Amplitude3("Amplitude3", Range(0,1)) = 0.08
        _Frequency3("Frequency3", Range(0,10)) = 4
        _Speed3("Speed3", Range(0,10)) = 1.8
        _Direction3("Direction3 (X,Z)", Vector) = (0.7,0.7,0,0)
        _FoamColor("Foam Color", Color) = (1,1,1,1)
        _FoamTexture("Foam Texture (R)", 2D) = "white" {}
        _FoamScale("Foam Texture Scale", Range(0,10)) = 3.0
        _FoamIntensity("Foam Intensity", Range(0,2)) = 1.0
        _SeaLevel("Base Sea Level", Float) = 0.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 clipPos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float2 uv : TEXCOORD2;
                float waveHeight : TEXCOORD3;
            };

            float4 _DeepColor, _ShallowColor, _FresnelColor;
            float _FresnelPower, _Transparency;
            float _Amplitude1, _Frequency1, _Speed1; float4 _Direction1;
            float _Amplitude2, _Frequency2, _Speed2; float4 _Direction2;
            float _Amplitude3, _Frequency3, _Speed3; float4 _Direction3;
            float4 _FoamColor;
            sampler2D _FoamTexture;
            float _FoamScale, _FoamIntensity, _SeaLevel;

            // --- FIX 1: devuelve offset Y y calcula derivada para la normal ---
            float GerstnerWaveY(float3 pos, float amplitude, float frequency, float speed, float2 direction)
            {
                float theta = dot(direction, pos.xz) * frequency + (_Time.y * speed);
                return amplitude * sin(theta);
            }

            // Tangente en X para reconstruir normal
            float GerstnerDdx(float3 pos, float amplitude, float frequency, float speed, float2 direction)
            {
                float theta = dot(direction, pos.xz) * frequency + (_Time.y * speed);
                return -amplitude * frequency * direction.x * cos(theta);
            }

            // Tangente en Z para reconstruir normal
            float GerstnerDdz(float3 pos, float amplitude, float frequency, float speed, float2 direction)
            {
                float theta = dot(direction, pos.xz) * frequency + (_Time.y * speed);
                return -amplitude * frequency * direction.y * cos(theta);
            }

            v2f vert(appdata v)
            {
                v2f o;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                float2 dir1 = normalize(_Direction1.xy);
                float2 dir2 = normalize(_Direction2.xy);
                float2 dir3 = normalize(_Direction3.xy);

                // Desplazamiento Y acumulado
                float wY = GerstnerWaveY(worldPos, _Amplitude1, _Frequency1, _Speed1, dir1)
                         + GerstnerWaveY(worldPos, _Amplitude2, _Frequency2, _Speed2, dir2)
                         + GerstnerWaveY(worldPos, _Amplitude3, _Frequency3, _Speed3, dir3);

                worldPos.y += wY;

                // --- FIX 2: reconstruir normal a partir de derivadas parciales ---
                float ddx = GerstnerDdx(worldPos, _Amplitude1, _Frequency1, _Speed1, dir1)
                          + GerstnerDdx(worldPos, _Amplitude2, _Frequency2, _Speed2, dir2)
                          + GerstnerDdx(worldPos, _Amplitude3, _Frequency3, _Speed3, dir3);

                float ddz = GerstnerDdz(worldPos, _Amplitude1, _Frequency1, _Speed1, dir1)
                          + GerstnerDdz(worldPos, _Amplitude2, _Frequency2, _Speed2, dir2)
                          + GerstnerDdz(worldPos, _Amplitude3, _Frequency3, _Speed3, dir3);

                // Normal = cross(tangent_x, tangent_z)
                float3 tangentX = normalize(float3(1.0, ddx, 0.0));
                float3 tangentZ = normalize(float3(0.0, ddz, 1.0));
                float3 waveNormal = normalize(cross(tangentZ, tangentX));

                o.waveHeight = wY; // altura relativa de la ola (sin offset de SeaLevel)
                o.clipPos = mul(UNITY_MATRIX_VP, float4(worldPos, 1.0));
                o.worldPos = worldPos;
                o.normal = waveNormal;
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 N = normalize(i.normal);
                float3 V = normalize(_WorldSpaceCameraPos - i.worldPos);

                // Fresnel
                float fresnelFactor = pow(1.0 - saturate(dot(N, V)), _FresnelPower);

                // --- FIX 3: shallowFactor basado en altura de ola, no worldPos.y absoluto ---
                float shallowFactor = saturate(i.waveHeight * 5.0 + 0.5);
                float3 waterColor = lerp(_DeepColor.rgb, _ShallowColor.rgb, shallowFactor);
                float3 finalColor = lerp(waterColor, _FresnelColor.rgb, fresnelFactor);

                // --- FIX 4: foam con umbrales ajustados para amplitudes pequeñas ---
                float foamEdge0 = 0.05;
                float foamEdge1 = 0.20;
                float foamFactorHeight = smoothstep(foamEdge0, foamEdge1, i.waveHeight);

                float slopeFactor = saturate(1.0 - dot(N, float3(0,1,0)));
                slopeFactor = pow(slopeFactor, 2.0) * 0.8;

                float2 foamUV = i.uv * _FoamScale;
                float foamTex = tex2D(_FoamTexture, foamUV).r;

                float foamFactor = saturate((foamFactorHeight + slopeFactor) * foamTex * _FoamIntensity);
                finalColor = lerp(finalColor, _FoamColor.rgb, foamFactor);

                return fixed4(finalColor, _Transparency);
            }
            ENDCG
        }
    }
    FallBack Off
}