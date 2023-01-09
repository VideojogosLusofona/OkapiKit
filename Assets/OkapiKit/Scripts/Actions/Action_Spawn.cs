using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Spawn : Action
{
    private Spawner spawner;

    public override string GetRawDescription(string ident)
    {
        return $"Spawns an entity using spawner {name}";
    }

    private void Start()
    {
        spawner = GetComponent<Spawner>();
    }

    public override void Execute()
    {
        if (!enableAction) return;

        if (spawner != null)
        {
            spawner.Spawn();
        }
    }
}
