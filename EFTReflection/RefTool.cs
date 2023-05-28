using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Aki.Reflection.Utils;
using HarmonyLib;

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
        ///     Return Method is Async
        /// </summary>
        /// <param name="methodInfo">Method</param>
        /// <returns>
        ///     <see cref="bool" />
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsAsync(this MethodInfo methodInfo)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            return methodInfo.IsDefined(typeof(AsyncStateMachineAttribute));
        }

        /// <summary>
        ///     Get Async Struct from MethodInfo
        /// </summary>
        /// <param name="methodInfo">Async Method</param>
        /// <returns>
        ///     <see cref="Type" />
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public static Type GetAsyncStruct(MethodInfo methodInfo)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            var asyncAttribute = methodInfo.GetCustomAttribute<AsyncStateMachineAttribute>();

            if (asyncAttribute == null)
            {
                throw new Exception(methodInfo.Name + " not is async type");
            }

            return asyncAttribute.StateMachineType;
        }

        // ReSharper disable once InvalidXmlDocComment
        /// <summary>
        ///     Get Async MoveNext from MethodInfo, Harmony have been added this feature on Newer versions
        ///     <see cref="AccessTools.AsyncMoveNext" />
        /// </summary>
        /// <param name="methodInfo">Async Method</param>
        /// <returns>
        ///     <see cref="MethodInfo" />
        /// </returns>
        public static MethodInfo GetAsyncMoveNext(MethodInfo methodInfo)
        {
            var asyncStruct = GetAsyncStruct(methodInfo);

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
                throw new Exception(type.Name + " not is Struct");
            }

            var method = type.GetMethod("MoveNext", NonPublic);

            if (method == null)
            {
                throw new Exception(type.Name + " not have MoveNext Method");
            }

            return method;
        }

        /// <summary>
        ///     Get Nested Methods from MethodInfo
        /// </summary>
        /// <param name="methodInfo">Method</param>
        /// <returns>
        ///     <see cref="IEnumerable{MethodInfo}" />
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<MethodInfo> GetNestedMethods(MethodInfo methodInfo)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            var declaringType = methodInfo.DeclaringType;
            if (declaringType == null)
            {
                throw new ArgumentNullException(nameof(declaringType));
            }

            return declaringType.GetMethods(DeclaredStatic | NonPublic)
                .Where(x => x.IsAssembly && x.Name.StartsWith($"<{methodInfo.Name}>"));
        }

        /// <summary>
        ///     Return MethodInfo have this IL
        /// </summary>
        /// <param name="methodInfo">MethodInfo</param>
        /// <param name="opcode">OpCode</param>
        /// <param name="operand">Operand</param>
        /// <returns>
        ///     <see cref="bool" />
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool ContainsIL(this MethodInfo methodInfo, OpCode opcode, object operand = null)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            var realMethod = methodInfo.IsAsync() ? GetAsyncMoveNext(methodInfo) : methodInfo;

            foreach (var il in PatchProcessor.ReadMethodBody(realMethod))
            {
                if (il.Key == opcode && il.Value == operand)
                    return true;
            }

            return false;
        }
    }
}