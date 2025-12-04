using UnityEngine;

public class DeathCryBehaviour : MonoBehaviour
{
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float emitterDuration = 0f;

    [SerializeField] private ParticleSystem emitter;


    private void Awake()
    {
        if (emitter != null)
        {
            emitter.Play();
        }
    }

    void FixedUpdate()
    {
        lifeTime -= Time.fixedDeltaTime;
        if (lifeTime < 0)
        {
            Destroy(gameObject);
        }

        if (emitter != null)
        {
            emitterDuration -= Time.fixedDeltaTime;

            if (emitterDuration < 0)
            {
                emitter.Stop();
            }
        }
    }
}
