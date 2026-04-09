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

    private List<ToolItem> unlockedWeapons = new List<ToolItem>();
    private int currentWeaponIndex = -1;

    public event Action<ToolItem> OnWeaponAdded;
    public event Action<int> OnWeaponSwitched;

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
                if (!unlockedWeapons.Contains(newTool))
                {
                    unlockedWeapons.Add(newTool);
                    if (OnWeaponAdded != null) OnWeaponAdded.Invoke(newTool);
                }

                EquipToolFromList(unlockedWeapons.IndexOf(newTool));
            }
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