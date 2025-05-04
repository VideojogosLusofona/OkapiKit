using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace OkapiKit
{

    public class CombatTextManager : OkapiElement
    {
        static CombatTextManager instance;

        class TextElem
        {
            public Color startColor;
            public Color endColor;
            public float elapsedTime;
            public float totalTime;
            public float speedModifier;
            public float number;
            public bool isNumber;
            public string baseText;
            public GameObject ownerObject;
            public RectTransform textTransform;
            public TextMeshProUGUI textObject;
        }

        [SerializeField] private TextMeshProUGUI textPrefab;
        [SerializeField] private float defaultTime = 1.0f;
        [SerializeField] private Vector2 movementVector;
        [SerializeField] private float fadeRate = 1;
        
        Camera          uiCamera;
        List<TextElem>  textList;
        Canvas          canvas;
        RectTransform   rectTransform;
        Vector2         screenToCanvasSizes;
        CanvasScaler    canvasScaler;

        protected override void Awake()
        {
            base.Awake();

            if ((instance != null) && (instance != this))
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            textList = new List<TextElem>();
            canvas = GetComponentInParent<Canvas>();
            canvasScaler = canvas.GetComponent<CanvasScaler>();
            rectTransform = transform as RectTransform;
            if (uiCamera == null)
            {
                uiCamera = canvas.worldCamera;
                if (uiCamera == null)
                {
                    uiCamera = Camera.main;
                }
            }

            screenToCanvasSizes.x = canvasScaler.referenceResolution.x / Screen.width;
            screenToCanvasSizes.y = canvasScaler.referenceResolution.y / Screen.height;
        }

        void Update()
        {
            foreach (var tElem in textList)
            {
                tElem.elapsedTime += Time.deltaTime;

                if (tElem.elapsedTime >= tElem.totalTime)
                {
                    Destroy(tElem.textObject.gameObject);
                }
                else
                {
                    float t = Mathf.Pow(tElem.elapsedTime / tElem.totalTime, fadeRate);
                    Color c = Color.Lerp(tElem.startColor, tElem.endColor, t);

                    tElem.textObject.color = c;

                    tElem.textTransform.anchoredPosition += movementVector * tElem.speedModifier * Time.deltaTime;
                }
            }

            textList.RemoveAll((t) => t.elapsedTime >= t.totalTime);
        }

        TextElem NewText(GameObject ownerObject, Vector2 offset)
        {
            var tmp = new TextElem();

            tmp.number = 0.0f;
            tmp.ownerObject = ownerObject;
            tmp.elapsedTime = 0.0f;
            tmp.textObject = Instantiate(textPrefab, transform);
            tmp.textTransform = tmp.textObject.GetComponent<RectTransform>();

            ResetPosition(tmp, ownerObject, offset);

            textList.Add(tmp);

            return tmp;
        }

        void ResetPosition(TextElem text, GameObject ownerObject, Vector2 offset)
        {
            var ctSpawnPoint = ownerObject.GetComponentInChildren<CombatTextPivot>();
            Vector3 offset3d = offset;
            var position = (ctSpawnPoint == null) ? (ownerObject.transform.position + offset3d) : (ctSpawnPoint.transform.position);

            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(uiCamera, position);

            // Convert the screen point to local coordinates in the RectTransform
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, uiCamera, out Vector2 localPoint);

            text.textTransform.anchoredPosition = localPoint;
        }

        TextElem FindNumberTextOfColor(Color c, GameObject ownerObject, Vector2 offset)
        {
            foreach (var tElem in textList)
            {
                if (tElem.isNumber)
                {
                    if ((tElem.startColor == c) && (tElem.ownerObject == ownerObject))
                    {
                        return tElem;
                    }
                }
            }

            return NewText(ownerObject, offset);
        }

        void _SpawnText(GameObject ownerObject, Vector2 offset, string text, Color startColor, Color endColor, float time = 0.0f, float moveSpeedModifier = 1.0f)
        {
            TextElem newText = NewText(ownerObject, offset);
            newText.isNumber = false;
            newText.baseText = text;
            newText.startColor = startColor;
            newText.endColor = endColor;
            newText.speedModifier = moveSpeedModifier;
            newText.totalTime = (time > 0.0f) ? (time) : (defaultTime);

            newText.textObject.text = newText.baseText;
            newText.textObject.color = startColor;
        }

        void _SpawnText(GameObject ownerObject, float value, string text, Color startColor, Color endColor, float time = 0.0f, float moveSpeedModifier = 1.0f)
        {
            TextElem newText = FindNumberTextOfColor(startColor, ownerObject, Vector2.zero);
            newText.isNumber = true;
            newText.number += value;
            newText.baseText = text;
            newText.startColor = startColor;
            newText.endColor = endColor;
            newText.speedModifier = moveSpeedModifier;
            newText.totalTime = (time > 0.0f) ? (time) : (defaultTime);
            newText.elapsedTime = 0.0f;
            ResetPosition(newText, ownerObject, Vector2.zero);

            newText.textObject.text = string.Format(text, newText.number);
            newText.textObject.color = startColor;
        }

        void _SpawnText(GameObject ownerObject, Vector2 offset, float value, string text, Color startColor, Color endColor, float time = 0.0f, float moveSpeedModifier = 1.0f)
        {
            TextElem newText = FindNumberTextOfColor(startColor, ownerObject, offset);
            newText.isNumber = true;
            newText.number += value;
            newText.baseText = text;
            newText.startColor = startColor;
            newText.endColor = endColor;
            newText.speedModifier = moveSpeedModifier;
            newText.totalTime = (time > 0.0f) ? (time) : (defaultTime);
            newText.elapsedTime = 0.0f;
            ResetPosition(newText, ownerObject, Vector2.zero);

            newText.textObject.text = string.Format(text, newText.number);
            newText.textObject.color = startColor;
        }
        public override string GetRawDescription(string ident, GameObject refObject)
        {
            var desc = "This allows the developer to use combat text easily.\n";

            desc += $"Every combat text will spawn (or reuse) a {textPrefab?.name ?? "UNDEFINED"} prefab.\n";
            desc += $"It will move {movementVector} units per second, for {defaultTime} seconds, fading out in {fadeRate} seconds.\n";

            return desc;
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if (textPrefab == null)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "Need a prefab to spawn!", "The prefab has to have at least a TextMeshProUGUI component that gets modified each time a combat text is spawned!"));
            }

            canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "This manager needs to be underneath a Canvas element.", "This manager needs to be placed in a child object underneath a Canvas element."));
            }
            else
            {
                if (canvas.renderMode != RenderMode.ScreenSpaceCamera)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Canvas needs to be a screen space camera canvas!", "Canvas needs to be a screen space camera canvas, otherwise positions won't work properly."));
                }
                canvasScaler = canvas.GetComponent<CanvasScaler>();
                if (canvasScaler == null)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Canvas needs a canvas scaler attached to it!", "Canvas needs a CanvasScaler component attached to it, otherwise positions won't work properly."));
                }
                uiCamera = canvas.worldCamera;
                if (uiCamera == null)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Canvas needs a camera configured!", "Canvas needs a camera configured!"));
                }
            }
        }

        public static bool IsActive()
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<CombatTextManager>();
            }

            return (instance != null);
        }

        public static void SpawnText(GameObject ownerObject, string text, Color startColor, Color endColor, float time = 0.0f, float moveSpeedModifier = 1.0f)
        {
            instance?._SpawnText(ownerObject, Vector2.zero, text, startColor, endColor, time, moveSpeedModifier);
        }
        public static void SpawnText(GameObject ownerObject, Vector2 offset, string text, Color startColor, Color endColor, float time = 0.0f, float moveSpeedModifier = 1.0f)
        {
            instance?._SpawnText(ownerObject, offset, text, startColor, endColor, time, moveSpeedModifier);
        }

        public static void SpawnText(GameObject ownerObject, float value, string text, Color startColor, Color endColor, float time = 0.0f, float moveSpeedModifier = 1.0f)
        {
            instance?._SpawnText(ownerObject, Vector2.zero, value, text, startColor, endColor, time, moveSpeedModifier);
        }

        public static void SpawnText(GameObject ownerObject, Vector2 offset, float value, string text, Color startColor, Color endColor, float time = 0.0f, float moveSpeedModifier = 1.0f)
        {
            instance?._SpawnText(ownerObject, offset, value, text, startColor, endColor, time, moveSpeedModifier);
        }
    }
}