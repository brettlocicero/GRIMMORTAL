using UnityEngine;

public abstract class SigilSO : ScriptableObject
{
    [SerializeField] string sigilName;
    [SerializeField, TextArea] string description;
    [SerializeField] Sprite icon;
    [SerializeField] SigilTrigger trigger;

    public string SigilName => sigilName;
    public string Description => description;
    public Sprite Icon => icon;
    public SigilTrigger Trigger => trigger;

    public abstract void Apply(WeaponContext context, int stacks);
}