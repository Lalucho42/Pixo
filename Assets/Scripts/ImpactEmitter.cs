using UnityEngine;

public class ImpactEmitter : MonoBehaviour
{
    public SurfaceEffectData resourceEffect;
    public SurfaceEffectData creatureEffect;
    public GameObject simpleResource;
    public GameObject simpleCreature;

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        string collidedTag = "";
        try { collidedTag = collision.gameObject.tag; } catch { }

        if (collidedTag == "Recurso")
        {
            if (resourceEffect != null) GameEvents.TriggerSurfaceImpact(contact.point, contact.normal, resourceEffect);
            else if (simpleResource != null) GameEvents.TriggerSimpleImpact(contact.point, contact.normal, simpleResource);
        }
        else if (collidedTag == "Criatura")
        {
            if (creatureEffect != null) GameEvents.TriggerSurfaceImpact(contact.point, contact.normal, creatureEffect);
            else if (simpleCreature != null) GameEvents.TriggerSimpleImpact(contact.point, contact.normal, simpleCreature);
        }
    }
}
