using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NodeEditor
{
    public static class EditorUtils
    {
        static public Color ColorFromHex(string htmlColor)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(htmlColor, out color)) return color;

            return Color.magenta;
        }

        static public Texture2D GetColorTexture(string name, Color color)
        {
            var ret = GetTexture(name);
            if (ret != null) return ret;

            var bitmap = new GUIBitmap(4, 4);
            bitmap.Fill(color);

            return BitmapToTexture(name, bitmap);
        }

        static Dictionary<string, Texture2D> textures;
        static public Texture2D AddTexture(string name, Texture2D texture)
        {
            if (textures == null) textures = new Dictionary<string, Texture2D>();

            textures[name] = texture;

            return texture;
        }

        static public Texture2D AddTexture(string name, GUIBitmap bitmap)
        {
            return BitmapToTexture(name, bitmap);
        }

        static public Texture2D GetTexture(string name)
        {
            if (textures == null) textures = new Dictionary<string, Texture2D>();

            Texture2D texture;
            if (textures.TryGetValue(name, out texture))
            {
                if (texture) return texture;
            }

            // Find path
            string path = $"Assets/OkapiKit/UI/{name}.png";
            if (!File.Exists(path))
            {
                path = System.IO.Path.GetFullPath($"Packages/com.videojogoslusofona.okapikit/UI/{name}.png");
                if (!File.Exists(path))
                {
                    return null;
                }
            }

            texture = new Texture2D(1, 1);
            if (texture.LoadImage(File.ReadAllBytes(path)))
            {
                texture.Apply();
                AddTexture(name, texture);
                return texture;
            }

            return null;
        }

        static public Texture2D BitmapToTexture(string name, GUIBitmap bitmap)
        {
            Texture2D result = new Texture2D(bitmap.width, bitmap.height);
            result.SetPixels(bitmap.bitmap);
            result.filterMode = FilterMode.Point;
            result.Apply();

            if (name != "")
            {
                AddTexture(name, result);
            }

            return result;
        }

        static public string ToReadableName(string str)
        {
            string ret = "";

            bool prevCaps = true;
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (char.IsUpper(c))
                {
                    if (prevCaps) ret += c;
                    else ret += $" {c}";

                    prevCaps = true;
                }
                else
                {
                    prevCaps = false;
                    if ((c == '_') || (c == ' ') || (c == '\t') || (c == '\n'))
                    {
                        if (ret.Length > 0) ret += ' ';
                    }
                    else ret += c;
                }
            }

            return ret;
        }
    }
}
