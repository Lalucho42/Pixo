using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private void Awake()
    {
        if (GameManager.instance == null)
        {
            GameObject gmObj = new GameObject("GameManager");
            gmObj.AddComponent<GameManager>();
            DontDestroyOnLoad(gmObj);
        }
        Destroy(this);
    }
}
