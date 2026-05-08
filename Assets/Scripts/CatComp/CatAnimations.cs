using UnityEngine;

public class CatAnimations
{
    private Cat cat;
    private float turnSuavizado;
    private float velocidadSuavizada;
    public float TurnValue => turnSuavizado;

    public CatAnimations(Cat catBrain) { cat = catBrain; }

    public void Tick()
    {
        if (cat.Anim == null) return;

        // --- CORRECCIÓN DE VELOCIDAD ---
        // Usamos desiredVelocity para que la animación reaccione al INSTANTE
        float velocidadObjetivo = cat.Agent.isStopped ? 0f : cat.Agent.desiredVelocity.magnitude;
        velocidadSuavizada = Mathf.Lerp(velocidadSuavizada, velocidadObjetivo, Time.deltaTime * 8f);

        // Si la velocidad es muy baja, forzamos 0 para evitar mezclas raras
        float valorFinalSpeed = (velocidadSuavizada < 0.1f) ? 0f : velocidadSuavizada;
        cat.Anim.SetFloat("Speed", valorFinalSpeed);

        cat.Anim.SetBool("IsTurning", cat.estaGirando);

        // --- CÁLCULO DE TURN ---
        Vector3 dirObjetivo = cat.Movement.ObtenerDireccionAlObjetivo();
        if (dirObjetivo.magnitude > 0.1f)
        {
            float angulo = Vector3.SignedAngle(cat.transform.forward, dirObjetivo, Vector3.up);
            float turnTarget = (Mathf.Abs(angulo) < 5f) ? 0f : Mathf.Clamp(angulo / 45f, -1f, 1f);
            turnSuavizado = Mathf.Lerp(turnSuavizado, turnTarget, Time.deltaTime * 5f);
        }
        else turnSuavizado = Mathf.Lerp(turnSuavizado, 0, Time.deltaTime * 5f);

        cat.Anim.SetFloat("Turn", turnSuavizado);

        ManejarSentado(cat.Agent.velocity.magnitude);
    }

    private void ManejarSentado(float velocidadReal)
    {
        if (cat.esperandoAlJugador)
        {
            if (!cat.estaSentado && velocidadReal < 0.1f)
            {
                cat.estaSentado = true;
                cat.Anim.SetTrigger("SitDown");
                cat.Anim.SetBool("IsSitting", true);
            }
        }
        else
        {
            if (cat.estaSentado && !cat.bloqueadoPorAnimacion)
            {
                cat.bloqueadoPorAnimacion = true;
                cat.Anim.SetTrigger("StandUp");
                cat.Anim.SetBool("IsSitting", false);
            }
        }
    }
}