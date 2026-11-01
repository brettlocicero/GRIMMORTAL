using UnityEngine;

[CreateAssetMenu(fileName = "SwiftHandsSigil", menuName = "Scriptable Objects/Sigils/SwiftHandsSigil")]
public class SwiftHandsSigilSO : SigilSO
{
    [SerializeField, Range(0f, 0.9f)] float speedBonus = 0.2f;

    public override void Apply(WeaponContext context, int stacks)
    {
        context.Stats.AttackRate *= Mathf.Pow(1f - speedBonus, stacks);
    }
}