using UnityEngine;

public class CatMovement
{
    private Cat cat;
    private int puntoActual = 0;

    public CatMovement(Cat catBrain) { cat = catBrain; }

    public void Tick()
    {
        if (cat.Agent == null || !cat.Agent.isActiveAndEnabled || !cat.Agent.isOnNavMesh) return;

        // Evaluamos distancias y actualizamos el Enum de estado
        ActualizarEstadoIA();

        // Bloqueo absoluto: Si la IA no está en modo Moving, el motor se clava a cero
        if (cat.estadoActual != Cat.CatState.Moving)
        {
            cat.Agent.isStopped = true;
            cat.Agent.velocity = Vector3.zero;

            // Si está esperando/sentado mira al jugador. Si se está levantando mira al frente (waypoint).
            Vector3 objetivoMirada = (cat.estadoActual == Cat.CatState.Waiting || cat.estadoActual == Cat.CatState.Sitting)
                ? cat.player.position
                : ObtenerPosicionObjetivo();

            MirarHacia(objetivoMirada);
            return;
        }

        // Solo si pasó el filtro anterior, el NavMeshAgent puede avanzar
        ReanudarMarcha(ObtenerPosicionObjetivo());
    }

    private void ActualizarEstadoIA()
    {
        Vector3 targetPos = ObtenerPosicionObjetivo();
        float distDestino = Vector3.Distance(cat.transform.position, targetPos);
        float distJugador = Vector3.Distance(cat.transform.position, cat.player.position);

        if (cat.seguirJugador)
        {
            if (distDestino <= cat.distanciaAlPunto)
            {
                if (cat.estadoActual == Cat.CatState.Moving)
                {
                    cat.estadoActual = Cat.CatState.Waiting;
                    cat.Anim.SetTrigger("SitDown");
                    cat.Anim.SetBool("IsSitting", true);
                }
            }
            else
            {
                // Si el jugador se aleja, nos paramos
                if (cat.estadoActual == Cat.CatState.Sitting || cat.estadoActual == Cat.CatState.Waiting)
                {
                    cat.estadoActual = Cat.CatState.StandingUp;
                    cat.Anim.SetTrigger("StandUp");
                    cat.Anim.SetBool("IsSitting", false);
                }
            }
        }
        else // MODO WAYPOINTS
        {
            if (cat.waypoints.Length == 0 || puntoActual >= cat.waypoints.Length)
            {
                if (cat.estadoActual == Cat.CatState.Moving)
                {
                    cat.estadoActual = Cat.CatState.Waiting;
                    cat.Anim.SetTrigger("SitDown");
                    cat.Anim.SetBool("IsSitting", true);
                }
                return;
            }

            if (distDestino <= cat.distanciaAlPunto)
            {
                if (cat.estadoActual == Cat.CatState.Moving)
                {
                    cat.estadoActual = Cat.CatState.Waiting;
                    cat.Anim.SetTrigger("SitDown");
                    cat.Anim.SetBool("IsSitting", true);
                }

                // Si el jugador llega al radio de activación, avanzamos de punto e iniciamos el levantado
                if (distJugador <= cat.distanciaParaAvanzar)
                {
                    if (cat.estadoActual == Cat.CatState.Sitting || cat.estadoActual == Cat.CatState.Waiting)
                    {
                        puntoActual++;
                        cat.estadoActual = Cat.CatState.StandingUp;
                        cat.Anim.SetTrigger("StandUp");
                        cat.Anim.SetBool("IsSitting", false);
                    }
                }
            }
        }
    }

    private void MirarHacia(Vector3 objetivo)
    {
        Vector3 dir = (objetivo - cat.transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            cat.transform.rotation = Quaternion.Slerp(cat.transform.rotation, targetRot, Time.deltaTime * 5f);
        }
    }

    private Vector3 ObtenerPosicionObjetivo()
    {
        if (cat.seguirJugador) return cat.player.position;
        if (cat.waypoints.Length > 0 && puntoActual < cat.waypoints.Length)
            return cat.waypoints[puntoActual].position;
        return cat.transform.position;
    }

    private void ReanudarMarcha(Vector3 destino)
    {
        if (cat.Agent.isStopped) cat.Agent.isStopped = false;
        cat.Agent.SetDestination(destino);
    }
}