using System.Collections.Generic;
using UnityEngine;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace EFTUtils
{
    public static class ColorHelper
    {
        private static readonly Dictionary<Color, string> SaveHexColors = new Dictionary<Color, string>();

        public static string ColorToHtml(this Color color)
        {
            if (SaveHexColors.TryGetValue(color, out var hexColor))
                return hexColor;

            hexColor = $"#{ColorUtility.ToHtmlStringRGBA(color)}";

            SaveHexColors.Add(color, hexColor);

            return hexColor;
        }
    }
}