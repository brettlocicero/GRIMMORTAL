using UnityEngine;

[CreateAssetMenu(fileName = "BurnSigil", menuName = "Scriptable Objects/Sigils/BurnSigil")]
public class BurnSigilSO : SigilSO
{
    [SerializeField] float damagePerTick = 3f;
    [SerializeField] float tickInterval = 0.5f;
    [SerializeField] int ticks = 5;

    public override void Apply(WeaponContext context, int stacks)
    {
        foreach (EnemyAI enemy in context.HitEnemies)
            DebuffHandler.Apply(enemy, new BurnDebuff(damagePerTick * stacks, tickInterval, ticks));
    }
}