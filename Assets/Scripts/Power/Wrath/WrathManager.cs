using UnityEngine;
using UnityEngine.InputSystem;

public class WrathManager : MonoBehaviour
{
    [SerializeField] WrathSO[] wraths;
    [SerializeField] PlayerController player;

    [Header("UI")]
    [SerializeField] WrathSlotUI slotPrefab;
    [SerializeField] Transform slotParent;

    int[] charges;
    InputAction[] inputs;

    void Awake()
    {
        charges = new int[wraths.Length];

        inputs = new[]
        {
            InputManager.Actions.Player.Wrath1,
            InputManager.Actions.Player.Wrath2,
            InputManager.Actions.Player.Wrath3
        };

        SpawnSlots();
    }

    void SpawnSlots()
    {
        for (int i = 0; i < wraths.Length; i++)
            Instantiate(slotPrefab, slotParent).Init(this, i);
    }

    void OnEnable()
    {
        EnemyAI.OnAnyEnemyKilled += AddCharge;
    }

    void OnDisable()
    {
        EnemyAI.OnAnyEnemyKilled -= AddCharge;
    }

    void Update()
    {
        for (int i = 0; i < wraths.Length; i++)
        {
            if (IsReady(i) && inputs[i].WasPressedThisFrame())
                Activate(i);
        }
    }

    public bool IsReady(int slot)
    {
        return charges[slot] >= wraths[slot].ChargeRequired;
    }

    public float GetChargePercent(int slot)
    {
        return (float)charges[slot] / wraths[slot].ChargeRequired;
    }

    void AddCharge(EnemyAI enemy)
    {
        for (int i = 0; i < wraths.Length; i++)
            charges[i] = Mathf.Min(charges[i] + enemy.Threat, wraths[i].ChargeRequired);
    }

    void Activate(int slot)
    {
        charges[slot] = 0;
        wraths[slot].Activate(player);
    }

    public WrathSO GetWrath(int slot)
    {
        return wraths[slot];
    }

    public string GetKeyName(int slot)
    {
        return inputs[slot].GetBindingDisplayString();
    }
}