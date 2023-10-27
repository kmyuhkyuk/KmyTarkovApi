using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

// ReSharper disable MemberCanBePrivate.Global

namespace EFTReflection.Patching
{
    public static class HookPatch
    {
        private static readonly Dictionary<MethodBase, Dictionary<Type, Harmony>>
            HookDictionary = new Dictionary<MethodBase, Dictionary<Type, Harmony>>();

        private static readonly Dictionary<MethodBase, MethodInfo>
            VirtualMethodDictionary = new Dictionary<MethodBase, MethodInfo>();

        /// <summary>
        ///     Add a Hook Patch by <see cref="Delegate" />
        /// </summary>
        /// <remarks>It can make your method parameter still output null if Original Method does not exist parameter</remarks>
        /// <param name="originalMethod"></param>
        /// <param name="hookDelegate"></param>
        /// <param name="patchType"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public static void Add(MethodBase originalMethod, Delegate hookDelegate,
            HarmonyPatchType patchType = HarmonyPatchType.Postfix)
        {
            Add(originalMethod, hookDelegate.Method, patchType);
        }

        /// <summary>
        ///     Add a Hook Patch by <see cref="MethodInfo" />
        /// </summary>
        /// <remarks>It can make your method parameter still output null if Original Method does not exist parameter</remarks>
        /// <param name="originalMethod"></param>
        /// <param name="hookMethod"></param>
        /// <param name="patchType"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public static void Add(MethodBase originalMethod, MethodInfo hookMethod,
            HarmonyPatchType patchType = HarmonyPatchType.Postfix)
        {
            if (originalMethod == null)
            {
                throw new ArgumentNullException(nameof(originalMethod));
            }

            var originalDeclaringType = originalMethod.DeclaringType;

            if (originalDeclaringType == null)
            {
                throw new ArgumentNullException(nameof(originalDeclaringType));
            }

            if (hookMethod == null)
            {
                throw new ArgumentNullException(nameof(hookMethod));
            }

            var hookDeclaringType = hookMethod.DeclaringType;

            if (hookDeclaringType == null)
            {
                throw new ArgumentNullException(nameof(hookDeclaringType));
            }

            if (!hookMethod.IsStatic)
            {
                throw new Exception($"{hookDeclaringType.Name}_{hookMethod.Name} not is static");
            }

            var hasHook = HookDictionary.TryGetValue(originalMethod, out var typeDictionary);

            if (hasHook && typeDictionary.TryGetValue(hookDeclaringType, out var harmony))
            {
                Patch(harmony, originalMethod, hookMethod, patchType);
            }
            else
            {
                harmony =
                    new Harmony(
                        $"Hook_{originalDeclaringType.Name}_{originalMethod.Name}:{hookDeclaringType.Name}_{hookMethod.Name}");

                Patch(harmony, originalMethod, hookMethod, patchType);

                if (hasHook)
                {
                    typeDictionary.Add(hookDeclaringType, harmony);
                }
                else
                {
                    HookDictionary.Add(originalMethod, new Dictionary<Type, Harmony>
                    {
                        { hookDeclaringType, harmony }
                    });
                }
            }
        }

        private static void Patch(Harmony harmony, MethodBase originalMethod, MethodInfo hookMethod,
            HarmonyPatchType patchType)
        {
            switch (patchType)
            {
                case HarmonyPatchType.All:
                    throw new Exception("Can't Patch All Type");
                case HarmonyPatchType.Prefix:
                    harmony.Patch(originalMethod, prefix: new HarmonyMethod(VirtualMethod(originalMethod, hookMethod)));
                    break;
                case HarmonyPatchType.Postfix:
                    harmony.Patch(originalMethod,
                        postfix: new HarmonyMethod(VirtualMethod(originalMethod, hookMethod)));
                    break;
                case HarmonyPatchType.Transpiler:
                    harmony.Patch(originalMethod, transpiler: new HarmonyMethod(hookMethod));
                    break;
                case HarmonyPatchType.Finalizer:
                    harmony.Patch(originalMethod,
                        finalizer: new HarmonyMethod(VirtualMethod(originalMethod, hookMethod)));
                    break;
                case HarmonyPatchType.ReversePatch:
                    harmony.CreateReversePatcher(originalMethod, new HarmonyMethod(hookMethod)).Patch();
                    break;
                case HarmonyPatchType.ILManipulator:
                    harmony.Patch(originalMethod, ilmanipulator: new HarmonyMethod(hookMethod));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(patchType), patchType, null);
            }
        }

        private static MethodInfo VirtualMethod(MethodBase originalMethod, MethodInfo hookMethod)
        {
            var delegateMethodDeclaringType = hookMethod.DeclaringType;

            if (delegateMethodDeclaringType == null)
            {
                throw new ArgumentNullException(nameof(delegateMethodDeclaringType));
            }

            if (VirtualMethodDictionary.TryGetValue(hookMethod, out var virtualMethod))
            {
                return virtualMethod;
            }
            else
            {
                if (!NeedCastParameters(originalMethod, hookMethod, out var validParameters))
                {
                    return hookMethod;
                }

                var validParametersNotNull = validParameters.Where(x => x != null).ToArray();

                var name = $"DTFType_{delegateMethodDeclaringType.Name}_{hookMethod.Name}";
                var assemblyName = new AssemblyName(name);
                var assemblyBuilder =
                    AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
                var moduleBuilder = assemblyBuilder.DefineDynamicModule("module");

                var typeBuilder = moduleBuilder.DefineType(name,
                    TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed);

                var invokeMethod = typeBuilder.DefineMethod(
                    hookMethod.Name,
                    MethodAttributes.HideBySig | MethodAttributes.Static | MethodAttributes.Public,
                    hookMethod.ReturnType, validParametersNotNull.Select(x => x.ParameterType).ToArray());

                var ilGen = invokeMethod.GetILGenerator();

                for (var i = 0; i < validParametersNotNull.Length; i++)
                {
                    var validParameterNotNull = validParametersNotNull[i];

                    invokeMethod.DefineParameter(i + 1, validParameterNotNull.Attributes, validParameterNotNull.Name);
                }

                for (var i = 0; i < validParameters.Length; i++)
                {
                    var validParameter = validParameters[i];

                    if (validParameter != null)
                    {
                        ilGen.Emit(OpCodes.Ldarg, i);
                    }
                    else
                    {
                        ilGen.Emit(OpCodes.Ldnull);
                    }
                }

                ilGen.Emit(OpCodes.Call, hookMethod);

                ilGen.Emit(OpCodes.Ret);

                virtualMethod = typeBuilder.CreateType()
                    .GetMethod(hookMethod.Name, BindingFlags.Static | RefTool.Public);

                VirtualMethodDictionary.Add(hookMethod, virtualMethod);

                return virtualMethod;
            }
        }

        private static bool NeedCastParameters(MethodBase originalMethod, MethodInfo hookMethod,
            out ParameterInfo[] validParameters)
        {
            var originalDeclaringType = originalMethod.DeclaringType;

            if (originalDeclaringType == null)
            {
                throw new ArgumentNullException(nameof(originalDeclaringType));
            }

            var originalParameters = originalMethod.GetParameters();

            var delegateParameters = hookMethod.GetParameters();

            var hookDelegateParameters = hookMethod.GetParameters();

            var list = new List<ParameterInfo>();

            /*var patchNames = new[]
            {
                "__instance",
                "__originalMethod",
                "__args",
                "__result",
                "__state",
                "__exception",
                "__runOriginal"
            };*/

            for (var i = 0; i < delegateParameters.Length; i++)
            {
                var delegateParameter = delegateParameters[i];

                var hookDelegateParameter = hookDelegateParameters[i];

                var parameterName = hookDelegateParameter.Name;

                var isFieldName = parameterName.StartsWith("___", StringComparison.Ordinal);

                var isPatchName = parameterName.StartsWith("__", StringComparison.Ordinal);

                if ( /*patchNames.Contains(parameterName) ||*/
                    isPatchName && !isFieldName ||
                    isFieldName && !isPatchName &&
                    originalDeclaringType.GetField(parameterName.Remove(0, 3), AccessTools.all) != null ||
                    originalParameters.Any(x =>
                        x.Name == parameterName &&
                        (x.ParameterType.IsAssignableFrom(hookDelegateParameter.ParameterType) ||
                         x.ParameterType.IsSubclassOf(hookDelegateParameter.ParameterType))))
                {
                    list.Add(delegateParameter);
                }
                else
                {
                    list.Add(null);
                }
            }

            if (list.Contains(null))
            {
                validParameters = list.ToArray();

                return true;
            }
            else
            {
                validParameters = null;

                return false;
            }
        }

        /// <summary>
        ///     Remove a Hook Patch by <see cref="Delegate" />
        /// </summary>
        /// <param name="originalMethod"></param>
        /// <param name="hookDelegate"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public static void Remove(MethodBase originalMethod, Delegate hookDelegate)
        {
            Remove(originalMethod, hookDelegate.Method);
        }

        /// <summary>
        ///     Remove a Hook Patch by <see cref="MethodInfo" />
        /// </summary>
        /// <param name="originalMethod"></param>
        /// <param name="hookMethod"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public static void Remove(MethodBase originalMethod, MethodInfo hookMethod)
        {
            if (originalMethod == null)
            {
                throw new ArgumentNullException(nameof(originalMethod));
            }

            var originalDeclaringType = originalMethod.DeclaringType;

            if (originalDeclaringType == null)
            {
                throw new ArgumentNullException(nameof(originalDeclaringType));
            }

            if (hookMethod == null)
            {
                throw new ArgumentNullException(nameof(hookMethod));
            }

            var hookDeclaringType = hookMethod.DeclaringType;

            if (hookDeclaringType == null)
            {
                throw new ArgumentNullException(nameof(hookDeclaringType));
            }

            if (!hookMethod.IsStatic)
            {
                throw new Exception($"{hookDeclaringType.Name}_{hookMethod.Name} not is static");
            }

            if (HookDictionary.TryGetValue(originalMethod, out var typeDictionary) &&
                typeDictionary.TryGetValue(hookDeclaringType, out var harmony))
            {
                harmony.Unpatch(originalMethod, hookMethod);
            }
        }
    }
}