using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImageBorderTool2.Helpers
{
    public static class ColorConverter
    {

        public static Color HexToRgb(string hex)
        {
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);

            if (hex.Length == 6)
                hex = "FF" + hex;

            byte a = (byte)int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte r = (byte)int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = (byte)int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = (byte)int.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);

            return Color.FromRgba(r, g, b, a);
        }

        public static Color HslToRgb(float h, float s, float l)
        {
            float r, g, b;

            if (s == 0)
            {
                r = g = b = l;
            }
            else
            {
                float hueToRgb(float p, float q, float t)
                {
                    if (t < 0) t += 1;
                    if (t > 1) t -= 1;
                    if (t < 1f / 6f) return p + (q - p) * 6 * t;
                    if (t < 1f / 2f) return q;
                    if (t < 2f / 3f) return p + (q - p) * (2f / 3f - t) * 6;
                    return p;
                }

                float q = l < 0.5f ? l * (1 + s) : l + s - l * s;
                float p = 2 * l - q;

                r = hueToRgb(p, q, h + 1f / 3f);
                g = hueToRgb(p, q, h);
                b = hueToRgb(p, q, h - 1f / 3f);
            }

            return Color.FromRgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
        }
    }

}
