using UnityEngine;
using TMPro;

public class MissionUI : MonoBehaviour
{
    public static MissionUI Instance;
    public TextMeshProUGUI textoMision;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void ActualizarMision(string nuevaMision)
    {
        if (textoMision != null)
        {
            textoMision.text = "Mission: " + nuevaMision;
        }
    }
}