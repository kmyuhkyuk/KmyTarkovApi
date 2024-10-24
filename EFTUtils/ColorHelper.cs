using System.Collections.Generic;
using UnityEngine;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace EFTUtils
{
    public static class ColorHelper
    {
        private static readonly Dictionary<Color, string> HtmlColorDictionary = new Dictionary<Color, string>();

        public static string ColorToHtml(this Color color)
        {
            if (HtmlColorDictionary.TryGetValue(color, out var htmlColor))
                return htmlColor;

            htmlColor = $"#{ColorUtility.ToHtmlStringRGBA(color)}";

            HtmlColorDictionary.Add(color, htmlColor);

            return htmlColor;
        }
    }
}