using System.Collections.Generic;
using UnityEngine;

public class DebuffHandler : MonoBehaviour
{
    List<Debuff> debuffs = new();
    EnemyAI enemy;

    void Awake()
    {
        enemy = GetComponent<EnemyAI>();
    }

    void Update()
    {
        for (int i = debuffs.Count - 1; i >= 0; i--)
        {
            debuffs[i].Update(enemy, Time.deltaTime);

            if (debuffs[i].IsFinished)
                debuffs.RemoveAt(i);
        }
    }

    public static void Apply(EnemyAI enemy, Debuff debuff)
    {
        if (!enemy.TryGetComponent(out DebuffHandler handler))
            handler = enemy.gameObject.AddComponent<DebuffHandler>();

        handler.debuffs.RemoveAll(d => d.GetType() == debuff.GetType());
        handler.debuffs.Add(debuff);
    }
}