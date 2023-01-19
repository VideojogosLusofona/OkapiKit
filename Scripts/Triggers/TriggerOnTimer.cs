using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class TriggerOnTimer : Trigger
{
    [SerializeField]
    private bool    startTriggered = false;
    [SerializeField, MinMaxSlider(0.0f, 10.0f)]
    private Vector2 timeInterval = new Vector2(5.0f, 5.0f);

    private float timer;

    protected override string GetRawDescription()
    {
        if (timeInterval.x == timeInterval.y)
        {
            return $"Every {timeInterval.x} seconds, trigger the actions:";
        }

        return $"Every [{timeInterval.x},{timeInterval.y}] seconds, trigger the actions:";
    }

    void Start()
    {
        if (startTriggered)
        {
            ExecuteTrigger();
        }
        timer = Random.Range(timeInterval.x, timeInterval.y);        
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            if (EvaluatePreconditions())
            {
                ExecuteTrigger();
            }

            timer += Random.Range(timeInterval.x, timeInterval.y);
        }
    }
}
