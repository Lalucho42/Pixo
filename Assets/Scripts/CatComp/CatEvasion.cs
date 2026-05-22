using UnityEngine;

public class CatEvasion
{
    private Cat cat;

    public CatEvasion(Cat catBrain)
    {
        cat = catBrain;
    }

    public bool Tick()
    {
        if (cat.Agent == null || !cat.Agent.isActiveAndEnabled || !cat.Agent.isOnNavMesh) return false;

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
        cat.Agent.updateRotation = true;

        // --- ADAPTACIÓN A MÁQUINA DE ESTADOS ---
        // Forzamos el estado a Moving para que el NavMesh no se clave a cero
        cat.estadoActual = Cat.CatState.Moving;

        // Por seguridad, si el gato estaba sentado, obligamos al Animator a quitar la pose
        cat.Anim.SetBool("IsSitting", false);

        Vector3 direccionHuida = (cat.transform.position - enemigo.position).normalized;
        Vector3 destinoSeguro = cat.transform.position + (direccionHuida * cat.distanciaHuida);

        cat.Agent.SetDestination(destinoSeguro);
    }
}