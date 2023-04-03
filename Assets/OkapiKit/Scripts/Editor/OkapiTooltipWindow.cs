using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace OkapiKit.Editor
{
    public class OkapiTooltipWindow : EditorWindow
    {
        private string tooltipText = "";
        private Color tooltipColor = Color.white;
        private DateTime tooltipTime;

        public void Show(Color color, string text)
        {
            wantsMouseMove = true;
            wantsMouseEnterLeaveWindow = true;

            tooltipText = text;
            tooltipColor = color;

            tooltipTime = DateTime.Now;

            var style = GUIUtils.GetTooltipTextStyle();
            var lines = tooltipText.Split('\n');
            Vector2 size = new Vector2(100, 20);

            foreach (var l in lines)
            {
                var tmp = style.CalcSize(new GUIContent(l));
                size.x = Mathf.Max(tmp.x, size.x);
                size.y = Mathf.Max(tmp.y, size.y);
            }

            size.y = lines.Length * (style.fixedHeight + 2);

            size.x += 10;
            size.y += 15;

            var newPos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            newPos.x -= 5;
            newPos.y -= size.y * 0.9f;
            position = new Rect(newPos, size);
            minSize = size;
        }

        private void Update()
        {
            if (tooltipText != "")
            {
                if (mouseOverWindow != this)
                {
                    if ((DateTime.Now - tooltipTime).Seconds > 1.0f)
                    {
                        Hide();
                    }
                }
                else
                {
                    tooltipTime = DateTime.Now;
                }
            }
        }

        public void Hide()
        {
            Close();
            tooltipText = "";
        }

        void OnGUI()
        {
            if (Event.current != null)
            {
                if (Event.current.type == EventType.MouseLeaveWindow)
                {
                    Hide();
                    return;
                }
            }

            // your GUI code here
            Rect boxRect = new Rect(0.0f, 0.0f, Screen.currentResolution.width, Screen.currentResolution.height);

            EditorGUI.DrawRect(boxRect, tooltipColor);

            Rect textRect = new Rect(5.0f, 5.0f, Screen.currentResolution.width - 10, Screen.currentResolution.height - 10);

            var style = GUIUtils.GetTooltipTextStyle();
            EditorGUI.LabelField(textRect, tooltipText, style);
        }
    }
}