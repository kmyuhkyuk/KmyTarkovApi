using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Aki.Reflection.Utils;
using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;

// ReSharper disable MemberCanBePrivate.Global

namespace EFTReflection
{
    public static class RefTool
    {
        /// <summary>
        ///     <see cref="BindingFlags.NonPublic" /> and <see cref="BindingFlags.Instance" />
        /// </summary>
        public static readonly BindingFlags NonPublic = BindingFlags.NonPublic | BindingFlags.Instance;

        /// <summary>
        ///     <see cref="BindingFlags.Public" /> and <see cref="BindingFlags.Instance" />
        /// </summary>
        public static readonly BindingFlags Public = BindingFlags.Public | BindingFlags.Instance;

        /// <summary>
        ///     <see cref="BindingFlags.DeclaredOnly" /> and <see cref="BindingFlags.Static" />
        /// </summary>
        public static readonly BindingFlags DeclaredStatic = BindingFlags.DeclaredOnly | BindingFlags.Static;

        /// <summary>
        ///     Find Single Eft Type by Lambda
        /// </summary>
        /// <param name="typePredicate">Type Lambda</param>
        /// <returns>
        ///     <see cref="Type" />
        /// </returns>
        public static Type GetEftType(Func<Type, bool> typePredicate)
        {
            return PatchConstants.EftTypes.Single(typePredicate);
        }

        /// <summary>
        ///     Find Single Eft Method by Lambda
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="flags">MethodInfo BindingFlags</param>
        /// <param name="methodPredicate">MethodInfo Lambda</param>
        /// <returns>
        ///     <see cref="MethodInfo" />
        /// </returns>
        public static MethodInfo GetEftMethod(Type type, BindingFlags flags, Func<MethodInfo, bool> methodPredicate)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.GetMethods(flags).Single(methodPredicate);
        }

        /// <summary>
        ///     Find Single Eft Method by Lambda
        /// </summary>
        /// <param name="typePredicate">Type Lambda</param>
        /// <param name="flags">MethodInfo BindingFlags</param>
        /// <param name="methodPredicate">MethodInfo Lambda</param>
        /// <returns>
        ///     <see cref="MethodInfo" />
        /// </returns>
        public static MethodInfo GetEftMethod(Func<Type, bool> typePredicate, BindingFlags flags,
            Func<MethodInfo, bool> methodPredicate)
        {
            return GetEftMethod(GetEftType(typePredicate), flags, methodPredicate);
        }

        /// <summary>
        ///     If Method is Async then return <see langword="true" />
        /// </summary>
        /// <param name="methodBase">Method</param>
        /// <returns>
        ///     <see cref="bool" />
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsAsync(this MethodBase methodBase)
        {
            if (methodBase == null)
            {
                throw new ArgumentNullException(nameof(methodBase));
            }

            return methodBase.IsDefined(typeof(AsyncStateMachineAttribute));
        }

        /// <summary>
        ///     If Type is Compiler Generate then return <see langword="true" />
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>
        ///     <see cref="bool" />
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsCompilerGenerated(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.IsDefined(typeof(CompilerGeneratedAttribute));
        }

        /// <summary>
        ///     If Method is Compiler Generate then return <see langword="true" />
        /// </summary>
        /// <param name="methodBase">Method</param>
        /// <returns>
        ///     <see cref="bool" />
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsCompilerGenerated(this MethodBase methodBase)
        {
            if (methodBase == null)
            {
                throw new ArgumentNullException(nameof(methodBase));
            }

            return methodBase.IsDefined(typeof(CompilerGeneratedAttribute));
        }

        /// <summary>
        ///     Get Async Struct from MethodInfo
        /// </summary>
        /// <param name="methodBase">Async Method</param>
        /// <returns>
        ///     <see cref="Type" />
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public static Type GetAsyncStruct(MethodBase methodBase)
        {
            if (methodBase == null)
            {
                throw new ArgumentNullException(nameof(methodBase));
            }

            var asyncAttribute = methodBase.GetCustomAttribute<AsyncStateMachineAttribute>();

            if (asyncAttribute == null)
            {
                throw new Exception($"{methodBase.Name} not is async type");
            }

            return asyncAttribute.StateMachineType;
        }

        // ReSharper disable once InvalidXmlDocComment
        /// <summary>
        ///     Get Async MoveNext from MethodInfo, Harmony have been added this feature on Newer versions
        ///     <see cref="AccessTools.AsyncMoveNext" />
        /// </summary>
        /// <param name="methodBase">Async Method</param>
        /// <returns>
        ///     <see cref="MethodInfo" />
        /// </returns>
        public static MethodInfo GetAsyncMoveNext(MethodBase methodBase)
        {
            var asyncStruct = GetAsyncStruct(methodBase);

            return asyncStruct.GetMethod("MoveNext", BindingFlags.DeclaredOnly | NonPublic);
        }

        /// <summary>
        ///     Get Async MoveNext from Struct
        /// </summary>
        /// <param name="type">Async Struct</param>
        /// <returns>
        ///     <see cref="MethodInfo" />
        /// </returns>
        /// <exception cref="Exception"></exception>
        public static MethodInfo GetAsyncMoveNext(Type type)
        {
            if (!type.IsValueType)
            {
                throw new Exception($"{type.Name} not is Struct");
            }

            var method = type.GetMethod("MoveNext", NonPublic);

            if (method == null)
            {
                throw new Exception($"{type.Name} not have MoveNext Method");
            }

            return method;
        }

        /// <summary>
        ///     Get Nested Methods from MethodInfo
        /// </summary>
        /// <param name="methodBase">Method</param>
        /// <returns>
        ///     <see cref="IEnumerable{MethodInfo}" />
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<MethodInfo> GetNestedMethods(MethodBase methodBase)
        {
            if (methodBase == null)
            {
                throw new ArgumentNullException(nameof(methodBase));
            }

            var declaringType = methodBase.DeclaringType;
            if (declaringType == null)
            {
                throw new ArgumentNullException(nameof(declaringType));
            }

            return declaringType.GetMethods(DeclaredStatic | NonPublic)
                .Where(x => x.IsAssembly && x.Name.StartsWith($"<{methodBase.Name}>"));
        }

        /// <summary>
        ///     If Method Contains this IL then return <see langword="true" />
        /// </summary>
        /// <param name="methodBase">MethodInfo</param>
        /// <param name="opcode">OpCode</param>
        /// <param name="operand">Operand</param>
        /// <returns>
        ///     <see cref="bool" />
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool ContainsIL(this MethodBase methodBase, OpCode opcode, object operand = null)
        {
            if (methodBase == null)
            {
                throw new ArgumentNullException(nameof(methodBase));
            }

            var realMethod = methodBase.IsAsync() ? GetAsyncMoveNext(methodBase) : methodBase;

            return PatchProcessor.ReadMethodBody(realMethod).Any(il => il.Key == opcode && il.Value == operand);
        }

        /// <summary>
        ///     Try Find BepInEx Plugin Type
        /// </summary>
        /// <param name="pluginGUID">Plugin GUID</param>
        /// <param name="plugin">Plugin</param>
        /// <returns></returns>
        public static bool TryGetPlugin(string pluginGUID, out BaseUnityPlugin plugin)
        {
            var hasPluginInfo = Chainloader.PluginInfos.TryGetValue(pluginGUID, out var pluginInfo);

            plugin = hasPluginInfo ? pluginInfo.Instance : null;

            return hasPluginInfo;
        }

        /// <summary>
        ///     Find Type from Plugin by Path
        /// </summary>
        /// <param name="plugin">Plugin</param>
        /// <param name="typePath">Path</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Type GetPluginType(BaseUnityPlugin plugin, string typePath)
        {
            if (plugin == null)
            {
                throw new ArgumentNullException(nameof(plugin));
            }

            return plugin.GetType().Assembly.GetType(typePath, true);
        }

        /// <summary>
        ///     Find Plugin Type by Lambda
        /// </summary>
        /// <param name="plugin">Plugin</param>
        /// <param name="typePredicate">Type Lambda</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Type GetPluginType(BaseUnityPlugin plugin, Func<Type, bool> typePredicate)
        {
            if (plugin == null)
            {
                throw new ArgumentNullException(nameof(plugin));
            }

            return plugin.GetType().Assembly.GetTypes().Single(typePredicate);
        }
    }
}