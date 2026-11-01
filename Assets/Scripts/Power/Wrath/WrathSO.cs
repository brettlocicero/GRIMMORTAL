using UnityEngine;

public abstract class WrathSO : ScriptableObject
{
    [SerializeField] string wrathName;
    [SerializeField, TextArea] string description;
    [SerializeField] Sprite icon;
    [SerializeField] int chargeRequired = 20;

    public string WrathName => wrathName;
    public string Description => description;
    public Sprite Icon => icon;
    public int ChargeRequired => chargeRequired;

    public abstract void Activate(PlayerController player);
}