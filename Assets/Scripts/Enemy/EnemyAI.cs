using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Action<EnemyAI> OnEnemyKilled;

    [Header("Components")]
    [SerializeField] EnemyMovement movement;
    [SerializeField] EnemyAttack attack;

    [Header("Stats")]
    [SerializeField] float maxHealth = 100f;
    [SerializeField, Min(1)] int threat = 1;

    [Header("AI Settings")]
    [SerializeField] float thinkInterval = 0.5f;

    [Header("References")]
    [SerializeField] Animator animator;
    [SerializeField] float hitAnimationLength = 0.5f;

    [Header("FX & Audio")]
    [SerializeField] ParticleSystem[] hitVFXParticles;
    [SerializeField] AudioClip hitSFX;
    [SerializeField] GameObject deathVFXPrefab;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] AudioSource audioSource;

    PlayerState target;
    float currentHealth;
    float nextThinkTime;
    float stunEndTime;
    bool isDead;

    public int Threat => threat;
    public float HealthPercent => currentHealth / maxHealth;

    bool IsStunned => Time.time < stunEndTime;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Start()
    {
        target = FindAnyObjectByType<PlayerState>();
    }

    void Update()
    {
        bool active = target && !isDead && !IsStunned;

        if (active)
            Tick();
    }

    void Tick()
    {
        if (!attack.IsAttacking && Time.time >= nextThinkTime)
        {
            nextThinkTime = Time.time + thinkInterval;
            Think();
        }

        movement.Face(target.transform.position);
    }

    void Think()
    {
        Vector3 targetPosition = target.transform.position;
        float distance = Vector3.Distance(transform.position, targetPosition);

        bool tooClose = distance < attack.MinRange;
        bool tooFar = distance > attack.Range;

        if (tooClose)
            movement.MoveAwayFrom(targetPosition);

        else if (tooFar)
            movement.MoveTo(targetPosition);

        else if (!attack.AttacksWhileMoving)
            movement.Stop();

        bool canAttackHere = !tooFar && (attack.AttacksWhileMoving || !tooClose);

        if (canAttackHere && attack.IsReady)
            attack.Execute(target);
    }

    public bool TakeDamage(float damage, float stunTime, AttackDirection attackDirection)
    {
        bool killed = false;

        if (!isDead)
        {
            currentHealth -= damage;
            killed = currentHealth <= 0f;

            if (killed)
            {
                Die();
            }

            else
            {
                PlayHitFX();

                if (stunTime > 0f)
                    Stun(stunTime, attackDirection);
            }
        }

        return killed;
    }

    void Stun(float duration, AttackDirection attackDirection)
    {
        attack.Cancel();
        movement.Stop();
        stunEndTime = Time.time + duration;

        PlayHitAnimation(attackDirection, duration);
    }

    public void PlayAttackAnimation()
    {
        animator.SetTrigger("Attack");
    }

    void PlayHitAnimation(AttackDirection attackDirection, float duration)
    {
        animator.ResetTrigger("Attack");
        animator.SetFloat("HitSpeed", hitAnimationLength / duration);

        switch (attackDirection)
        {
            case AttackDirection.Left:
                animator.SetTrigger("HitLeft");
                break;
            case AttackDirection.Right:
                animator.SetTrigger("HitRight");
                break;
            default:
                animator.SetTrigger("SmallHit");
                break;
        }
    }

    void PlayHitFX()
    {
        if (audioSource && hitSFX)
            audioSource.PlayOneShot(hitSFX);

        foreach (ParticleSystem ps in hitVFXParticles)
            ps.Play();
    }

    void Die()
    {
        isDead = true;
        attack.Cancel();
        movement.Stop();

        PlayDeathFX();

        OnEnemyKilled?.Invoke(this);
        Destroy(gameObject);
    }

    void PlayDeathFX()
    {
        if (deathSFX)
            AudioSource.PlayClipAtPoint(deathSFX, transform.position, 0.2f);

        if (deathVFXPrefab)
        {
            GameObject deathFX = Instantiate(deathVFXPrefab, transform.position, transform.rotation);

            foreach (Rigidbody deathRb in deathFX.GetComponentsInChildren<Rigidbody>())
                deathRb.AddForce(-transform.forward * 10f, ForceMode.Impulse);

            Destroy(deathFX, 10f);
        }
    }
}