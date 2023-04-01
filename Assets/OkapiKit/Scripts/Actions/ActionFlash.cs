using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    public class ActionFlash : Action
    {
        [SerializeField] private Renderer target;
        [SerializeField] private Gradient color;
        [SerializeField] private float duration = 1.0f;

        float timer;

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;
            if (target == null) return;

            timer = duration;

            StartCoroutine(FlashCR());
        }

        IEnumerator FlashCR()
        {
            var originalMaterial = target.material;

            Material newMaterial = new Material(originalMaterial);
            newMaterial.shader = Shader.Find("Shader Graphs/FlashShader");
            target.material = newMaterial;

            timer = duration;

            while (timer > 0)
            {
                float t = 1.0f - (timer / duration);
                Color c = color.Evaluate(t);

                newMaterial.SetColor("_FlashColor", c);

                timer -= Time.deltaTime;

                yield return null;
            }

            target.material = originalMaterial;
        }

        public override string GetActionTitle() => "Flash";
        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string desc = GetPreconditionsString(gameObject);

            if (target == null)
            {
                desc += $"flashes this renderer for {duration} seconds";
            }
            else
            {
                desc += $"flashes renderer {target.name} for {duration} seconds";
            }

            return desc;
        }

        protected override void Awake()
        {
            base.Awake();

            if (target == null)
            {
                target = GetComponent<Renderer>();
                if (target == null)
                {
                    target = GetComponentInChildren<Renderer>();
                }
            }
        }
    }
}