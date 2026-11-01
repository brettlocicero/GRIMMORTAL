using UnityEngine;

public class WeaponStats
{
    float damageBonus;
    float damageBonusTimer;

    public float AttackRate { get; set; }
    public float DamageMultiplier => 1f + damageBonus;

    public void AddDamageBonus(float amount, float max, float duration)
    {
        damageBonus = Mathf.Min(damageBonus + amount, max);
        damageBonusTimer = duration;
    }

    public void Tick(float deltaTime)
    {
        damageBonusTimer -= deltaTime;

        if (damageBonusTimer <= 0f)
            damageBonus = 0f;
    }
}