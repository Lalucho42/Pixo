using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class RecipeButtonUI : MonoBehaviour
{
    public TextMeshProUGUI textoNombre;
    public Transform contenedorSlots;
    public Button botonAccion;
    public GameObject slotPrefab;

    public void Configurar(CraftingRecipe receta, CraftingUI manager)
    {
        if (textoNombre != null) textoNombre.text = receta.nombreReceta;
        foreach (Transform child in contenedorSlots) Destroy(child.gameObject);

        bool puedeComprar = true;
        List<ResourceCost> costosAMostrar = receta.costos;

        if (receta.accion == CraftingActionType.MejorarArma || receta.accion == CraftingActionType.RepararArma)
        {
            ToolItem arma = manager.ObtenerJugador().Crafting.ObtenerArma(receta.nombreArmaObjetivo);

            if (arma == null)
            {
                if (textoNombre != null) textoNombre.text = receta.nombreReceta + " (BLOQUEADA)";
                puedeComprar = false;
                costosAMostrar = new List<ResourceCost>();
            }
            else
            {
                if (receta.accion == CraftingActionType.MejorarArma && arma.estaMejorada)
                {
                    if (textoNombre != null) textoNombre.text = receta.nombreReceta + " (MAXIMA)";
                    puedeComprar = false;
                    costosAMostrar = new List<ResourceCost>();
                }
                else if (receta.accion == CraftingActionType.RepararArma)
                {
                    if (arma.usosActuales >= arma.usosMaximos)
                    {
                        if (textoNombre != null) textoNombre.text = receta.nombreReceta + " (NUEVA)";
                        puedeComprar = false;
                        costosAMostrar = new List<ResourceCost>();
                    }
                    else
                    {
                        costosAMostrar = manager.ObtenerJugador().Crafting.CalcularCostoReparacion(receta.costos, arma);
                    }
                }
            }
        }

        foreach (ResourceCost costo in costosAMostrar)
        {
            if (costo.cantidad <= 0) continue;
            GameObject nuevoSlot = Instantiate(slotPrefab, contenedorSlots);
            nuevoSlot.transform.localScale = Vector3.one;
            nuevoSlot.GetComponent<ResourceSlotUI>()?.Configurar(costo.icono, costo.cantidad);
        }

        botonAccion.interactable = puedeComprar;
        botonAccion.onClick.RemoveAllListeners();
        botonAccion.onClick.AddListener(() => manager.EjecutarAccion(receta));
    }
}