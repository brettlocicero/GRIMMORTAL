using System.Collections.Generic;
using UnityEngine;

public class SigilInventory : MonoBehaviour
{
    [SerializeField] SigilSO[] startingSigils;

    Dictionary<SigilSO, int> sigils = new();

    void Awake()
    {
        foreach (SigilSO sigil in startingSigils)
            Add(sigil);
    }

    public void Add(SigilSO sigil)
    {
        sigils.TryGetValue(sigil, out int stacks);
        sigils[sigil] = stacks + 1;
    }

    public void Trigger(SigilTrigger trigger, WeaponContext context)
    {
        foreach (var (sigil, stacks) in sigils)
        {
            if (sigil.Trigger == trigger)
                sigil.Apply(context, stacks);
        }
    }
}