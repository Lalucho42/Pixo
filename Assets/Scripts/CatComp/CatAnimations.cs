using UnityEngine;

public class CatAnimations
{
    private Cat cat;

    public CatAnimations(Cat catBrain)
    {
        cat = catBrain;
    }

    public void Tick()
    {
        if (cat.Anim == null) return;

        float velocidad = cat.Agent.velocity.magnitude;
        cat.Anim.SetFloat("Speed", velocidad);
    }
}