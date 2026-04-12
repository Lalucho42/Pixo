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
        GatoSaliendo, // Fase para asegurar que el gato llegue a la meta
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

        gato.isTrapped = true;
        gato.Agent.isStopped = false;
        gato.Agent.updateRotation = true;
        gato.Agent.SetDestination(ultimoWaypointDelGato.position);

        faseActual = FaseTrampa.GatoCorriendo;
    }

    private void Update()
    {
        if (faseActual == FaseTrampa.GatoCorriendo)
        {
            float distancia = Vector3.Distance(gato.transform.position, ultimoWaypointDelGato.position);

            if (distancia <= 1.5f)
            {
                gato.Agent.isStopped = true;
                gato.Agent.velocity = Vector3.zero;

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
            // Limpia la lista de enemigos muertos
            enemigosVivos.RemoveAll(e => e == null || !e.activeInHierarchy || e.GetComponent<HealthSystem>().IsDead);

            if (enemigosVivos.Count == 0)
            {
                faseActual = FaseTrampa.EsperandoInteraccion;
                Debug.Log("Pelea terminada. Acercate a interactuar con el escudo.");
            }
        }
        // --- NUEVA LÓGICA DE SALIDA SEGURA ---
        else if (faseActual == FaseTrampa.GatoSaliendo)
        {
            float distanciaSalida = Vector3.Distance(gato.transform.position, puertaDeSalida.position);

            // Si el gato llega a la puerta, recién ahí lo liberamos
            if (distanciaSalida <= 1.5f)
            {
                gato.isTrapped = false;
                faseActual = FaseTrampa.Terminado;
                Debug.Log("Gato libre y en la salida.");
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

        // Mantenemos 'isTrapped = true' para que el gato no intente volver con el jugador
        gato.Agent.isStopped = false;
        gato.Agent.updateRotation = true;
        gato.Agent.SetDestination(puertaDeSalida.position);

        faseActual = FaseTrampa.GatoSaliendo;
        Debug.Log("¡Gato libre! Corriendo a la salida.");
    }
}