using UnityEngine;
using System; 

public class BasePuzzleModule : MonoBehaviour
{
    
    public Action<bool> AlTerminarElPuzzle;

    
    public virtual void StartPuzzle()
    {
        Debug.Log("Aqui empezara el minijuego de mi amigo.");
    }

    
    public void EnviarResultadoAlCerebro(bool seCompletoConExito)
    {
        if (AlTerminarElPuzzle != null)
        {
            AlTerminarElPuzzle.Invoke(seCompletoConExito);
        }
    }



}