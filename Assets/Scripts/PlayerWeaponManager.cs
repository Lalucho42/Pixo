using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    [Header("Settings")]
    public string boneName = "mixamorig:RightHandMiddle1";
    private Transform targetHandBone;
    private GameObject currentWeaponInHand;

    public bool HasWeapon => currentWeaponInHand != null;

    private void Start()
    {
        targetHandBone = FindBoneRecursive(transform, boneName);
    }

    public void EquipWeapon(GameObject weaponPrefab)
    {
        if (targetHandBone == null) return;
        if (weaponPrefab == null) return;

        if (currentWeaponInHand != null) Destroy(currentWeaponInHand);

        currentWeaponInHand = Instantiate(weaponPrefab);
        
        Collider col = currentWeaponInHand.GetComponent<Collider>();
        if (col != null) Destroy(col);

        currentWeaponInHand.transform.SetParent(targetHandBone, true);
        
        currentWeaponInHand.transform.localPosition = Vector3.zero;
        currentWeaponInHand.transform.localRotation = Quaternion.identity;
        
        WeaponPickup pickup = currentWeaponInHand.GetComponent<WeaponPickup>();
        if (pickup != null) Destroy(pickup);
    }

    public void ActivateWeaponByName(string weaponName)
    {
        if (targetHandBone == null)
            targetHandBone = FindBoneRecursive(transform, boneName);

        if (targetHandBone == null) return;

        Transform weaponTransform = FindRecursive(targetHandBone, weaponName);
        if (weaponTransform != null)
        {
            weaponTransform.gameObject.SetActive(true);
            currentWeaponInHand = weaponTransform.gameObject;
        }
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
