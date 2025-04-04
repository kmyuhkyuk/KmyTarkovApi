﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Bootstrap;
using EFT;
using HarmonyLib;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace KmyTarkovReflection
{
    public static class RefTool
    {
        public static readonly Type[] EftTypes = typeof(AbstractGame).Assembly.GetTypes();

        public static Assembly[] Assemblies => AppDomain.CurrentDomain.GetAssemblies();

        #region BindingFlags

        /// <summary>
        ///     <see cref="BindingFlags.NonPublic" /> and <see cref="BindingFlags.Instance" />
        /// </summary>
        public const BindingFlags NonPublic = BindingFlags.NonPublic | BindingFlags.Instance;

        /// <summary>
        ///     <see cref="BindingFlags.Public" /> and <see cref="BindingFlags.Instance" />
        /// </summary>
        public const BindingFlags Public = BindingFlags.Public | BindingFlags.Instance;

        /// <summary>
        ///     <see cref="BindingFlags.DeclaredOnly" /> and <see cref="BindingFlags.Static" />
        /// </summary>
        public const BindingFlags DeclaredStatic = BindingFlags.DeclaredOnly | BindingFlags.Static;

        #endregion

        #region GetAssembly

        public static Assembly GetAssembly(string name)
        {
            return GetAssembly(x => x.GetName().Name == name);
        }

        public static Assembly GetAssembly(Func<Assembly, bool> assemblyPredicate)
        {
            return Assemblies.Single(assemblyPredicate);
        }

        #endregion

        #region TryGetAssembly

        public static bool TryGetAssembly(string name, out Assembly assembly)
        {
            return TryGetAssembly(x => x.GetName().Name == name, out assembly);
        }

        public static bool TryGetAssembly(Func<Assembly, bool> assemblyPredicate, out Assembly assembly)
        {
            assembly = Assemblies.SingleOrDefault(assemblyPredicate);

            return assembly != null;
        }

        #endregion

        /// <summary>
        ///     Find Single <see cref="Type" /> by Lambda
        /// </summary>
        /// <param name="typePredicate"></param>
        /// <returns></returns>
        public static Type GetEftType(Func<Type, bool> typePredicate)
        {
            return EftTypes.Single(typePredicate);
        }

        /// <summary>
        ///     Try To Find Single <see cref="Type" /> by Lambda
        /// </summary>
        /// <param name="typePredicate"></param>
        /// <param name="eftType"></param>
        /// <returns></returns>
        public static bool TryGetEftType(Func<Type, bool> typePredicate, out Type eftType)
        {
            eftType = EftTypes.SingleOrDefault(typePredicate);

            return eftType != null;
        }

        #region GetMethod

        /// <summary>
        ///     Find Single <see cref="MethodInfo" /> by Lambda
        /// </summary>
        /// <param name="type"></param>
        /// <param name="flags"></param>
        /// <param name="methodPredicate"></param>
        /// <returns></returns>
        public static MethodInfo GetEftMethod(Type type, BindingFlags flags, Func<MethodInfo, bool> methodPredicate)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.GetMethods(flags).Single(methodPredicate);
        }

        /// <summary>
        ///     Find Single <see cref="MethodInfo" /> by Lambda
        /// </summary>
        /// <param name="typePredicate"></param>
        /// <param name="flags"></param>
        /// <param name="methodPredicate"></param>
        /// <returns></returns>
        public static MethodInfo GetEftMethod(Func<Type, bool> typePredicate, BindingFlags flags,
            Func<MethodInfo, bool> methodPredicate)
        {
            return GetEftMethod(GetEftType(typePredicate), flags, methodPredicate);
        }

        #endregion

        #region TryGetMethod

        /// <summary>
        ///     Try To Find Single <see cref="MethodInfo" /> by Lambda
        /// </summary>
        /// <param name="type"></param>
        /// <param name="flags"></param>
        /// <param name="methodPredicate"></param>
        /// <param name="eftMethod"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool TryGetEftMethod(Type type, BindingFlags flags, Func<MethodInfo, bool> methodPredicate,
            out MethodInfo eftMethod)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            eftMethod = type.GetMethods(flags).SingleOrDefault(methodPredicate);

            return eftMethod != null;
        }

        /// <summary>
        ///     Try To Find Single <see cref="MethodInfo" /> by Lambda
        /// </summary>
        /// <param name="typePredicate"></param>
        /// <param name="flags"></param>
        /// <param name="methodPredicate"></param>
        /// <param name="eftMethod"></param>
        /// <returns></returns>
        public static bool TryGetEftMethod(Func<Type, bool> typePredicate, BindingFlags flags,
            Func<MethodInfo, bool> methodPredicate, out MethodInfo eftMethod)
        {
            if (TryGetEftType(typePredicate, out var type))
                return TryGetEftMethod(type, flags, methodPredicate, out eftMethod);

            eftMethod = null;

            return false;
        }

        #endregion

        /// <summary>
        ///     If <see cref="MethodBase" /> is Async then return <see langword="true" />
        /// </summary>
        /// <param name="methodBase"></param>
        /// <returns></returns>
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
        ///     If <see cref="MemberInfo" /> is Compiler Generate then return <see langword="true" />
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsCompilerGenerated(this MemberInfo member)
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            return member.IsDefined(typeof(CompilerGeneratedAttribute));
        }

        /// <summary>
        ///     Get Async Struct from <see cref="MethodBase" />
        /// </summary>
        /// <param name="methodBase"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public static Type GetAsyncStruct(MethodBase methodBase)
        {
            if (methodBase == null)
            {
                throw new ArgumentNullException(nameof(methodBase));
            }

            var asyncAttribute = methodBase.GetCustomAttribute<AsyncStateMachineAttribute>() ??
                                 throw new Exception($"{methodBase.Name} is not Async Method");

            return asyncAttribute.StateMachineType;
        }

        #region GetAsyncMoveNext

        // ReSharper disable once InvalidXmlDocComment
        /// <summary>
        ///     Get Async MoveNext from <see cref="MethodBase" />, Harmony have been added this feature on Newer versions
        ///     <see cref="AccessTools.AsyncMoveNext" />
        /// </summary>
        /// <param name="methodBase"></param>
        /// <returns></returns>
        public static MethodInfo GetAsyncMoveNext(MethodBase methodBase)
        {
            var asyncStruct = GetAsyncStruct(methodBase);

            return GetAsyncMoveNext(asyncStruct);
        }

        /// <summary>
        ///     Get Async MoveNext from Struct
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static MethodInfo GetAsyncMoveNext(Type type)
        {
            if (!type.IsValueType)
            {
                throw new Exception($"{type.Name} is not Struct");
            }

            return type.GetMethod("MoveNext", AccessTools.allDeclared) ??
                   throw new Exception($"{type.Name} not exist MoveNext Method");
        }

        #endregion

        /// <summary>
        ///     If <see cref="MethodBase" /> is Async then return <see cref="GetAsyncMoveNext(MethodBase)" />
        /// </summary>
        /// <param name="methodBase"></param>
        /// <returns></returns>
        public static MethodInfo GetRealMethod(this MethodBase methodBase)
        {
            return methodBase.IsAsync() ? GetAsyncMoveNext(methodBase) : (MethodInfo)methodBase;
        }

        /// <summary>
        ///     Get Nested Methods from <see cref="MethodBase" />
        /// </summary>
        /// <param name="methodBase"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static MethodInfo[] GetNestedMethods(MethodBase methodBase)
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
                .Where(x => x.IsAssembly && x.Name.StartsWith($"<{methodBase.Name}>")).ToArray();
        }

        /// <summary>
        ///     <see cref="PatchProcessor.ReadMethodBody(MethodBase)" />
        /// </summary>
        /// <param name="methodBase"></param>
        /// <param name="realMethod"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<OpCode, object>> ReadMethodBody(this MethodBase methodBase,
            bool realMethod = true)
        {
            return PatchProcessor.ReadMethodBody(realMethod ? GetRealMethod(methodBase) : methodBase);
        }

        /// <summary>
        ///     If <see cref="IEnumerable{T}" /> Contains this IL then return <see langword="true" />
        /// </summary>
        /// <param name="methodBody"></param>
        /// <param name="opcode"></param>
        /// <param name="operand"></param>
        /// <returns></returns>
        public static bool ContainsIL(this IEnumerable<KeyValuePair<OpCode, object>> methodBody, OpCode opcode,
            object operand = null)
        {
            return methodBody.Contains(new KeyValuePair<OpCode, object>(opcode, operand));
        }

        /// <summary>
        ///     If <see cref="IEnumerable{T}" /> not other IL then return <see langword="true" />
        /// </summary>
        /// <param name="methodBody"></param>
        /// <returns></returns>
        public static bool IsEmptyIL(this IEnumerable<KeyValuePair<OpCode, object>> methodBody)
        {
            return methodBody.ElementAtOrDefault(0).Key == OpCodes.Ret;
        }

        /// <summary>
        ///     If <see cref="IEnumerable{T}" /> Contains sequence then return <see langword="true" />
        /// </summary>
        /// <param name="methodBody"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public static bool ContainsSequenceIL(this IEnumerable<KeyValuePair<OpCode, object>> methodBody,
            IEnumerable<KeyValuePair<OpCode, object>> sequence)
        {
            var methodBodyArray = methodBody as KeyValuePair<OpCode, object>[] ?? methodBody.ToArray();
            var sequenceArray = sequence as KeyValuePair<OpCode, object>[] ?? sequence.ToArray();

            for (var i = 0; i < methodBodyArray.Length - sequenceArray.Length; i++)
            {
                if (methodBodyArray.Skip(i).Take(sequenceArray.Length).SequenceEqual(sequenceArray))
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     Find all <see cref="OpCodes.Call " /> or <see cref="OpCodes.Callvirt" /> Methods from <see cref="IEnumerable{T}" />
        /// </summary>
        /// <param name="methodBody"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static MethodInfo[] GetCallMethods(this IEnumerable<KeyValuePair<OpCode, object>> methodBody)
        {
            if (methodBody == null)
            {
                throw new ArgumentNullException(nameof(methodBody));
            }

            return methodBody.Where(x => x.Key == OpCodes.Call || x.Key == OpCodes.Callvirt)
                .Select(x => (MethodInfo)x.Value).ToArray();
        }

        /// <summary>
        ///     Try Find BepInEx Plugin <see cref="Type" />
        /// </summary>
        /// <param name="pluginGUID"></param>
        /// <param name="plugin"></param>
        /// <returns></returns>
        public static bool TryGetPlugin(string pluginGUID, out BaseUnityPlugin plugin)
        {
            var hasPluginInfo = Chainloader.PluginInfos.TryGetValue(pluginGUID, out var pluginInfo);

            plugin = hasPluginInfo ? pluginInfo.Instance : null;

            return hasPluginInfo;
        }

        #region GetPluginType

        /// <summary>
        ///     Find <see cref="Type" /> from Plugin Assembly by Path
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="typePath"></param>
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
        ///     Find <see cref="Type" /> from Plugin Assembly by Lambda
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="typePredicate"></param>
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

        #endregion

        #region TryGetPluginType

        /// <summary>
        ///     Try Find <see cref="Type" /> from Plugin Assembly by Path
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="typePath"></param>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool TryGetPluginType(BaseUnityPlugin plugin, string typePath, out Type pluginType)
        {
            if (plugin == null)
            {
                throw new ArgumentNullException(nameof(plugin));
            }

            pluginType = plugin.GetType().Assembly.GetType(typePath);

            return pluginType != null;
        }

        /// <summary>
        ///     Try Find <see cref="Type" /> from Plugin Assembly by Lambda
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="typePredicate"></param>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool TryGetPluginType(BaseUnityPlugin plugin, Func<Type, bool> typePredicate, out Type pluginType)
        {
            if (plugin == null)
            {
                throw new ArgumentNullException(nameof(plugin));
            }

            pluginType = plugin.GetType().Assembly.GetTypes().SingleOrDefault(typePredicate);

            return pluginType != null;
        }

        #endregion
    }
}