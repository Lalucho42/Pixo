using UnityEngine;

public class CatMovement
{
    private Cat cat;
    private int puntoActual = 0;

    public CatMovement(Cat catBrain)
    {
        cat = catBrain;

        if (cat.waypoints.Length > 0)
        {
            ReanudarMarcha();
        }
    }

    public void Tick()
    {
        if (cat.waypoints.Length == 0 || puntoActual >= cat.waypoints.Length)
        {
            // Solo mandamos la orden de frenar si NO estaba frenado ya
            if (!cat.Agent.isStopped) DetenerYMirarJugador();
            return;
        }

        float distanciaJugador = Vector3.Distance(cat.transform.position, cat.player.position);
        float distanciaAlPunto = Vector3.Distance(cat.transform.position, cat.waypoints[puntoActual].position);

        // 1. ¿EL GATO YA LLEGÓ AL WAYPOINT? (Acá es donde te espera)
        if (distanciaAlPunto <= cat.distanciaAlPunto)
        {
            if (!cat.Agent.isStopped) DetenerYMirarJugador();

            // ¿El jugador ya llegó a donde estoy yo?
            if (distanciaJugador <= cat.distanciaParaAvanzar)
            {
                puntoActual++;
                if (puntoActual < cat.waypoints.Length)
                {
                    ReanudarMarcha();
                }
            }
        }
        // 2. EL GATO ESTÁ CAMINANDO 
        else
        {
            // ¡EL ARREGLO ESTÁ ACÁ! 
            // Solo le decimos que marche si estaba frenado. Así no rompemos el salto.
            if (cat.Agent.isStopped)
            {
                ReanudarMarcha();
            }
        }
    }

    private void DetenerYMirarJugador()
    {
        cat.Agent.isStopped = true;
        cat.Agent.velocity = Vector3.zero;
        cat.Agent.updateRotation = false;

        Vector3 direccion = cat.player.position - cat.transform.position;
        direccion.y = 0;
        if (direccion != Vector3.zero)
        {
            cat.transform.rotation = Quaternion.Slerp(cat.transform.rotation, Quaternion.LookRotation(direccion), Time.deltaTime * 5f);
        }
    }

    private void ReanudarMarcha()
    {
        cat.Agent.isStopped = false;
        cat.Agent.updateRotation = true;
        cat.Agent.SetDestination(cat.waypoints[puntoActual].position); // Se llama UNA sola vez
    }
}