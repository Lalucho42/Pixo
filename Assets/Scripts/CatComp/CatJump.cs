using UnityEngine;
using UnityEngine.AI;

public class CatJump
{
    private Cat cat;
    private float progresoSalto = 0f;
    private Vector3 inicioSalto;
    private Vector3 finSalto;

    public CatJump(Cat catBrain)
    {
        cat = catBrain;
    }

    public bool Tick()
    {
        if (cat.estaSaltando)
        {
            EjecutarSalto();
            return true;
        }

        if (cat.Agent.isOnOffMeshLink)
        {
            PrepararSalto();
            return true;
        }

        return false;
    }

    private void PrepararSalto()
    {
        cat.estaSaltando = true;
        progresoSalto = 0f;

        OffMeshLinkData data = cat.Agent.currentOffMeshLinkData;
        inicioSalto = cat.transform.position; // Que salte exactamente desde donde tiene las patas
        finSalto = data.endPos;

        // --- LA MAGIA CONTRA EL ANCLA ---
        // Le sacamos el control de la física al NavMesh para que no pelee con nuestro salto
        cat.Agent.updatePosition = false;
        cat.Agent.updateRotation = false;
        cat.Agent.velocity = Vector3.zero;
    }

    private void EjecutarSalto()
    {
        progresoSalto += Time.deltaTime / cat.duracionSalto;

        if (progresoSalto >= 1f)
        {
            // Ya aterrizó
            cat.transform.position = finSalto;
            cat.Agent.CompleteOffMeshLink();

            // --- LE DEVOLVEMOS EL CONTROL AL NAVMESH ---
            cat.Agent.updatePosition = true;
            cat.Agent.updateRotation = true;

            cat.estaSaltando = false;
        }
        else
        {
            // Sigue en el aire: aplicamos la parábola libremente
            Vector3 posicionActual = Vector3.Lerp(inicioSalto, finSalto, progresoSalto);
            posicionActual.y += cat.alturaSalto * 4f * progresoSalto * (1f - progresoSalto);
            cat.transform.position = posicionActual;

            // Hacemos que gire suavemente mirando hacia donde va a aterrizar
            Vector3 direccion = finSalto - inicioSalto;
            direccion.y = 0;
            if (direccion != Vector3.zero)
            {
                cat.transform.rotation = Quaternion.Slerp(cat.transform.rotation, Quaternion.LookRotation(direccion), Time.deltaTime * 10f);
            }
        }
    }
}