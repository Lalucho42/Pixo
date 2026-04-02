using System.Collections.Generic; // Necesario para usar Listas
using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    [Header("Settings")]
    public string boneName = "mixamorig:RightHandMiddle1";
    private Transform targetHandBone;

    public ToolItem CurrentTool { get; private set; }

    public bool HasWeapon
    {
        get { return CurrentTool != null; }
    }

    // --- NUEVO: Inventario dinámico ---
    private List<ToolItem> unlockedWeapons = new List<ToolItem>();
    private int currentWeaponIndex = -1;

    private void Start()
    {
        targetHandBone = FindBoneRecursive(transform, boneName);
    }

    public void ActivateWeaponByName(string weaponName)
    {
        if (targetHandBone == null) targetHandBone = FindBoneRecursive(transform, boneName);
        if (targetHandBone == null) return;

        Transform weaponTransform = FindRecursive(targetHandBone, weaponName);
        if (weaponTransform != null)
        {
            ToolItem newTool = weaponTransform.GetComponent<ToolItem>();
            if (newTool != null)
            {
                // Si no teníamos esta arma en la lista, la agregamos al inventario
                if (unlockedWeapons.Contains(newTool) == false)
                {
                    unlockedWeapons.Add(newTool);
                }

                // Equipamos esa arma buscando en qué posición de la lista quedó
                EquipToolFromList(unlockedWeapons.IndexOf(newTool));
            }
        }
    }

    // --- NUEVO: Cambiar de arma con la rueda ---
    public void CycleWeapon(float scrollDirection)
    {
        // Si tenemos 0 o 1 arma, no hay nada que cambiar
        if (unlockedWeapons.Count <= 1) return;

        int newIndex = currentWeaponIndex;

        if (scrollDirection > 0f) // Rueda hacia arriba
        {
            newIndex = newIndex + 1;
            // Si pasamos el límite, volvemos a la primera
            if (newIndex >= unlockedWeapons.Count)
            {
                newIndex = 0;
            }
        }
        else if (scrollDirection < 0f) // Rueda hacia abajo
        {
            newIndex = newIndex - 1;
            // Si bajamos de cero, vamos a la última
            if (newIndex < 0)
            {
                newIndex = unlockedWeapons.Count - 1;
            }
        }

        EquipToolFromList(newIndex);
    }

    private void EquipToolFromList(int index)
    {
        if (index < 0 || index >= unlockedWeapons.Count) return;

        // Apagamos la que teníamos en la mano
        if (CurrentTool != null)
        {
            CurrentTool.gameObject.SetActive(false);
            CurrentTool.OnUnequip();
        }

        // Prendemos la nueva
        currentWeaponIndex = index;
        CurrentTool = unlockedWeapons[currentWeaponIndex];
        CurrentTool.gameObject.SetActive(true);
        CurrentTool.OnEquip();
    }

    // Tus buscadores de huesos (no cambian)
    private Transform FindRecursive(Transform parent, string name)
    {
        if (parent.name == name) return parent;
        foreach (Transform child in parent)
        {
            Transform found = FindRecursive(child, name);
            if (found != null) return found;
        }
        return null;
    }

    private Transform FindBoneRecursive(Transform parent, string name)
    {
        if (parent.name == name) return parent;
        foreach (Transform child in parent)
        {
            Transform found = FindBoneRecursive(child, name);
            if (found != null) return found;
        }
        return null;
    }
}