using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PuzzleScript : BasePuzzleModule
{
    [Header("Referencias de UI")]
    public GameObject panelDelPuzzle;
    public Image imagenDeLaTeclaA;
    public Image imagenDeLaTeclaD;

    [Header("Colores")]
    public Color colorGrisNormal = Color.white;
    public Color colorVerdeExito = Color.green;
    public Color colorRojoError = Color.red;

    private int[] patronDeTeclas = { 0, 1, 0, 0, 1 };
    private int pasoEnElQueVaElJugador = 0;
    private bool elJuegoEstaAndando = false;

    public override void StartPuzzle()
    {
        elJuegoEstaAndando = true;
        pasoEnElQueVaElJugador = 0;

        if (panelDelPuzzle != null) panelDelPuzzle.SetActive(true);

        Player.Instance.InputHandler.ActivarControlesDelPuzzle();
        Player.Instance.InputHandler.OnTeclaAPresionada += CuandoElJugadorTocaA;
        Player.Instance.InputHandler.OnTeclaDPresionada += CuandoElJugadorTocaD;

        ActualizarLosColoresDeLaPantalla();
    }

    private void CuandoElJugadorTocaA() { if (elJuegoEstaAndando) RevisarSiGanoOPerdio(0); }
    private void CuandoElJugadorTocaD() { if (elJuegoEstaAndando) RevisarSiGanoOPerdio(1); }

    private void RevisarSiGanoOPerdio(int teclaQueToco)
    {
        if (teclaQueToco == patronDeTeclas[pasoEnElQueVaElJugador])
        {
            pasoEnElQueVaElJugador = pasoEnElQueVaElJugador + 1;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX2D("Puzzle_Correcto");
            }

            if (pasoEnElQueVaElJugador >= patronDeTeclas.Length)
            {
                TerminarElPuzzleYGanar();
            }
            else
            {
                StartCoroutine(EfectoDeTeclaCorrecta());
            }
        }
        else
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX2D("Puzzle_Error");
            }
            StartCoroutine(MostrarEFERojo());
        }
    }

    IEnumerator EfectoDeTeclaCorrecta()
    {
        imagenDeLaTeclaA.color = colorGrisNormal;
        imagenDeLaTeclaD.color = colorGrisNormal;
        yield return new WaitForSeconds(0.1f);
        ActualizarLosColoresDeLaPantalla();
    }

    private void ActualizarLosColoresDeLaPantalla()
    {
        imagenDeLaTeclaA.color = colorGrisNormal;
        imagenDeLaTeclaD.color = colorGrisNormal;

        if (patronDeTeclas[pasoEnElQueVaElJugador] == 0) imagenDeLaTeclaA.color = colorVerdeExito;
        else imagenDeLaTeclaD.color = colorVerdeExito;
    }

    IEnumerator MostrarEFERojo()
    {
        pasoEnElQueVaElJugador = 0;
        imagenDeLaTeclaA.color = colorRojoError;
        imagenDeLaTeclaD.color = colorRojoError;
        yield return new WaitForSeconds(0.5f);
        ActualizarLosColoresDeLaPantalla();
    }

    private void TerminarElPuzzleYGanar()
    {
        elJuegoEstaAndando = false;
        if (panelDelPuzzle != null) panelDelPuzzle.SetActive(false);

        Player.Instance.InputHandler.OnTeclaAPresionada -= CuandoElJugadorTocaA;
        Player.Instance.InputHandler.OnTeclaDPresionada -= CuandoElJugadorTocaD;
        Player.Instance.InputHandler.ActivarControlesDeCaminar();

        EnviarResultadoAlCerebro(true);
    }
}