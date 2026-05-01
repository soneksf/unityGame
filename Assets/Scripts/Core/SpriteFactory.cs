using UnityEngine;

namespace ZorianyRaid.Core
{
    /// <summary>
    /// Утиліта для створення кольорових спрайтів у runtime.
    /// Дозволяє запускати гру без імпорту художнього контенту.
    /// </summary>
    public static class SpriteFactory
    {
        public static Sprite CreateRect(Color color, int width = 64, int height = 64)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
            tex.SetPixels(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f);
        }

        public static Sprite CreateTriangle(Color color, int size = 64)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;

            Color clear = new Color(0, 0, 0, 0);
            Color[] pixels = new Color[size * size];

            for (int y = 0; y < size; y++)
            {
                // Трикутник вістрям вгору
                int half = (size - y) / 2;
                int left = half;
                int right = size - half;
                for (int x = 0; x < size; x++)
                {
                    pixels[y * size + x] = (x >= left && x <= right) ? color : clear;
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
        }

        public static Sprite CreateCircle(Color color, int size = 64)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;

            Color clear = new Color(0, 0, 0, 0);
            float r = size * 0.5f;
            float r2 = r * r;
            Color[] pixels = new Color[size * size];

            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float dx = x - r + 0.5f;
                float dy = y - r + 0.5f;
                pixels[y * size + x] = (dx * dx + dy * dy <= r2) ? color : clear;
            }

            tex.SetPixels(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
        }
    }
}
