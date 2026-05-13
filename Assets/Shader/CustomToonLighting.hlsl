// ----------------------------------------------------------------------------------
// Archivo: CustomToonLighting.hlsl
// Descripción: Función con "Máscara de Luz" y múltiples Anillos (Bands) para luces.
// ----------------------------------------------------------------------------------

#ifndef CUSTOM_TOON_LIGHTING_INCLUDED
#define CUSTOM_TOON_LIGHTING_INCLUDED

/* INSTRUCCIONES ACTUALIZADAS:
    Debes actualizar los Inputs de tu nodo Custom Function en Shader Graph:
    
    Inputs (+):
    1. WorldPos (Vector 3)
    2. WorldNormal (Vector 3)
    3. ToonBands (Float)       <-- ¡NUEVO! Cantidad de anillos (Ej: 3)
    4. ToonSmoothness (Float)  <-- Suavizado de los anillos (Ej: 0.05)
    5. LightMultiplier (Float) <-- ¡NUEVO! Multiplicador para agrandar la luz (Ej: 2.0)
    
    Outputs (+):
    1. Diffuse (Vector 3)
    2. LightMask (Float)
*/

void CalculateAdditionalLights_float(
    float3 WorldPos,
    float3 WorldNormal,
    float ToonBands, // Cantidad de anillos/escalones
    float ToonSmoothness, // Suavizado entre anillos
    float LightMultiplier, // Agrandar/achicar el alcance de la luz
    out float3 Diffuse,
    out float LightMask)
{
    Diffuse = float3(0, 0, 0);
    LightMask = 0.0;

#ifndef SHADERGRAPH_PREVIEW
        
#ifndef _ADDITIONAL_LIGHTS
#define _ADDITIONAL_LIGHTS
#endif

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

    InputData inputData = (InputData) 0;
    inputData.positionWS = WorldPos;
    float4 clipPos = TransformWorldToHClip(WorldPos);
    float4 screenPos = ComputeScreenPos(clipPos);
    inputData.normalizedScreenSpaceUV = screenPos.xy / max(screenPos.w, 0.0001);

    uint lightCount = GetAdditionalLightsCount();

    LIGHT_LOOP_BEGIN(lightCount)
            
    Light light = GetAdditionalLight(lightIndex, WorldPos, half4(1, 1, 1, 1));
            
    float distanceAttenuation = light.distanceAttenuation;
    float shadowAttenuation = light.shadowAttenuation;
            
    float NdotL = saturate(dot(WorldNormal, light.direction));
            
            // Calculamos la fuerza bruta y la multiplicamos para controlar su tamaño
    float combinedIntensity = saturate(NdotL * distanceAttenuation * shadowAttenuation * LightMultiplier);
            
            // -----------------------------------------------------------
            // GENERADOR MATEMÁTICO DE ANILLOS (GRADIENTE TOON)
            // -----------------------------------------------------------
            // Multiplicamos la intensidad por la cantidad de bandas/anillos
    float stepped = combinedIntensity * max(1.0, ToonBands);
            
    float base = floor(stepped); // Parte entera (0, 1, 2...)
    float remainder = frac(stepped); // Parte decimal (0.0 a 1.0)
            
            // Suavizamos el salto entre un anillo y otro para que no se vea pixelado
    float smoothed = smoothstep(0.5 - ToonSmoothness, 0.5 + ToonSmoothness, remainder);
            
            // Juntamos todo y normalizamos de vuelta al rango 0 a 1
    float toonBanding = saturate((base + smoothed) / max(1.0, ToonBands));
            // -----------------------------------------------------------
            
            // Sumamos el color de la luz con los anillos aplicados
    Diffuse += light.color * toonBanding;

            // Guardamos la fuerza máxima de la luz para usarla como máscara (y borrar sombras)
    LightMask = max(LightMask, toonBanding);

    LIGHT_LOOP_END
        
#endif
}

#endif // CUSTOM_TOON_LIGHTING_INCLUDED