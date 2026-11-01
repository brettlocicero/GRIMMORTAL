public class BurnDebuff : Debuff
{
    float damage;

    public BurnDebuff(float damage, float interval, int ticks) : base(interval, ticks)
    {
        this.damage = damage;
    }

    protected override void OnTick(EnemyAI target)
    {
        target.TakeDamage(damage, 0f, AttackDirection.None);
    }
}