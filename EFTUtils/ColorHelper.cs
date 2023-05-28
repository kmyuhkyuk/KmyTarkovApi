using System.Collections.Generic;
using UnityEngine;

namespace EFTUtils
{
    public static class ColorHelper
    {
        private static readonly Dictionary<Color, string> HexColorPool = new Dictionary<Color, string>();

        public static string ColorToHtml(this Color color)
        {
            if (HexColorPool.TryGetValue(color, out var hexColor))
            {
                return hexColor;
            }
            else
            {
                hexColor = string.Concat("#", ColorUtility.ToHtmlStringRGBA(color));

                HexColorPool.Add(color, hexColor);

                return hexColor;
            }
        }
    }
}
