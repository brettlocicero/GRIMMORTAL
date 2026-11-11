using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Meteor : MonoBehaviour
{
    [SerializeField] float explosionRadius = 4f;
    [SerializeField] LayerMask groundMask;
    [SerializeField] LayerMask enemyLayerMask;
    [SerializeField] GameObject explosionVFXPrefab;
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] AudioClip explosionSound;

    [Header("Debris")]
    [SerializeField] LayerMask debrisLayerMask;
    [SerializeField] float explosionForce = 30f;
    [SerializeField] float upwardsModifier = 1f;

    AudioSource audioSource;
    Rigidbody rb;
    float damage;
    float stunTime;
    bool triggered = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        audioSource.pitch = Random.Range(0.8f, 1.2f);
    }

    public void Init(float damage, float stunTime, Vector3 velocity)
    {
        this.damage = damage;
        this.stunTime = stunTime;

        rb.linearVelocity = velocity;
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        bool hitGround = (groundMask.value & (1 << other.gameObject.layer)) != 0;

        if (hitGround)
            Explode(transform.position);
    }

    void Explode(Vector3 point)
    {
        DamageEnemies(point);
        PushDebris(point);

        if (explosionVFXPrefab)
            Destroy(Instantiate(explosionVFXPrefab, point, Quaternion.identity), 5f);

        particleSystem.Stop();
        GetComponent<MeshRenderer>().enabled = false;
        rb.isKinematic = true;

        CinemachineShakeManager.Instance.ShakeCamera(4f, 0.25f, 1f, 90);
        Destroy(gameObject, 3f);
        triggered = true;

        audioSource.Stop();
        audioSource.PlayOneShot(explosionSound);
    }

    void DamageEnemies(Vector3 point)
    {
        Collider[] hits = Physics.OverlapSphere(point, explosionRadius, enemyLayerMask);

        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent(out EnemyAI enemy))
                enemy.TakeDamage(damage, stunTime, AttackDirection.None, enemy.transform.position);
        }
    }

    void PushDebris(Vector3 point)
    {
        Collider[] hits = Physics.OverlapSphere(point, explosionRadius, debrisLayerMask);

        foreach (Collider hit in hits)
        {
            if (hit.attachedRigidbody)
                hit.attachedRigidbody.AddExplosionForce(explosionForce, point, explosionRadius, upwardsModifier, ForceMode.Impulse);
        }
    }
}