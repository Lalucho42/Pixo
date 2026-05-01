using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleScript : BasePuzzleModule
{
    [Header("Configuración del Puzzle")]
    public int toquesNecesarios = 5;
    private int toquesActuales = 0;
    private bool puzzleActivo = false;

    // Referencia directa a la acción de la tecla F
    public InputActionReference accionInteractuarF;

    private void OnEnable()
    {
        
        if (accionInteractuarF != null)
        {
            accionInteractuarF.action.Enable();
            accionInteractuarF.action.performed += OnFPresionada;
        }
    }

    private void OnDisable()
    {
        
        if (accionInteractuarF != null)
        {
            accionInteractuarF.action.performed -= OnFPresionada;
            accionInteractuarF.action.Disable();
        }
    }

    public override void StartPuzzle()
    {
        Debug.Log("Se detectó el inicio del Puzzle! Presioná F."); 
        puzzleActivo = true;
        toquesActuales = 0;
    }

    
    private void OnFPresionada(InputAction.CallbackContext context)
    {
        if (!puzzleActivo) return;

        toquesActuales++;
        Debug.Log($"Toques recibidos: {toquesActuales}/{toquesNecesarios}");

        if (toquesActuales >= toquesNecesarios)
        {
            FinalizarPuzzle();
        }
    }

    private void FinalizarPuzzle()
    {
        puzzleActivo = false;
        Debug.Log("¡Puzzle completado con éxito!"); 
        EnviarResultadoAlCerebro(true); 
    }
}