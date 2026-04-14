using UnityEngine;
using System.Collections.Generic;

public class DynamicInventoryUI : MonoBehaviour
{
    public Player player;
    public GameObject resourceSlotPrefab;
    public Transform contenedor;

    [Header("Iconos de Recursos")]
    public Sprite iconoMadera;
    public Sprite iconoPiedra;

    private Dictionary<ResourceType, ResourceSlotUI> slotsCreados = new Dictionary<ResourceType, ResourceSlotUI>();

    private void Start()
    {
        if (player == null) player = FindFirstObjectByType<Player>();
        player.Inventory.OnInventoryChanged += ActualizarInventario;

        foreach (Transform child in contenedor) Destroy(child.gameObject);
    }

    private void ActualizarInventario(int madera, int piedra)
    {
        CheckResource(ResourceType.Madera, madera);
        CheckResource(ResourceType.Piedra, piedra);
    }

    private void CheckResource(ResourceType tipo, int cantidad)
    {
        if (cantidad > 0)
        {
            if (!slotsCreados.ContainsKey(tipo))
            {
                GameObject nuevo = Instantiate(resourceSlotPrefab, contenedor);
                nuevo.transform.localScale = Vector3.one;

                ResourceSlotUI script = nuevo.GetComponent<ResourceSlotUI>();
                slotsCreados.Add(tipo, script);
            }

            Sprite icono = GetSpriteForResource(tipo);
            slotsCreados[tipo].Configurar(icono, cantidad);
        }
        else if (slotsCreados.ContainsKey(tipo))
        {
            Destroy(slotsCreados[tipo].gameObject);
            slotsCreados.Remove(tipo);
        }
    }

    private Sprite GetSpriteForResource(ResourceType tipo)
    {
        if (tipo == ResourceType.Madera) return iconoMadera;
        if (tipo == ResourceType.Piedra) return iconoPiedra;
        return null;
    }
}