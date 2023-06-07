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
            var declaringType = hookDelegate.Method.DeclaringType;

            if (declaringType == null)
            {
                throw new ArgumentNullException(nameof(declaringType));
            }

            var delegateMethod = hookDelegate.Method;

            if (!delegateMethod.IsStatic)
            {
                throw new Exception($"{declaringType.Name}_{delegateMethod.Name} not is static");
            }

            var hasHook = HookDictionary.TryGetValue(original, out var typeDictionary);

            if (hasHook && typeDictionary.TryGetValue(declaringType, out var harmonyTuple))
            {
                Patch(harmonyTuple.Harmony, original, hookDelegate.Method, patchType);
                harmonyTuple.MethodCount++;
            }
            else
            {
                var harmony = new Harmony($"{declaringType.Name}_Hook_{declaringType.Name}_{original.Name}");

                Patch(harmony, original, hookDelegate.Method, patchType);

                if (hasHook)
                {
                    typeDictionary.Add(declaringType, (harmony, 1));
                }
                else
                {
                    HookDictionary.Add(original, new Dictionary<Type, (Harmony Harmony, int MethodCount)>
                    {
                        { declaringType, (harmony, 1) }
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

        public static void Remove(MethodBase method, Delegate hookDelegate)
        {
            var declaringType = hookDelegate.Method.DeclaringType;

            if (declaringType == null)
            {
                throw new ArgumentNullException(nameof(declaringType));
            }

            var delegateMethod = hookDelegate.Method;

            if (!delegateMethod.IsStatic)
            {
                throw new Exception($"{nameof(delegateMethod)} not is static");
            }

            if (HookDictionary.TryGetValue(method, out var typeDictionary) &&
                typeDictionary.TryGetValue(declaringType, out var harmonyTuple))
            {
                harmonyTuple.Harmony.Unpatch(method, delegateMethod);
                harmonyTuple.MethodCount--;

                if (harmonyTuple.MethodCount == 0)
                {
                    typeDictionary.Remove(declaringType);
                }

                if (typeDictionary.Count == 0)
                {
                    HookDictionary.Remove(method);
                }
            }
        }
    }
}