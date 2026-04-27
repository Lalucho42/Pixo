using UnityEngine;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    
    public void DeseleccionarBoton()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}