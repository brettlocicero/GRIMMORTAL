using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponObject : ItemObject
{
    [Header("Weapon References")]
    [SerializeField] Animation attackAnimation;
    [SerializeField] Transform hitSpot;
    [SerializeField] SigilInventory sigilInventory;

    [Header("Block")]
    [SerializeField] Vector3 blockPositionOffset;
    [SerializeField] Vector3 blockRotationOffset;
    [SerializeField] float blockTransitionSpeed = 10f;

    bool inAttack = false;
    bool isBlocking = false;

    WeaponSO weapon;
    WeaponStats stats = new();
    WeaponContext passiveContext;
    float attackTimer = 0f;
    int comboIndex = 0;

    AudioSource audioSource;
    CinemachineShakeManager cinemachineShake;

    float chargeTime = 0f;

    public bool IsBlocking => isBlocking;

    protected override void Start()
    {
        base.Start();
        weapon = item as WeaponSO;
        passiveContext = new WeaponContext { Stats = stats };
        audioSource = GetComponent<AudioSource>();
        cinemachineShake = CinemachineShakeManager.Instance;
    }

    void OnEnable()
    {
        attackAnimation.Rewind();
        inAttack = false;
        isBlocking = false;
    }

    protected override void Update()
    {
        base.Update();
        stats.Tick(Time.deltaTime);
        HandleBlock();
        HandleAttack();
    }

    bool IsBusy()
    {
        return playerController.SensitivityLocked;
    }

    bool CanAttack()
    {
        return !(inAttack || isBlocking || IsBusy());
    }

    void HandleBlock()
    {
        isBlocking = !inAttack && !IsBusy() && InputManager.Actions.Player.Block.IsPressed();

        Vector3 targetPos = isBlocking ? blockPositionOffset : Vector3.zero;
        Quaternion targetRot = isBlocking ? Quaternion.Euler(blockRotationOffset) : Quaternion.identity;

        posOffset = Vector3.Lerp(posOffset, targetPos, Time.deltaTime * blockTransitionSpeed);
        rotOffset = Quaternion.Slerp(rotOffset, targetRot, Time.deltaTime * blockTransitionSpeed);
    }

    WeaponContext CreateWeaponContext(Attack attack)
    {
        return new WeaponContext
        {
            Stats = stats,
            Damage = attack.damage,
            StunTime = attack.stunTime
        };
    }

    void HandleAttack()
    {
        attackTimer += Time.deltaTime;

        stats.AttackRate = weapon.attackRate;
        sigilInventory.Trigger(SigilTrigger.Passive, passiveContext);

        bool pressedAttack = InputManager.Actions.Player.Attack.WasPressedThisFrame();
        bool ready = attackTimer >= stats.AttackRate;

        if (pressedAttack && ready && CanAttack())
        {
            StartCoroutine(AttackWorker(weapon.attacks[comboIndex]));
            attackTimer = 0f;
        }
    }

    IEnumerator AttackWorker(Attack attack)
    {
        inAttack = true;
        WeaponContext weaponContext = CreateWeaponContext(attack);

        // Prepare VFX
        attackAnimation.Rewind();
        attackAnimation.Play(attack.animationClip.name);

        comboIndex = ++comboIndex % weapon.attacks.Length;

        yield return WindupWorker(attack);

        float chargePercent = attack.maxChargeTime > 0f ? chargeTime / attack.maxChargeTime : 0f;
        weaponContext.Damage = attack.damage * Mathf.Lerp(1f, attack.chargeDamageMultiplier, chargePercent) * stats.DamageMultiplier;

        sigilInventory.Trigger(SigilTrigger.OnAttack, weaponContext);

        cinemachineShake.ShakeCamera(attack.camShakeIntensity, attack.camShakeDuration, 0.5f, 80f);
        PlayAttackAudio();

        // if (attack.projectile) LaunchProjectile(attack);
        if (attack.useHitbox) TriggerAttackHitbox(attack, weaponContext);

        inAttack = false;
    }

    IEnumerator WindupWorker(Attack attack)
    {
        AnimationState state = attackAnimation[attack.animationClip.name];
        bool charging = attack.maxChargeTime > 0f;
        chargeTime = 0f;

        while (charging || state.time < attack.attackDelay)
        {
            charging = charging && chargeTime < attack.maxChargeTime && InputManager.Actions.Player.Attack.IsPressed();

            if (charging)
            {
                bool atApex = state.time >= attack.attackDelay;
                state.speed = atApex ? 0f : attack.chargeSpeed;
                chargeTime += Time.deltaTime;
            }

            else
            {
                state.speed = 1f;
            }

            yield return null;
        }

        state.speed = 1f;
    }

    // void LaunchProjectile(Attack attack)
    // {
    //     PlayerProjectile projectile = Instantiate(attack.projectile, hitSpot.position, hitSpot.rotation);

    //     projectile.GetComponent<Rigidbody>().AddForce(cameraTransform.forward * attack.projectileForce, ForceMode.Impulse);
    //     projectile.Init(attack);
    // }

    void TriggerAttackHitbox(Attack attack, WeaponContext weaponContext)
    {
        Collider[] hits = Physics.OverlapSphere(hitSpot.position, weapon.range, LayerMask.GetMask("Enemy", "Breakable"));

        if (hits.Length > 0)
        {
            audioSource.PlayOneShot(weapon.contactSound);
            StartCoroutine(HitstopWorker(attack.animationClip.name, 0.1f));
        }

        weaponContext.HitEnemies = hits
            .Select(hit => hit.GetComponent<EnemyAI>())
            .Where(enemy => enemy != null)
            .Distinct()
            .ToArray();

        if (weaponContext.HitEnemies.Length > 0)
            DamageEnemies(attack, weaponContext);
    }

    void DamageEnemies(Attack attack, WeaponContext weaponContext)
    {
        // Runs before damage so OnHit Sigils can still modify it
        sigilInventory.Trigger(SigilTrigger.OnHit, weaponContext);

        List<EnemyAI> killedEnemies = new();

        foreach (EnemyAI enemy in weaponContext.HitEnemies)
        {
            if (enemy.TakeDamage(weaponContext.Damage, weaponContext.StunTime, attack.direction))
                killedEnemies.Add(enemy);
        }

        weaponContext.KilledEnemies = killedEnemies.ToArray();

        if (killedEnemies.Count > 0)
            sigilInventory.Trigger(SigilTrigger.OnKill, weaponContext);
    }

    IEnumerator HitstopWorker(string clipName, float duration)
    {
        AnimationState state = attackAnimation[clipName];
        state.speed = 0f;

        yield return new WaitForSeconds(duration);

        state.speed = 1f;
    }

    void PlayAttackAudio()
    {
        audioSource.pitch = Random.Range(weapon.attackSoundPitchRange.x, weapon.attackSoundPitchRange.y);
        audioSource.PlayOneShot(weapon.attackSound);
    }
}