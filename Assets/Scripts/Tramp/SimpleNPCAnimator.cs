using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class SimpleNPCAnimator : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private float timerCambioIdle = 0f;
    private float tiempoParaSiguienteIdle = 5f;
    private float idleIndexActual = 0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (agent == null || animator == null) return;

        float velocidadActual = agent.velocity.magnitude;
        animator.SetFloat("Speed", velocidadActual);

        if (velocidadActual < 0.1f)
        {
            timerCambioIdle += Time.deltaTime;
            if (timerCambioIdle >= tiempoParaSiguienteIdle)
            {
                timerCambioIdle = 0f;
                idleIndexActual = Random.Range(0, 3);
                tiempoParaSiguienteIdle = Random.Range(4f, 7f);
            }
        }
        else
        {
            timerCambioIdle = 0f;
        }

        animator.SetFloat("IdleIndex", idleIndexActual);
    }
}