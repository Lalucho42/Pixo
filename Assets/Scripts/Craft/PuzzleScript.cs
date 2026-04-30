using UnityEngine;

public class PuzzleScript : BasePuzzleModule
{
    [Header("Configuración del Puzzle")]
    public int toquesNecesarios = 5;
    private int toquesActuales = 0;
    private bool puzzleActivo = false;

    // Estas variables son las que la computadora mira para saber si ganaste
    public bool elPuzzleFueCompletadoConExito = false;
    public bool elPuzzleYaTermino = false;

    // Esto lo llama la computadora automáticamente[cite: 1, 3]
    public override void StartPuzzle()
    {
        Debug.Log("¡PC detectada! Iniciando puzzle de 5 toques.");
        puzzleActivo = true;
        toquesActuales = 0;
        elPuzzleYaTermino = false;
        elPuzzleFueCompletadoConExito = false;
    }

    private void Update()
    {
        if (!puzzleActivo) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            toquesActuales++;
            Debug.Log("Toques: " + toquesActuales + "/" + toquesNecesarios);

            if (toquesActuales >= toquesNecesarios)
            {
                FinalizarPuzzle();
            }
        }
    }

    private void FinalizarPuzzle()
    {
        puzzleActivo = false;

        // Seteamos las variables de la Guía Técnica
        elPuzzleFueCompletadoConExito = true;
        elPuzzleYaTermino = true;

        Debug.Log("¡Puzzle completado!");

        // Avisamos a la base que el puzzle terminó[cite: 5]
        EnviarResultadoAlCerebro(true);
    }
}