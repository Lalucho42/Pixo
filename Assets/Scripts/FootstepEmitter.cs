using UnityEngine;

public class FootstepEmitter : MonoBehaviour
{
    public GameObject simpleEffect;
    
    [Header("Audio")]
    public AudioClip footstepClick;
    public float footstepVolume = 0.5f;

    public bool isAutomatic = true;
    public float stepDistance = 1.5f;
    public Vector3 stepPositionOffset = new Vector3(0, -0.8f, 0);

    private Vector3 lastPosition;
    private float distanceMoved;
    private CharacterController controller;

    private void Start()
    {
        lastPosition = transform.position;
        controller = GetComponentInParent<CharacterController>();
    }

    private void Update()
    {
        if (isAutomatic)
        {
            if (controller != null && !controller.isGrounded) return;

            float dist = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(lastPosition.x, 0, lastPosition.z));
            distanceMoved += dist;
            lastPosition = transform.position;

            if (distanceMoved >= stepDistance)
            {
                TriggerFootstep();
                distanceMoved = 0f;
            }
        }
    }

    public void TriggerFootstep()
    {
        Vector3 spawnPosition = transform.position + stepPositionOffset;
        
        if (simpleEffect != null) GameEvents.TriggerSimpleImpact(spawnPosition, Vector3.up, simpleEffect);

        if (footstepClick != null && AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(footstepClick, footstepVolume);
        }
    }
}
