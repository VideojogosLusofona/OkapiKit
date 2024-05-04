using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NodeEditor
{
    public class GUIBitmap
    {
        public int width = 0;
        public int height = 0;
        public Color[] bitmap;

        public GUIBitmap(GUIBitmap src)
        {
            width = src.width;
            height = src.height;
            bitmap = new Color[width * height];
            Array.Copy(src.bitmap, bitmap, src.bitmap.Length);
        }

        public GUIBitmap(int width, int height)
        {
            this.width = width;
            this.height = height;

            bitmap = new Color[width * height];
        }

        public void Fill(Color color)
        {
            for (int i = 0; i < bitmap.Length; i++) bitmap[i] = color;
        }

        public void HLine(int x1, int x2, int y, Color color)
        {
            int baseIndex = y * width;

            for (int x = x1; x <= x2; x++) bitmap[baseIndex + x] = color;
        }

        public void VLine(int x, int y1, int y2, Color color)
        {
            int baseIndex = x + y1 * width;

            for (int y = y1; x <= y2; x++)
            {
                bitmap[baseIndex] = color;
                baseIndex += width;
            }
        }

        public void Multiply(Color m)
        {
            Color c;
            for (int idx = 0; idx < bitmap.Length; idx++)
            {
                c = bitmap[idx];
                c.r *= m.r;
                c.g *= m.g;
                c.b *= m.b;
                c.a *= m.a;
                bitmap[idx] = c;
            }
        }

        public void Border(Color c)
        {
            HLine(0, width - 1, 0, c);
            HLine(0, width - 1, height - 1, c);
            VLine(0, 0, height - 1, c);
            VLine(width - 1, 0, height - 1, c);
        }

        public void FillAlpha(Color m)
        {
            Color c;
            for (int idx = 0; idx < bitmap.Length; idx++)
            {
                c = bitmap[idx];
                c.r = c.r * c.a + m.r * (1 - c.a);
                c.g = c.g * c.a + m.g * (1 - c.a);
                c.b = c.b * c.a + m.b * (1 - c.a);
                c.a = m.a;
                bitmap[idx] = c;
            }
        }
    }
}
