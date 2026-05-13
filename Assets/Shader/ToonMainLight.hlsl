// ----------------------------------------------------------------------------------
// Archivo: ToonMainLight.hlsl
// Descripción: Extractor de la luz principal con apagado automático nocturno.
// ----------------------------------------------------------------------------------

#ifndef TOON_MAIN_LIGHT_INCLUDED
#define TOON_MAIN_LIGHT_INCLUDED

/* INSTRUCCIONES PARA EL NODO CUSTOM FUNCTION:
    - Type: File
    - Name: GetMainLightData
    
    Inputs (+):
    1. WorldPos (Vector 3)
    
    Outputs (+):
    1. Direction (Vector 3)
    2. Color (Vector 3)
    3. Shadow (Float)
*/

void GetMainLightData_float(float3 WorldPos, out float3 Direction, out float3 Color, out float Shadow)
{
    // 1. Valores por defecto para que la ventanita de Shader Graph no dé error
    Direction = float3(0.5, 0.5, 0);
    Color = float3(1, 1, 1);
    Shadow = 1.0;

    // 2. Ejecución real dentro del motor de Unity
#ifndef SHADERGRAPH_PREVIEW
        
        // Importamos la librería de iluminación de URP
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        
        // Convertimos la posición del mundo en coordenadas de sombra
    float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
        
        // Obtenemos los datos de la luz principal
    Light mainLight = GetMainLight(shadowCoord);
        
    Direction = mainLight.direction;
        
        // -----------------------------------------------------------
        // TRUCO DÍA/NOCHE (Apagado automático debajo del horizonte)
        // -----------------------------------------------------------
        // La dirección 'Y' de la luz indica su altura en el mundo.
        // smoothstep(0.0, 0.1, Y) mantiene la luz al 100% mientras esté arriba,
        // y la apaga suavemente (atardecer) justo al cruzar la línea del horizonte.
    float horizonFade = smoothstep(0.0, 0.1, mainLight.direction.y);
        
        // Multiplicamos el color del sol por nuestro atenuador del horizonte
    Color = mainLight.color * horizonFade;
        
    Shadow = mainLight.shadowAttenuation;
        
#endif
}

#endif // TOON_MAIN_LIGHT_INCLUDED