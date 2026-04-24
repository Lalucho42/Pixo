using UnityEngine;
using System.Collections.Generic;

public class GrassDecorator : MonoBehaviour
{
    [Header("Modelos de Vegetación")]
    public GameObject[] grassPrefabs;

    [Header("Configuración de Esparcido")]
    public int grassDensity = 25;

    // --- CAMBIO CLAVE: Vector2 para Ancho (X) y Largo (Y) ---
    [Tooltip("X = Ancho, Y = Largo del área de generación")]
    public Vector2 areaSize = new Vector2(10f, 10f);

    [Tooltip("Control total de posición: X (Lados), Y (Altura), Z (Frente/Atrás)")]
    public Vector3 spawnOffset = new Vector3(0, 0.5f, 0);

    [Header("Variación Estética")]
    public float minScale = 0.8f;
    public float maxScale = 1.5f;
    public bool randomRotation = true;

    [Header("Visualización (Solo Editor)")]
    public Color gizmoColor = new Color(0, 1, 0, 0.3f);

    [ContextMenu("Generar Césped")]
    public void GenerateGrass()
    {
        ClearGrass();

        if (grassPrefabs == null || grassPrefabs.Length == 0) return;

        for (int i = 0; i < grassDensity; i++)
        {
            GameObject prefabToSpawn = grassPrefabs[Random.Range(0, grassPrefabs.Length)];

            // Calculamos posición aleatoria dentro del rectángulo a medida
            float posX = Random.Range(-areaSize.x / 2f, areaSize.x / 2f);
            float posZ = Random.Range(-areaSize.y / 2f, areaSize.y / 2f);

            // Posición local relativa al objeto (ahora rectangular)
            Vector3 localPos = spawnOffset + new Vector3(posX, 0, posZ);

            // Convertimos a posición de mundo real
            Vector3 worldPos = transform.TransformPoint(localPos);

            GameObject grass = Instantiate(prefabToSpawn, worldPos, transform.rotation, transform);
            grass.name = "Grass_Generated";

            if (randomRotation)
                grass.transform.Rotate(Vector3.up, Random.Range(0, 360f));

            float scale = Random.Range(minScale, maxScale);
            grass.transform.localScale = new Vector3(scale, scale, scale);
        }
    }

    [ContextMenu("Limpiar Césped")]
    public void ClearGrass()
    {
        List<GameObject> toDelete = new List<GameObject>();
        foreach (Transform child in transform)
        {
            if (child.name == "Grass_Generated") toDelete.Add(child.gameObject);
        }
        foreach (GameObject obj in toDelete) DestroyImmediate(obj);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.matrix = transform.localToWorldMatrix;

        // El Gizmo ahora dibuja el rectángulo usando X y Y de areaSize
        Vector3 size = new Vector3(areaSize.x, 0.05f, areaSize.y);

        Gizmos.DrawCube(spawnOffset, size);
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 1f);
        Gizmos.DrawWireCube(spawnOffset, size);
    }
}