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
            if (textures == null)
            {
                textures = new Dictionary<string, Texture2D>();
            }
            else
            {
                Texture2D texture;
                if (textures.TryGetValue(name, out texture))
                {
                    if (texture) return texture;
                }
            }

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

        struct TexturePath
        {
            public string path;
            public string packagePath;
        }
        static List<TexturePath> texturePaths = new List<TexturePath>();

        static public void AddTexturePath(string path, string packagePath)
        {
            foreach (var texturePath in texturePaths) 
            { 
                if ((path == texturePath.path) &&
                    (packagePath == texturePath.packagePath))
                {
                    return;
                }
            }

            texturePaths.Add(new TexturePath { path = path, packagePath = packagePath });
        }

        static public Texture2D GetTexture(string name)
        {
            if (textures == null) textures = new Dictionary<string, Texture2D>();

            Texture2D texture;
            if (textures.TryGetValue(name, out texture))
            {
                if (texture) return texture;
            }

            string path = null;
            foreach (var texturePath in texturePaths)
            {
                // Find path
                string tmp = $"{texturePath.path}/{name}.png";
                if (!File.Exists(tmp))
                {
                    tmp = System.IO.Path.GetFullPath($"{texturePath.packagePath}/{name}.png");
                    if (File.Exists(tmp))
                    {
                        path = tmp;
                        break;
                    }
                }
                else
                {
                    path = tmp;
                    break;
                }
            }

            if (path == null)
            {
                Debug.LogWarning($"Can't find texture {name}!");
                Debug.LogWarning("Searched for:");
                foreach (var texturePath in texturePaths)
                {
                    // Find path
                    string tmp = $"{texturePath.path}/{name}.png";
                    Debug.LogWarning(tmp);
                    tmp = System.IO.Path.GetFullPath($"{texturePath.packagePath}/{name}.png");
                    Debug.LogWarning(tmp);
                }
                return null;
            }

            texture = new Texture2D(1, 1);
            if (texture.LoadImage(File.ReadAllBytes(path)))
            {
                texture.Apply();
                AddTexture(name, texture);
                return texture;
            }

            Debug.LogWarning($"Failed to load texture {path}!");

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
