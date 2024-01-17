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
            return parent.ReplaceTargetFont(_ => true, toFontAsset);
        }

        public static GameObject ReplaceTargetFont(this GameObject parent, TMP_FontAsset fromFontAsset,
            TMP_FontAsset toFontAsset)
        {
            return parent.ReplaceTargetFont(x => x == fromFontAsset, toFontAsset);
        }

        public static GameObject ReplaceTargetFont(this GameObject parent,
            Func<TMP_FontAsset, bool> fromFontAssetPredicate, TMP_FontAsset toFontAsset)
        {
            foreach (var tmpText in parent.GetComponentsInChildren<TMP_Text>(true))
            {
                if (fromFontAssetPredicate(tmpText.font))
                {
                    tmpText.font = toFontAsset;
                }
            }

            return parent;
        }
    }
}