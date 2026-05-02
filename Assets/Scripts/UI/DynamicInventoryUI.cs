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
    public Sprite iconoParteComputadora; 

    private Dictionary<ResourceType, ResourceSlotUI> slotsCreados = new Dictionary<ResourceType, ResourceSlotUI>();

    private void Start()
    {
        if (player == null) player = FindFirstObjectByType<Player>();
        player.Inventory.OnInventoryChanged += ActualizarInventario;
    }

   
    private void ActualizarInventario(int madera, int piedra, int parte)
    {
        CheckResource(ResourceType.Madera, madera);
        CheckResource(ResourceType.Piedra, piedra);
        CheckResource(ResourceType.ParteComputadora, parte);
    }

    private void CheckResource(ResourceType tipo, int cantidad)
    {
        if (cantidad > 0)
        {
            if (!slotsCreados.ContainsKey(tipo))
            {
                GameObject nuevo = Instantiate(resourceSlotPrefab, contenedor);
                ResourceSlotUI script = nuevo.GetComponent<ResourceSlotUI>();
                slotsCreados.Add(tipo, script);
            }
            slotsCreados[tipo].Configurar(GetSpriteForResource(tipo), cantidad);
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
        if (tipo == ResourceType.ParteComputadora) return iconoParteComputadora;
        return null;
    }
}