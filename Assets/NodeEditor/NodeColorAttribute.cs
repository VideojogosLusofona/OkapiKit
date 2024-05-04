using System;
using UnityEngine;

namespace NodeEditor
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class NodeColorAttribute : Attribute
    {
        public Color bgColor { get; }
        public Color textColor { get; }
        public Color separatorColor { get; }

        public NodeColorAttribute(string hexBgColor, string hexTextColor, string hexSeparatorColor)
        {
            bgColor = ColorFromHex(hexBgColor);
            textColor = ColorFromHex(hexTextColor);
            separatorColor = ColorFromHex(hexSeparatorColor);
        }

        static public Color ColorFromHex(string htmlColor)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(htmlColor, out color)) return color;

            return Color.magenta;
        }

    }
}
