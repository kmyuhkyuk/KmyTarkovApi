using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace EFTReflection.Patching
{
    public static class HookPatch
    {
        private static readonly Dictionary<MethodBase, Dictionary<Type, (Harmony Harmony, int MethodCount)>>
            HookDictionary = new Dictionary<MethodBase, Dictionary<Type, (Harmony Harmony, int MethodCount)>>();

        public static void Add(MethodBase original, Delegate hookDelegate,
            HarmonyPatchType patchType = HarmonyPatchType.Postfix)
        {
            var originalDeclaringType = original.DeclaringType;

            if (originalDeclaringType == null)
            {
                throw new ArgumentNullException(nameof(originalDeclaringType));
            }

            var hookDelegateMethod = hookDelegate.Method;

            var hookDeclaringType = hookDelegateMethod.DeclaringType;

            if (hookDeclaringType == null)
            {
                throw new ArgumentNullException(nameof(hookDeclaringType));
            }

            if (!hookDelegateMethod.IsStatic)
            {
                throw new Exception($"{hookDeclaringType.Name}_{hookDelegateMethod.Name} not is static");
            }

            var hasHook = HookDictionary.TryGetValue(original, out var typeDictionary);

            if (hasHook && typeDictionary.TryGetValue(hookDeclaringType, out var harmonyTuple))
            {
                Patch(harmonyTuple.Harmony, original, hookDelegateMethod, patchType);
                harmonyTuple.MethodCount++;
            }
            else
            {
                var harmony =
                    new Harmony(
                        $"Hook_{originalDeclaringType.Name}_{original.Name}:{hookDeclaringType.Name}_{hookDelegateMethod.Name}");

                Patch(harmony, original, hookDelegateMethod, patchType);

                if (hasHook)
                {
                    typeDictionary.Add(hookDeclaringType, (harmony, 1));
                }
                else
                {
                    HookDictionary.Add(original, new Dictionary<Type, (Harmony Harmony, int MethodCount)>
                    {
                        { hookDeclaringType, (harmony, 1) }
                    });
                }
            }
        }

        private static void Patch(Harmony harmony, MethodBase original, MethodInfo delegateMethod,
            HarmonyPatchType patchType)
        {
            var harmonyMethod = new HarmonyMethod(delegateMethod);

            switch (patchType)
            {
                case HarmonyPatchType.All:
                    throw new Exception("Can't Patch All Type");
                case HarmonyPatchType.Prefix:
                    harmony.Patch(original, prefix: harmonyMethod);
                    break;
                case HarmonyPatchType.Postfix:
                    harmony.Patch(original, postfix: harmonyMethod);
                    break;
                case HarmonyPatchType.Transpiler:
                    harmony.Patch(original, transpiler: harmonyMethod);
                    break;
                case HarmonyPatchType.Finalizer:
                    harmony.Patch(original, finalizer: harmonyMethod);
                    break;
                case HarmonyPatchType.ReversePatch:
                    harmony.CreateReversePatcher(original, harmonyMethod).Patch();
                    break;
                case HarmonyPatchType.ILManipulator:
                    harmony.Patch(original, ilmanipulator: harmonyMethod);
                    break;
            }
        }

        public static void Remove(MethodBase original, Delegate hookDelegate)
        {
            var originalDeclaringType = original.DeclaringType;

            if (originalDeclaringType == null)
            {
                throw new ArgumentNullException(nameof(originalDeclaringType));
            }

            var hookDelegateMethod = hookDelegate.Method;

            var hookDeclaringType = hookDelegateMethod.DeclaringType;

            if (hookDeclaringType == null)
            {
                throw new ArgumentNullException(nameof(hookDeclaringType));
            }

            if (!hookDelegateMethod.IsStatic)
            {
                throw new Exception($"{hookDeclaringType.Name}_{hookDelegateMethod.Name} not is static");
            }

            if (HookDictionary.TryGetValue(original, out var typeDictionary) &&
                typeDictionary.TryGetValue(hookDeclaringType, out var harmonyTuple))
            {
                harmonyTuple.Harmony.Unpatch(original, hookDelegateMethod);
                harmonyTuple.MethodCount--;

                if (harmonyTuple.MethodCount == 0)
                {
                    typeDictionary.Remove(hookDeclaringType);
                }

                if (typeDictionary.Count == 0)
                {
                    HookDictionary.Remove(original);
                }
            }
        }
    }
}