using UnityEngine;

public class CatMovement
{
    private Cat cat;
    private int puntoActual = 0;

    public CatMovement(Cat catBrain) { cat = catBrain; }

    public void Tick()
    {
        if (cat.Agent == null || !cat.Agent.isActiveAndEnabled || !cat.Agent.isOnNavMesh) return;

        ActualizarEstadoEspera();

        // 1. Prioridad: Mirar al jugador si estamos esperando o sentados
        if (cat.esperandoAlJugador)
        {
            MirarHacia(cat.player.position);
            return;
        }

        // 2. Bloqueos de animación (levantarse o girar)
        if (cat.bloqueadoPorAnimacion || cat.estaSentado || cat.estaGirando)
        {
            MirarHacia(ObtenerPosicionObjetivo());
            return;
        }

        // 3. Lógica de arranque
        // Si el ángulo es muy cerrado (> 0.8), activamos el giro de 90 grados
        if (Mathf.Abs(cat.Animations.TurnValue) > 0.8f)
        {
            cat.estaGirando = true;
            cat.Agent.isStopped = true;
        }
        else
        {
            // Si no estamos esperando ni bloqueados, ¡CAMINAMOS!
            ReanudarMarcha(ObtenerPosicionObjetivo());
        }
    }

    private void ActualizarEstadoEspera()
    {
        Vector3 targetPos = ObtenerPosicionObjetivo();
        float distDestino = Vector3.Distance(cat.transform.position, targetPos);
        float distJugador = Vector3.Distance(cat.transform.position, cat.player.position);

        if (cat.seguirJugador)
        {
            cat.esperandoAlJugador = (distDestino <= cat.distanciaAlPunto);
        }
        else
        {
            if (cat.waypoints.Length == 0 || puntoActual >= cat.waypoints.Length)
            {
                cat.esperandoAlJugador = true;
                return;
            }

            if (distDestino <= cat.distanciaAlPunto)
            {
                cat.esperandoAlJugador = true;
                // Si el jugador está cerca, pasamos al siguiente punto
                if (distJugador <= cat.distanciaParaAvanzar)
                {
                    puntoActual++;
                    cat.esperandoAlJugador = false;
                }
            }
        }
    }

    private void MirarHacia(Vector3 objetivo)
    {
        cat.Agent.isStopped = true;
        cat.Agent.velocity = Vector3.zero;

        Vector3 dir = (objetivo - cat.transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            cat.transform.rotation = Quaternion.Slerp(cat.transform.rotation, targetRot, Time.deltaTime * 6f);
        }
    }

    public Vector3 ObtenerDireccionAlObjetivo()
    {
        Vector3 posDestino = ObtenerPosicionObjetivo();
        Vector3 dir = (posDestino - cat.transform.position).normalized;
        dir.y = 0;
        return dir;
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
        // Forzamos al agente a encenderse
        if (cat.Agent.isStopped) cat.Agent.isStopped = false;
        cat.Agent.SetDestination(destino);
    }
}