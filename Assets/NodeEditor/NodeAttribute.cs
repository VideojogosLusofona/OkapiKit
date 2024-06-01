using System;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// Attributes for BaseNode and derived
/// 
/// NodeColor allows to define the color of the title bar of the node on the NodeEditor:
///     [NodeColor("#D0FFFF", "#2f4858", "#86CBFF")] => NodeColor(bgColor, textColor, separator color)
/// NodePath allows to define the path for the new menu in the NodeEditor
///     [NodePath("Triggers/On Every Frame")] => NodePath(fullPath)
/// NodeDefaultPropertyVisibility allows to define if the elements on this class should be visible or not in the node display
///     [NodeDefaultPropertyVisibility(true)]
/// NodeWidth allows to define the width of the node in the NodeEditor
///     [NodeWidth(300.0f)]

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

    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class NodeDefaultPropertyVisibilityAttribute : Attribute
    {
        public bool defaultVisibility { get; }

        public NodeDefaultPropertyVisibilityAttribute(bool visibility)
        {
            defaultVisibility = visibility;
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class NodePathAttribute : Attribute
    {
        public string fullPath { get; }
        public string path { get; }
        public string name { get; }

        public NodePathAttribute(string fullPath)
        {
            this.fullPath = fullPath;

            path = fullPath;

            int idx = path.LastIndexOf('/');
            if (idx != -1)
            {
                path = fullPath.Substring(0, idx);
                name = fullPath.Substring(idx + 1);
            }
            else
            {
                path = "";
                name = fullPath;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class NodeWidthAttribute : Attribute
    {
        public float width { get; }

        public NodeWidthAttribute(float width)
        {
            this.width = width;
        }
    }

}
