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
        GatoSaliendo,
        Terminado
    }

    [Header("Estado de la Mision")]
    public FaseTrampa faseActual = FaseTrampa.EsperandoJugador;

    [Header("Referencias de Actores Reales")]
    public Player player;
    public Cat gato;
    public NavMeshAgent ciervoBebe;

    [Header("Actores Ficticios de la Cinematica")]
    public GameObject[] actoresCinematica;

    [Header("Director de la Cinematica")]
    public PlayableDirector timelineCinematica;

    [Header("Elementos de la Trampa")]
    public GameObject escudoVisual;
    public Transform puntoFinalTutorial;

    [Header("Spawns de Combate")]
    public EnemySpawnPoint[] puntosDeSpawn;

    [Header("Audio")]
    [Tooltip("Nombre del Sound configurado en AudioManager.musicaTracks que se reproduce justo al iniciar la cinematica de la trampa.")]
    public string musicaSuspenso = "Musica_Suspenso";
    [Tooltip("Nombre del Sound configurado en AudioManager.sfxClips que se reproduce al romperse el escudo.")]
    public string sfxEscudoRoto = "Escudo_Roto";

    private List<GameObject> enemigosVivos = new List<GameObject>();
    private bool rotacionFinalAplicada = false;

    private void Start()
    {
        if (gato != null && gato.Agent != null)
        {
            gato.Agent.isStopped = true;
        }

        if (ciervoBebe != null)
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

        if (gato != null)
        {
            gato.isTrapped = true;
            if (gato.Agent != null) gato.Agent.enabled = false;
        }

        if (ciervoBebe != null)
        {
            ciervoBebe.enabled = false;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(musicaSuspenso);
        }

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

        if (actoresCinematica != null)
        {
            for (int i = 0; i < actoresCinematica.Length; i++)
            {
                if (actoresCinematica[i] != null)
                {
                    actoresCinematica[i].SetActive(false);
                }
            }
        }

        if (gato != null && gato.Agent != null)
        {
            gato.Agent.enabled = true;
            gato.Agent.Warp(gato.transform.position);
            gato.Agent.isStopped = true;
            gato.Agent.ResetPath();
        }

        if (ciervoBebe != null)
        {
            ciervoBebe.enabled = true;
            ciervoBebe.Warp(ciervoBebe.transform.position);
            ciervoBebe.isStopped = true;
            ciervoBebe.ResetPath();
        }

        if (MissionUI.Instance != null)
        {
            MissionUI.Instance.ActualizarMision("Eliminate the robots and rescue the animals.");
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
            ActualizarEnemigosVivos();
            return;
        }

        if (faseActual == FaseTrampa.GatoSaliendo)
        {
            ActualizarSalidaDeAnimales();
        }
    }

    private void ActualizarEnemigosVivos()
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
            RomperEscudoYLiberarAnimales();
        }
    }

    private void ActualizarSalidaDeAnimales()
    {
        if (gato == null) return;

        float distanciaSalida = Vector3.Distance(gato.transform.position, puntoFinalTutorial.position);

        if (distanciaSalida <= 2.5f)
        {
            if (!rotacionFinalAplicada)
            {
                if (gato.Agent != null) gato.Agent.updateRotation = false;
                if (ciervoBebe != null) ciervoBebe.updateRotation = false;
                rotacionFinalAplicada = true;
            }

            gato.transform.rotation = Quaternion.Slerp(gato.transform.rotation, puntoFinalTutorial.rotation, Time.deltaTime * 4f);

            if (ciervoBebe != null)
            {
                ciervoBebe.transform.rotation = Quaternion.Slerp(ciervoBebe.transform.rotation, puntoFinalTutorial.rotation, Time.deltaTime * 4f);
            }
        }

        if (distanciaSalida <= 1.2f)
        {
            gato.transform.rotation = puntoFinalTutorial.rotation;
            gato.DetenerControlExterno(true);

            if (ciervoBebe != null)
            {
                ciervoBebe.transform.rotation = puntoFinalTutorial.rotation;
                ciervoBebe.isStopped = true;
                ciervoBebe.velocity = Vector3.zero;
            }

            faseActual = FaseTrampa.Terminado;
        }
    }

    private void SpawnearEnemigos()
    {
        foreach (EnemySpawnPoint spawn in puntosDeSpawn)
        {
            if (spawn != null)
            {
                GameObject nuevoEnemigo = spawn.Spawnear();
                if (nuevoEnemigo != null)
                {
                    enemigosVivos.Add(nuevoEnemigo);

                    NavMeshAgent agentEnemigo = nuevoEnemigo.GetComponent<NavMeshAgent>();
                    if (agentEnemigo != null && agentEnemigo.isActiveAndEnabled && agentEnemigo.isOnNavMesh)
                    {
                        agentEnemigo.velocity = Vector3.zero;
                        agentEnemigo.ResetPath();
                    }
                }
            }
        }
    }

    private void RomperEscudoYLiberarAnimales()
    {
        if (escudoVisual != null) escudoVisual.SetActive(false);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX2D(sfxEscudoRoto);
        }

        rotacionFinalAplicada = false;

        if (gato != null)
        {
            gato.IniciarControlExterno(puntoFinalTutorial.position);
        }

        if (ciervoBebe != null && ciervoBebe.isActiveAndEnabled && ciervoBebe.isOnNavMesh)
        {
            ciervoBebe.isStopped = false;
            ciervoBebe.updateRotation = true;
            ciervoBebe.SetDestination(puntoFinalTutorial.position);
        }

        if (MissionUI.Instance != null)
        {
            MissionUI.Instance.ActualizarMision("repair the exit");
        }

        faseActual = FaseTrampa.GatoSaliendo;
    }
}