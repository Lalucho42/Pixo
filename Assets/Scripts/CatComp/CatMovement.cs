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
        if (cat.Agent == null || !cat.Agent.isActiveAndEnabled || !cat.Agent.isOnNavMesh) return;

        if (cat.waypoints.Length == 0 || puntoActual >= cat.waypoints.Length)
        {
            if (!cat.Agent.isStopped) DetenerYMirarJugador();
            return;
        }

        float distanciaJugador = Vector3.Distance(cat.transform.position, cat.player.position);
        float distanciaAlPunto = Vector3.Distance(cat.transform.position, cat.waypoints[puntoActual].position);

        if (distanciaAlPunto <= cat.distanciaAlPunto)
        {
            if (!cat.Agent.isStopped) DetenerYMirarJugador();

            if (distanciaJugador <= cat.distanciaParaAvanzar)
            {
                puntoActual++;
                if (puntoActual < cat.waypoints.Length)
                {
                    ReanudarMarcha();
                }
            }
        }
        else
        {
            if (cat.Agent.isStopped)
            {
                ReanudarMarcha();
            }
        }
    }

    private void DetenerYMirarJugador()
    {
        if (cat.Agent == null || !cat.Agent.isActiveAndEnabled || !cat.Agent.isOnNavMesh) return;

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
        if (cat.Agent == null || !cat.Agent.isActiveAndEnabled || !cat.Agent.isOnNavMesh) return;

        cat.Agent.isStopped = false;
        cat.Agent.updateRotation = true;
        cat.Agent.SetDestination(cat.waypoints[puntoActual].position);
    }
}
