using UnityEngine;

public class CatAnimations
{
    private Cat cat;
    private float velocidadSuavizada;

    public CatAnimations(Cat catBrain) { cat = catBrain; }

    public void Tick()
    {
        if (cat.Anim == null) return;

        float velocidadReal = cat.Agent.velocity.magnitude;
        velocidadSuavizada = Mathf.Lerp(velocidadSuavizada, velocidadReal, Time.deltaTime * 8f);

        float valorFinalSpeed = (cat.estadoActual != Cat.CatState.Moving || velocidadSuavizada < 0.15f) ? 0f : velocidadSuavizada;

        cat.Anim.SetFloat("Speed", valorFinalSpeed);
    }
}