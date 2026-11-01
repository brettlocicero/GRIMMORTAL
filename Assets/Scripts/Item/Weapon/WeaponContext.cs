public class WeaponContext
{
    public EnemyAI[] HitEnemies { get; set; } = System.Array.Empty<EnemyAI>();
    public EnemyAI[] KilledEnemies { get; set; } = System.Array.Empty<EnemyAI>();
    public float Damage { get; set; }
    public float StunTime { get; set; }
}