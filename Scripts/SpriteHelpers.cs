using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteHelpers : MonoBehaviour
{
    [System.Serializable]
    private struct FlashParams
    {
        public Color color;
        public float time;
    }

    [System.Serializable]
    private struct BlinkParams
    {
        public float blinkOnTime;
        public float blinkOffTime;
    }

    [SerializeField]
    private FlashParams flashParameters;
    [SerializeField]
    private BlinkParams blinkParameters;


    private SpriteRenderer spriteRenderer;

    private float          flashTimer;
    private Coroutine      flashCR;

    private Coroutine       blinkCR;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Flash()
    {
        flashTimer = flashParameters.time;

        if (flashCR == null) flashCR = StartCoroutine(FlashCR());
    }

    IEnumerator FlashCR()
    {
        Material prevMaterial = spriteRenderer.material;
        Material newMaterial = new Material(prevMaterial);
        newMaterial.shader = Shader.Find("Shader Graphs/FlashShader");

        spriteRenderer.material = newMaterial;

        Color c = flashParameters.color;

        while (flashTimer > 0)
        {
            c.a = flashTimer / flashParameters.time;
            newMaterial.SetColor("_FlashColor", c);

            flashTimer -= Time.deltaTime;

            yield return null;
        }

        spriteRenderer.material = prevMaterial;

        flashCR = null;
    }

    public void BlinkOn()
    {
        if (blinkCR == null) blinkCR = StartCoroutine(BlinkCR());
    }

    public void BlinkOff()
    {
        if (blinkCR != null)
        {
            StopCoroutine(blinkCR);
            spriteRenderer.enabled = true;
            blinkCR = null;
        }
    }

    IEnumerator BlinkCR()
    {
        while (true)
        {
            if (spriteRenderer == null) break;
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(blinkParameters.blinkOffTime);
            if (spriteRenderer == null) break;
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(blinkParameters.blinkOnTime);
        }
    }
}
