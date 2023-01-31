using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueDisplay : MonoBehaviour
{
    [SerializeField, ShowIf("needValueHandler")]
    protected VariableInstance valueHandler;
    [SerializeField, ShowIf("needValueVariable")]
    protected Variable variable;

    private bool needValueHandler => (variable == null);
    private bool needValueVariable => (valueHandler == null);

    protected Variable GetVariable()
    {
        if (variable) return variable;

        if (valueHandler)
        {
            return valueHandler.GetVariable();
        }

        return null;
    }
}
