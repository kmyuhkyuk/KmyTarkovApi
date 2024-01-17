using System;
using TMPro;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable Unity.UnknownResource
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace EFTUtils
{
    public static class EFTFontHelper
    {
        public static readonly TMP_FontAsset BenderNormal =
            Resources.Load<TMP_FontAsset>("ui/fonts/jovanny lemonad - bender normal sdf");

        public static readonly TMP_FontAsset BenderOutline =
            Resources.Load<TMP_FontAsset>("ui/fonts/jovanny lemonad - bender outline sdf");

        public static readonly TMP_FontAsset BenderOverlay =
            Resources.Load<TMP_FontAsset>("ui/fonts/jovanny lemonad - bender overlay sdf");

        public static readonly TMP_FontAsset BenderShadowed =
            Resources.Load<TMP_FontAsset>("ui/fonts/jovanny lemonad - bender shadowed sdf");

        public static GameObject ReplaceAllFont(this GameObject parent, TMP_FontAsset toFontAsset)
        {
            return parent.ReplaceAllFont(_ => true, toFontAsset);
        }

        public static GameObject ReplaceAllFont(this GameObject parent,
            Func<TMP_Text, bool> predicate, TMP_FontAsset toFontAsset)
        {
            foreach (var tmpText in parent.GetComponentsInChildren<TMP_Text>(true))
            {
                if (predicate(tmpText))
                {
                    tmpText.font = toFontAsset;
                }
            }

            return parent;
        }
    }
}