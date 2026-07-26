using UnityEngine;
using UnityEngine.UI;

public class CraftingSlotUI : MonoBehaviour
{
    public CraftingRecipe receta;
    public Button botonCraftear;

    private void Start()
    {
        if (botonCraftear == null)
        {
            botonCraftear = GetComponent<Button>();
        }

        if (botonCraftear != null)
        {
            botonCraftear.onClick.AddListener(AlHacerClic);
        }
    }

    private void AlHacerClic()
    {
        if (receta != null && CraftingUI.Instance != null)
        {
            CraftingUI.Instance.IntentarCraftear(receta);
        }
    }
}