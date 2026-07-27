using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingSlotUI : MonoBehaviour
{
    [Header("Datos de la Receta")]
    public CraftingRecipe receta;

    [Header("Referencias de UI Principal (TMP)")]
    public Image iconoItem;
    public TextMeshProUGUI nombreTexto;
    public Button botonCraftear;

    [Header("Contenedor Visual de Costos")]
    public Transform contenedorCostos;
    public GameObject costoPrefab;

    private void Awake()
    {
        if (botonCraftear == null)
        {
            botonCraftear = GetComponent<Button>();
        }
    }

    private void Start()
    {
        if (botonCraftear != null)
        {
            botonCraftear.onClick.RemoveAllListeners();
            botonCraftear.onClick.AddListener(AlHacerClic);
        }
    }

    public void ConfigurarYActualizar(Player player)
    {
        if (botonCraftear == null)
        {
            botonCraftear = GetComponent<Button>();
        }

        if (receta == null) return;

        if (iconoItem != null && receta.icono != null)
        {
            iconoItem.sprite = receta.icono;
            iconoItem.preserveAspect = true; 
            iconoItem.gameObject.SetActive(true);
        }

        if (nombreTexto != null)
        {
            nombreTexto.text = receta.nombreReceta;
        }

        if (contenedorCostos != null && costoPrefab != null)
        {
            foreach (Transform child in contenedorCostos)
            {
                Destroy(child.gameObject);
            }

            foreach (ResourceCost costo in receta.costos)
            {
                GameObject nuevoCostoObj = Instantiate(costoPrefab, contenedorCostos);
                ResourceSlotUI uiCosto = nuevoCostoObj.GetComponent<ResourceSlotUI>();

                bool tieneSuficiente = player != null && player.Inventory.HasResource(costo.tipoRecurso, costo.cantidad);

                if (uiCosto != null)
                {
                    uiCosto.Configurar(costo.icono, costo.cantidad, tieneSuficiente);
                }
            }
        }

        if (player == null) return;

        bool poseesHerramienta = false;
        bool estaMejorada = false;

        if (player.WeaponManager != null)
        {
            ToolItem herramienta = player.WeaponManager.unlockedWeapons.Find(t => t.toolName == receta.toolName);
            if (herramienta != null)
            {
                poseesHerramienta = true;
                estaMejorada = herramienta.estaMejorada;
            }
        }

        if (receta.tipoReceta == RecipeType.DesbloquearHerramienta && poseesHerramienta)
        {
            if (botonCraftear != null) botonCraftear.interactable = false;
            return;
        }

        if (receta.tipoReceta == RecipeType.MejorarHerramienta)
        {
            if (!poseesHerramienta || estaMejorada)
            {
                if (botonCraftear != null) botonCraftear.interactable = false;
                return;
            }
        }

        bool tieneRecursosTotal = true;
        foreach (ResourceCost costo in receta.costos)
        {
            if (!player.Inventory.HasResource(costo.tipoRecurso, costo.cantidad))
            {
                tieneRecursosTotal = false;
                break;
            }
        }

        if (botonCraftear != null)
        {
            botonCraftear.interactable = tieneRecursosTotal;
        }
    }

    private void AlHacerClic()
    {
        if (receta != null && CraftingUI.Instance != null)
        {
            if (CraftingUI.Instance.IntentarCraftear(receta))
            {
                CraftingUI.Instance.RefrescarMenu();
            }
        }
    }
}