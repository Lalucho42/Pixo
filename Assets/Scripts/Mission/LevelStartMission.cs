using UnityEngine;
using System.Collections;

public class LevelStartMission : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        if (MissionUI.Instance != null)
        {
            MissionUI.Instance.ActualizarMision("Follow the cat");
        }
    }
}