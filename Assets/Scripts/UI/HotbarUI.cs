using System.Collections.Generic;
using UnityEngine;

public class HotbarUI : MonoBehaviour
{
    public Player player;
    public GameObject slotPrefab;
    public Transform contenedor;

    private List<WeaponSlotUI> slotsActivos = new List<WeaponSlotUI>();

    private void Start()
    {
        if (player == null) player = Object.FindFirstObjectByType<Player>();

        player.WeaponManager.OnWeaponAdded += AgregarNuevoSlot;
        player.WeaponManager.OnWeaponSwitched += ActualizarSeleccion;
    }

    private void AgregarNuevoSlot(ToolItem nuevaHerramienta)
    {
        GameObject nuevoObj = Instantiate(slotPrefab, contenedor);
        WeaponSlotUI slotScript = nuevoObj.GetComponent<WeaponSlotUI>();
        slotScript.Configurar(nuevaHerramienta);
        slotsActivos.Add(slotScript);
    }

    private void ActualizarSeleccion(int indexSeleccionado)
    {
        for (int i = 0; i < slotsActivos.Count; i++)
        {
            slotsActivos[i].SetSeleccionado(i == indexSeleccionado);
        }
    }
}