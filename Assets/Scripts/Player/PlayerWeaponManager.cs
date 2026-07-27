using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    [Header("Settings")]
    public string boneName = "mixamorig:RightHandMiddle1";
    private Transform targetHandBone;

    public ToolItem CurrentTool { get; private set; }

    public bool HasWeapon { get { return CurrentTool != null; } }

    public List<ToolItem> unlockedWeapons = new List<ToolItem>();
    private int currentWeaponIndex = -1;

    public event Action<ToolItem> OnWeaponAdded;
    public event Action<int> OnWeaponSwitched;

    private void Start()
    {
        targetHandBone = FindBoneRecursive(transform, boneName);
    }

    public void ActivateWeaponByName(string weaponName)
    {
        ToolItem[] allTools = GetComponentsInChildren<ToolItem>(true);
        ToolItem foundTool = null;

        foreach (ToolItem tool in allTools)
        {
            if (tool.toolName.Equals(weaponName, StringComparison.OrdinalIgnoreCase) ||
                tool.gameObject.name.Equals(weaponName, StringComparison.OrdinalIgnoreCase))
            {
                foundTool = tool;
                break;
            }
        }

        if (foundTool != null)
        {
            if (!unlockedWeapons.Contains(foundTool))
            {
                unlockedWeapons.Add(foundTool);
                if (OnWeaponAdded != null) OnWeaponAdded.Invoke(foundTool);
            }

            int index = unlockedWeapons.IndexOf(foundTool);
            EquipToolFromList(index);
        }
        else
        {
            Debug.LogError($"[PlayerWeaponManager] No se encontro ninguna herramienta con el nombre o GameObject: '{weaponName}' dentro del Player.");
        }
    }

    public void CycleWeapon(float scrollDirection)
    {
        if (unlockedWeapons.Count <= 1) return;

        int newIndex = currentWeaponIndex;

        if (scrollDirection > 0f)
        {
            newIndex++;
            if (newIndex >= unlockedWeapons.Count) newIndex = 0;
        }
        else if (scrollDirection < 0f)
        {
            newIndex--;
            if (newIndex < 0) newIndex = unlockedWeapons.Count - 1;
        }

        EquipToolFromList(newIndex);
    }

    private void EquipToolFromList(int index)
    {
        if (index < 0 || index >= unlockedWeapons.Count) return;

        if (CurrentTool != null)
        {
            CurrentTool.gameObject.SetActive(false);
            CurrentTool.OnUnequip();
        }

        currentWeaponIndex = index;
        CurrentTool = unlockedWeapons[currentWeaponIndex];
        CurrentTool.gameObject.SetActive(true);
        CurrentTool.OnEquip();

        if (OnWeaponSwitched != null) OnWeaponSwitched.Invoke(currentWeaponIndex);
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