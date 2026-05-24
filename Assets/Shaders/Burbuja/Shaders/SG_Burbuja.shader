Shader "Custom/SG_Burbuja"
{
    Properties
    {
        _Color ("Color de la Burbuja", Color) = (0.8, 0.9, 1.0, 0.1)
        _FresnelPower ("Brillo del Borde (Fresnel)", Range(1.0, 10.0)) = 3.0
        _RimColor ("Color del Borde", Color) = (1.0, 1.0, 1.0, 1.0)
        _NoiseTex ("Ruido de Deformacion (Gris)", 2D) = "white" {}
        _Speed ("Velocidad del Movimiento", Range(0.0, 5.0)) = 0.5
        _Distortion ("Fuerza de Deformacion", Range(0.0, 0.2)) = 0.05
    }
    SubShader
    {
        // Indicamos que es un objeto transparente y que se renderiza después de la geometría opaca
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        // Configuramos la mezcla para que sea transparente (Alpha Blending)
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        CGPROGRAM
        // Usamos el modelo de iluminación Lambert con soporte para transparencia (alpha)
        // También usamos una función de modificación de vértices (vert) para hacer que "tiemble"
        #pragma surface surf Lambert alpha vertex:vert

        #pragma target 3.0

        sampler2D _NoiseTex;
        float4 _Color;
        float _FresnelPower;
        float4 _RimColor;
        float _Speed;
        float _Distortion;

        struct Input
        {
            float3 viewDir;     // Dirección en la que la cámara está mirando al píxel
            float3 worldNormal; // Orientación del píxel en el mundo 3D
            float2 uv_NoiseTex; // Coordenadas de nuestra textura de ruido
        };

        // --- DEFORMACIÓN DE VÉRTICES (Paso 10 de ChatGPT) ---
        // Aquí usamos tu textura gris para deformar ligeramente la esfera y hacer que "tiemble"
        void vert(inout appdata_full v)
        {
            // Calculamos una coordenada de textura que se mueve con el tiempo
            float2 movingUV = v.texcoord.xy + float2(_Time.y * _Speed, _Time.y * _Speed);
            
            // Leemos el valor gris de tu textura usando las coordenadas en movimiento
            float4 noise = tex2Dlod(_NoiseTex, float4(movingUV, 0, 0));
            
            // Desplazamos los vértices hacia afuera basándonos en la textura gris
            v.vertex.xyz += v.normal * (noise.r - 0.5) * _Distortion;
        }

        // --- PÍXELES, TRANSPARENCIA Y REFLEJO (Pasos 7, 8 y 9) ---
        void surf (Input IN, inout SurfaceOutput o)
        {
            // 1. Color Base (Súper transparente en el centro)
            o.Albedo = _Color.rgb;

            // 2. EFECTO FRESNEL (La magia del jabón)
            // Calculamos qué tan de frente miramos cada parte de la esfera.
            // Si miramos el centro de frente, "dotProduct" es cercano a 1. Si miramos el borde de reojo, es cercano a 0.
            half dotProduct = 1.0 - saturate(dot(normalize(IN.viewDir), normalize(IN.worldNormal)));
            
            // Elevamos el resultado para afilar el borde brillante
            half fresnel = pow(dotProduct, _FresnelPower);

            // 3. COLOR FINAL Y TRANSPARENCIA
            // Sumamos el brillo del borde (RimColor) al color de fondo
            o.Emission = _RimColor.rgb * fresnel;
            
            // El centro es casi transparente (_Color.a) pero el borde es completamente visible (fresnel)
            o.Alpha = saturate(_Color.a + fresnel);
        }
        ENDCG
    }
    FallBack "Diffuse"
}