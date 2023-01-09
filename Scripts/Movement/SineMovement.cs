using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineMovement : Movement
{
    [SerializeField] private float amplitude = 10.0f;
    [SerializeField] private float frequency = 180.0f;
    [SerializeField] private bool  useLocal = true;

    public override Vector2 GetSpeed()
    {
        return new Vector2(frequency, frequency);
    }

    public override void SetSpeed(Vector2 speed)
    {
        frequency = speed.y;
    }

    private float angle = 0.0f;
    private Vector3 prevUp;

    private void Start()
    {
        prevUp = transform.up;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentPos = (useLocal)?(transform.localPosition) : (transform.position);

        Vector3 prevPos = currentPos - prevUp * amplitude * Mathf.Sin(angle * Mathf.Deg2Rad);
        angle += frequency * Time.deltaTime;
        prevUp = transform.up;
        currentPos = prevPos + prevUp * amplitude * Mathf.Sin(angle * Mathf.Deg2Rad);

        if (useLocal) transform.localPosition = currentPos;
        else transform.position = currentPos;
    }
}
