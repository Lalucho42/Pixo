using UnityEngine;
using System.Collections;

public class ComplexRepairableStructure : MonoBehaviour
{
    public GameObject modeloDeLosEscombros;
    public GameObject modeloDeLaEstructuraReparada;
    public GameObject paredInvisible;

    public void TriggerRepair()
    {
        StartCoroutine(EfectoCaida());
    }

    IEnumerator EfectoCaida()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX2D("Estructura_Reparada");
        }

        if (modeloDeLosEscombros != null) modeloDeLosEscombros.SetActive(false);

        if (modeloDeLaEstructuraReparada != null)
        {
            Vector3 posFinal = modeloDeLaEstructuraReparada.transform.localPosition;
            modeloDeLaEstructuraReparada.transform.localPosition = posFinal + new Vector3(0, 15, 0);
            modeloDeLaEstructuraReparada.SetActive(true);

            float progreso = 0;
            while (progreso < 1.0f)
            {
                progreso = progreso + Time.deltaTime * 3.5f;
                modeloDeLaEstructuraReparada.transform.localPosition = Vector3.Lerp(modeloDeLaEstructuraReparada.transform.localPosition, posFinal, progreso);
                yield return null;
            }
            modeloDeLaEstructuraReparada.transform.localPosition = posFinal;
        }

        if (paredInvisible != null) paredInvisible.SetActive(false);
    }
}