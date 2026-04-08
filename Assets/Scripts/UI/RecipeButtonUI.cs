using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeButtonUI : MonoBehaviour
{
    public TextMeshProUGUI textoNombre;
    public Transform contenedorSlots; // Acá se clonarán los íconos
    public Button botonAccion;
    public GameObject slotPrefab;     // Tu famoso Slot_Costo

    public void Configurar(CraftingRecipe receta, CraftingUI manager)
    {
        // 1. Ponemos el título (Ej: "Medikit")
        if (textoNombre != null) textoNombre.text = receta.nombreReceta;

        // 2. Limpiamos basura vieja
        foreach (Transform child in contenedorSlots) Destroy(child.gameObject);

        // 3. Clonamos un ícono por cada material que exija la receta
        foreach (ResourceCost costo in receta.costos)
        {
            GameObject nuevoSlot = Instantiate(slotPrefab, contenedorSlots);
            ResourceSlotUI slotScript = nuevoSlot.GetComponent<ResourceSlotUI>();
            if (slotScript != null)
            {
                slotScript.Configurar(costo.icono, costo.cantidad);
            }
        }

        // 4. Conectamos el clic del botón a la receta correcta
        botonAccion.onClick.RemoveAllListeners();
        botonAccion.onClick.AddListener(() => manager.IntentarCraftear(receta));
    }
}