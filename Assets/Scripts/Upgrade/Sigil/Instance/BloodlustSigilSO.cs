using UnityEngine;

[CreateAssetMenu(fileName = "BloodlustSigil", menuName = "Scriptable Objects/Sigils/BloodlustSigil")]
public class BloodlustSigilSO : SigilSO
{
    [SerializeField] float bonusPerKill = 0.05f;
    [SerializeField] float maxBonus = 0.5f;
    [SerializeField] float duration = 5f;

    public override void Apply(WeaponContext context, int stacks)
    {
        float bonus = bonusPerKill * stacks * context.KilledEnemies.Length;
        context.Stats.AddDamageBonus(bonus, maxBonus * stacks, duration);
    }
}