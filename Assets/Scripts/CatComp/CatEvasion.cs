using UnityEngine;

public class CatEvasion
{
    private Cat cat;

    public CatEvasion(Cat catBrain)
    {
        cat = catBrain;
    }

    // Devuelve TRUE si encontró un enemigo y está huyendo
    public bool Tick()
    {
        Transform enemigoCercano = BuscarEnemigo();

        if (enemigoCercano != null)
        {
            HuirDe(enemigoCercano);
            return true;
        }

        return false;
    }

    private Transform BuscarEnemigo()
    {
        Collider[] hits = Physics.OverlapSphere(cat.transform.position, cat.radioDeteccion);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy")) return hit.transform;
        }
        return null;
    }

    private void HuirDe(Transform enemigo)
    {
        cat.Agent.isStopped = false;
        cat.Agent.updateRotation = true; // <- AGREGAR ESTA LÍNEA AQUÍ

        Vector3 direccionHuida = (cat.transform.position - enemigo.position).normalized;
        Vector3 destinoSeguro = cat.transform.position + (direccionHuida * cat.distanciaHuida);

        cat.Agent.SetDestination(destinoSeguro);
    }
}