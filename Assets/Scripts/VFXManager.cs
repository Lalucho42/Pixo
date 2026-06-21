using UnityEngine;
using System;

public class VFXManager : MonoBehaviour
{
    [System.Serializable]
    public class VFXElement
    {
        public string nombreID;
        public GameObject prefabParticula;
        public float tiempoDeVida = 2f;
    }

    public static VFXManager Instance;

    [Header("Base de Datos de Particulas")]
    public VFXElement[] baseDeDatosVFX;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SpawnVFX(string nombreID, Vector3 posicionImpacto, Quaternion rotacion)
    {
        if (baseDeDatosVFX == null) return;

        VFXElement vfx = Array.Find(baseDeDatosVFX, element => element.nombreID == nombreID);

        if (vfx == null || vfx.prefabParticula == null) return;

        GameObject nuevaParticula = Instantiate(vfx.prefabParticula, posicionImpacto, rotacion);
        Destroy(nuevaParticula, vfx.tiempoDeVida);
    }
}