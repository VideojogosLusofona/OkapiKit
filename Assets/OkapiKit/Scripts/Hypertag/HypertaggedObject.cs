using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HypertaggedObject : MonoBehaviour
{
    [SerializeField] private Hypertag[] hypertags;

    public bool Has(Hypertag tag)
    {
        foreach (var t in hypertags)
        {
            if (t == tag) return true;
        }

        return false;
    }
}
