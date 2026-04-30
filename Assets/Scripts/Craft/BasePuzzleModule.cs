using UnityEngine;
using System; // Esto es necesario para usar Action

public class BasePuzzleModule : MonoBehaviour
{
    // Esto es el "Aviso". Cuando el puzzle termine, llamara a este aviso.
    // El "bool" que tiene adentro es para decir si salio Bien (true) o Mal (false).
    public Action<bool> AlTerminarElPuzzle;

    // Tu amigo escribira aqui el codigo para empezar su puzzle
    public virtual void StartPuzzle()
    {
        Debug.Log("Aqui empezara el minijuego de mi amigo.");
    }

    // Tu amigo tendra que llamar a esta funcion desde su codigo cuando gane o pierda
    public void EnviarResultadoAlCerebro(bool seCompletoConExito)
    {
        if (AlTerminarElPuzzle != null)
        {
            AlTerminarElPuzzle.Invoke(seCompletoConExito);
        }
    }
}