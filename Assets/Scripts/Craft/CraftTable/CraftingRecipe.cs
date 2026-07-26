using UnityEngine;
using System.Collections.Generic;

public enum RecipeType { CurarVida, DesbloquearHerramienta, MejorarHerramienta }

[CreateAssetMenu(fileName = "NuevaReceta", menuName = "Crafting/Receta")]
public class CraftingRecipe : ScriptableObject
{
    public string nombreReceta;
    public Sprite icono;
    public RecipeType tipoReceta;

    [Header("Configuracion de Herramienta")]
    public string toolName;

    [Header("Configuracion de Vida")]
    public int cantidadCuracion = 30;

    [Header("Costos de Recursos")]
    public List<ResourceCost> costos = new List<ResourceCost>();
}