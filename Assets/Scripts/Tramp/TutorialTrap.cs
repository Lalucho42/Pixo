using UnityEngine;
using System.Collections.Generic;

public class TutorialTrap : MonoBehaviour
{
    public enum FaseTrampa
    {
        EsperandoJugador,
        GatoCorriendo,
        PausaDramatica,
        Peleando,
        EsperandoInteraccion,
        GatoSaliendo,
        Terminado
    }

    [Header("Estado Actual")]
    public FaseTrampa faseActual = FaseTrampa.EsperandoJugador;

    [Header("Referencias Clave")]
    public Player player;
    public Cat gato;

    [Header("El Escudo y Salida")]
    public GameObject escudoVisual;
    public Transform ultimoWaypointDelGato;
    public Transform puertaDeSalida;

    [Header("Los Enemigos (Spawns)")]
    public EnemySpawnPoint[] puntosDeSpawn;

    private List<GameObject> enemigosVivos = new List<GameObject>();
    private float cronometro = 0f;

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
        player.IsMovementLocked = true;

        Vector3 direccionGato = (ultimoWaypointDelGato.position - player.transform.position).normalized;
        direccionGato.y = 0;
        player.transform.rotation = Quaternion.LookRotation(direccionGato);

        // --- INTEGRACIÓN CON MÁQUINA DE ESTADOS ---
        gato.isTrapped = true;
        gato.estadoActual = Cat.CatState.Moving; // Le avisamos al sistema que el gato debe moverse
        gato.Anim.SetBool("IsSitting", false);    // Quitamos la pose de sentado inmediatamente

        if (gato.Agent != null && gato.Agent.isActiveAndEnabled && gato.Agent.isOnNavMesh)
        {
            gato.Agent.isStopped = false;
            gato.Agent.updateRotation = true;
            gato.Agent.SetDestination(ultimoWaypointDelGato.position);
        }

        faseActual = FaseTrampa.GatoCorriendo;
    }

    private void Update()
    {
        if (faseActual == FaseTrampa.GatoCorriendo)
        {
            float distancia = Vector3.Distance(gato.transform.position, ultimoWaypointDelGato.position);

            if (distancia <= 1.5f)
            {
                if (gato.Agent != null && gato.Agent.isActiveAndEnabled && gato.Agent.isOnNavMesh)
                {
                    gato.Agent.isStopped = true;
                    gato.Agent.velocity = Vector3.zero;
                }

                // --- INTEGRACIÓN CON MÁQUINA DE ESTADOS ---
                // El gato llegó a la trampa, forzamos el estado de sentado visual
                gato.estadoActual = Cat.CatState.Sitting;
                gato.Anim.SetTrigger("SitDown");
                gato.Anim.SetBool("IsSitting", true);

                if (escudoVisual != null) escudoVisual.SetActive(true);

                SpawnearEnemigos();

                cronometro = 1.5f;
                faseActual = FaseTrampa.PausaDramatica;
            }
        }
        else if (faseActual == FaseTrampa.PausaDramatica)
        {
            cronometro -= Time.deltaTime;

            if (cronometro <= 0f)
            {
                player.IsMovementLocked = false;
                faseActual = FaseTrampa.Peleando;
            }
        }
        else if (faseActual == FaseTrampa.Peleando)
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
                // Devolvemos el gato al estado Moving normal para que retome su IA común
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

        // --- INTEGRACIÓN CON MÁQUINA DE ESTADOS ---
        // Liberamos al gato: pasa a Moving y se apaga el bool de sentado
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