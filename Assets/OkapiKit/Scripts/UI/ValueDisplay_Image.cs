using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueDisplay_Image : ValueDisplay
{
    // Update is called once per frame
    void Update()
    {
        var v = GetVariable();
        if (v == null) return;

        int i = 0;
        
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(i < v.currentValue);
            i++;
        }
    }
}
