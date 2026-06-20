using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;

public class TutorialTrap : MonoBehaviour
{
    public enum FaseTrampa
    {
        EsperandoJugador,
        CinematicaEnCurso,
        Peleando,
        EsperandoInteraccion,
        GatoSaliendo,
        Terminado
    }

    [Header("Estado de la Misión")]
    public FaseTrampa faseActual = FaseTrampa.EsperandoJugador;

    [Header("Referencias de Actores")]
    public Player player;
    public Cat gato;

    [Header("Director de la Cinemática")]
    public PlayableDirector timelineCinematica;

    [Header("Elementos de la Trampa")]
    public GameObject escudoVisual;
    public Transform puertaDeSalida;

    [Header("Spawns de Combate")]
    public EnemySpawnPoint[] puntosDeSpawn;

    private List<GameObject> enemigosVivos = new List<GameObject>();

    private void Start()
    {
        if (escudoVisual != null) escudoVisual.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (faseActual == FaseTrampa.EsperandoJugador && other.CompareTag("Player"))
        {
            IniciarCinematica();
        }
    }

    private void IniciarCinematica()
    {
        faseActual = FaseTrampa.CinematicaEnCurso;
        player.IsMovementLocked = true; // Congela el New Input System visualmente

        Vector3 direccionTrampa = (escudoVisual.transform.position - player.transform.position).normalized;
        direccionTrampa.y = 0;
        player.transform.rotation = Quaternion.LookRotation(direccionTrampa);

        gato.isTrapped = true;

        if (timelineCinematica != null)
        {
            timelineCinematica.Play(); // Arranca el Timeline normalmente
        }
    }

    // Cambiamos el escuchador por una función directa que llamaremos desde el Update
    private void FinalizarCinematica()
    {
        faseActual = FaseTrampa.Peleando;

        // ¡DEVOLVEMOS EL CONTROL! Al volverse false, PlayerMovement y PlayerCombat vuelven a leer el New Input System
        player.IsMovementLocked = false;

        // Materializamos los enemigos con IA real en sus marcas
        SpawnearEnemigos();
    }

    private void Update()
    {
        // CONTROL MATEMÁTICO DEL TIMELINE:
        // Si la película está corriendo, revisamos si el tiempo actual llegó a la duración total
        if (faseActual == FaseTrampa.CinematicaEnCurso)
        {
            if (timelineCinematica != null && timelineCinematica.time >= (timelineCinematica.duration - 0.05f))
            {
                FinalizarCinematica();
            }
            return; // Evitamos procesar lo de abajo mientras dure la película
        }

        if (faseActual == FaseTrampa.Peleando)
        {
            enemigosVivos.RemoveAll(e => e == null || !e.activeInHierarchy || e.GetComponent<HealthSystem>().IsDead);

            if (enemigosVivos.Count == 0)
            {
                faseActual = FaseTrampa.EsperandoInteraccion;
            }
        }
        else if (faseActual == FaseTrampa.GatoSaliendo)
        {
            float distanciaSalida = Vector3.Distance(gato.transform.position, puertaDeSalida.position);

            if (distanciaSalida <= 1.5f)
            {
                gato.isTrapped = false;
                gato.estadoActual = Cat.CatState.Moving;
                faseActual = FaseTrampa.Terminado;
            }
        }
    }

    private void SpawnearEnemigos()
    {
        foreach (EnemySpawnPoint spawn in puntosDeSpawn)
        {
            if (spawn != null)
            {
                GameObject nuevoEnemigo = spawn.Spawnear();
                if (nuevoEnemigo != null) enemigosVivos.Add(nuevoEnemigo);
            }
        }
    }

    public void EscudoInteractuado()
    {
        if (faseActual != FaseTrampa.EsperandoInteraccion) return;

        if (escudoVisual != null) escudoVisual.SetActive(false);

        gato.isTrapped = false;
        gato.estadoActual = Cat.CatState.Moving;
        gato.Anim.SetBool("IsSitting", false);

        if (gato.Agent != null && gato.Agent.isActiveAndEnabled && gato.Agent.isOnNavMesh)
        {
            gato.Agent.isStopped = false;
            gato.Agent.updateRotation = true;
            gato.Agent.SetDestination(puertaDeSalida.position);
        }

        faseActual = FaseTrampa.GatoSaliendo;
    }
}