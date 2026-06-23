using UnityEngine;
using UnityEngine.AI;
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
    public NavMeshAgent ciervoBebe;

    [Header("Director de la Cinemática")]
    public PlayableDirector timelineCinematica;

    [Header("Elementos de la Trampa")]
    public GameObject escudoVisual;
    public Transform puntoFinalTutorial;

    [Header("Spawns de Combate")]
    public EnemySpawnPoint[] puntosDeSpawn;

    private List<GameObject> enemigosVivos = new List<GameObject>();

    private void Start()
    {
        if (gato != null && gato.Agent != null && gato.Agent.isActiveAndEnabled && gato.Agent.isOnNavMesh)
        {
            gato.Agent.isStopped = true;
        }

        if (ciervoBebe != null && ciervoBebe.isActiveAndEnabled && ciervoBebe.isOnNavMesh)
        {
            ciervoBebe.isStopped = true;
        }
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
        player.IsMovementLocked = true;

        Vector3 direccionTrampa = (puntoFinalTutorial.position - player.transform.position).normalized;
        direccionTrampa.y = 0;
        player.transform.rotation = Quaternion.LookRotation(direccionTrampa);

        if (gato != null) gato.isTrapped = true;

        if (timelineCinematica != null)
        {
            timelineCinematica.Play();
        }
    }

    private void FinalizarCinematica()
    {
        faseActual = FaseTrampa.Peleando;
        player.IsMovementLocked = false;

        if (timelineCinematica != null)
        {
            timelineCinematica.Stop();
        }
        if (escudoVisual != null)
        {
            escudoVisual.SetActive(true);
        }

        SpawnearEnemigos();
    }

    private void Update()
    {
        if (faseActual == FaseTrampa.CinematicaEnCurso)
        {
            if (timelineCinematica != null && timelineCinematica.time >= (timelineCinematica.duration - 0.05f))
            {
                FinalizarCinematica();
            }
            return;
        }

        if (faseActual == FaseTrampa.Peleando)
        {
            for (int i = enemigosVivos.Count - 1; i >= 0; i--)
            {
                if (enemigosVivos[i] == null || !enemigosVivos[i].activeInHierarchy)
                {
                    enemigosVivos.RemoveAt(i);
                    continue;
                }

                HealthSystem hp = enemigosVivos[i].GetComponent<HealthSystem>();
                if (hp != null && hp.IsDead)
                {
                    enemigosVivos.RemoveAt(i);
                }
            }

            if (enemigosVivos.Count == 0)
            {
                faseActual = FaseTrampa.EsperandoInteraccion;
            }
        }
        else if (faseActual == FaseTrampa.GatoSaliendo)
        {
            if (gato != null)
            {
                float distanciaSalida = Vector3.Distance(gato.transform.position, puntoFinalTutorial.position);
                if (distanciaSalida <= 2.0f)
                {
                    gato.isTrapped = false;
                    gato.estadoActual = Cat.CatState.Waiting;
                    faseActual = FaseTrampa.Terminado;
                }
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

        if (gato != null)
        {
            gato.isTrapped = false;
            gato.estadoActual = Cat.CatState.Moving;
            if (gato.Anim != null) gato.Anim.SetBool("IsSitting", false);

            if (gato.Agent != null && gato.Agent.isActiveAndEnabled && gato.Agent.isOnNavMesh)
            {
                gato.Agent.isStopped = false;
                gato.Agent.updateRotation = true;
                gato.Agent.SetDestination(puntoFinalTutorial.position);
            }
        }

        if (ciervoBebe != null && ciervoBebe.isActiveAndEnabled && ciervoBebe.isOnNavMesh)
        {
            ciervoBebe.isStopped = false;
            ciervoBebe.updateRotation = true;
            ciervoBebe.SetDestination(puntoFinalTutorial.position);
        }

        faseActual = FaseTrampa.GatoSaliendo;
    }
}