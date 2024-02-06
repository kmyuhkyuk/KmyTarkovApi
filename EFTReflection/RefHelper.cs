using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using EFTReflection.Patching;
using HarmonyLib;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace EFTReflection
{
    public static class RefHelper
    {
        /// <summary>
        ///     Create Object Field GetValue Delegate, It usually used by <see cref="FieldRef{T, F}" />
        /// </summary>
        /// <typeparam name="T">Instance</typeparam>
        /// <typeparam name="TF">Return</typeparam>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Func<T, TF> ObjectFieldGetAccess<T, TF>(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
            {
                throw new ArgumentNullException(nameof(fieldInfo));
            }

            var delegateInstanceType = typeof(T);
            var delegateReturnType = typeof(TF);

            var declaringType = fieldInfo.DeclaringType;
            if (declaringType == null)
            {
                throw new ArgumentNullException(nameof(declaringType));
            }

            var returnNeedBox = delegateReturnType == typeof(object) && fieldInfo.FieldType.IsValueType;

            var dmd = new DynamicMethod($"__get_{declaringType.Name}_fi_{fieldInfo.Name}", delegateReturnType,
                new[] { delegateInstanceType }, true);

            var ilGen = dmd.GetILGenerator();

            if (!fieldInfo.IsStatic)
            {
                var declaringInIsObject = delegateInstanceType == typeof(object);
                var inIsValueType = declaringType.IsValueType;

                if (!inIsValueType)
                {
                    ilGen.Emit(OpCodes.Ldarg_0);
                }
                else
                {
                    ilGen.Emit(OpCodes.Ldarga_S, 0);
                }

                if (declaringInIsObject)
                {
                    ilGen.Emit(!inIsValueType ? OpCodes.Castclass : OpCodes.Unbox_Any, declaringType);
                }

                ilGen.Emit(OpCodes.Ldfld, fieldInfo);
            }
            else
            {
                ilGen.Emit(OpCodes.Ldsfld, fieldInfo);
            }

            if (returnNeedBox)
            {
                ilGen.Emit(OpCodes.Box, fieldInfo.FieldType);
            }

            ilGen.Emit(OpCodes.Ret);

            return (Func<T, TF>)dmd.CreateDelegate(typeof(Func<T, TF>));
        }

        /// <summary>
        ///     Create Object Field SetValue Delegate, It usually used by <see cref="FieldRef{T, F}" />
        /// </summary>
        /// <typeparam name="T">Instance</typeparam>
        /// <typeparam name="TF">Target</typeparam>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Action<T, TF> ObjectFieldSetAccess<T, TF>(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
            {
                throw new ArgumentNullException(nameof(fieldInfo));
            }

            var delegateInstanceType = typeof(T);
            var delegateParameterType = typeof(TF);

            var fieldType = fieldInfo.FieldType;

            var delegateParameterIsObject = delegateParameterType == typeof(object);
            var parameterIsValueType = fieldType.IsValueType;

            var declaringType = fieldInfo.DeclaringType;
            if (declaringType == null)
            {
                throw new ArgumentNullException(nameof(declaringType));
            }

            var dmd = new DynamicMethod($"__set_{declaringType.Name}_fi_{fieldInfo.Name}", null,
                new[] { delegateInstanceType, delegateParameterType }, true);

            var ilGen = dmd.GetILGenerator();

            if (!fieldInfo.IsStatic)
            {
                var delegateInIsObject = delegateInstanceType == typeof(object);
                var inIsValueType = declaringType.IsValueType;

                if (!inIsValueType)
                {
                    ilGen.Emit(OpCodes.Ldarg_0);
                }
                else
                {
                    ilGen.Emit(OpCodes.Ldarga_S, 0);
                }

                if (delegateInIsObject)
                {
                    ilGen.Emit(!inIsValueType ? OpCodes.Castclass : OpCodes.Unbox_Any, declaringType);
                }

                ilGen.Emit(OpCodes.Ldarg_1);

                if (delegateParameterIsObject)
                {
                    ilGen.Emit(parameterIsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, fieldType);
                }

                ilGen.Emit(OpCodes.Stfld, fieldInfo);
            }
            else
            {
                ilGen.Emit(OpCodes.Ldarg_1);

                if (delegateParameterIsObject)
                {
                    ilGen.Emit(parameterIsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, fieldType);
                }

                ilGen.Emit(OpCodes.Stsfld, fieldInfo);
            }

            ilGen.Emit(OpCodes.Ret);

            return (Action<T, TF>)dmd.CreateDelegate(typeof(Action<T, TF>));
        }

        /// <summary>
        ///     Create Object Method Delegate
        ///     <para>More convenient and fast Invoke Method</para>
        /// </summary>
        /// <remarks>Solve <see cref="AccessTools.MethodDelegate{DelegateType}" /> Cannot create delegate with object parameters</remarks>
        /// <typeparam name="TDelegateType"></typeparam>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static TDelegateType ObjectMethodDelegate<TDelegateType>(MethodInfo methodInfo)
            where TDelegateType : Delegate
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

            var returnType = methodInfo.ReturnType;

            var delegateType = typeof(TDelegateType);
            var delegateMethod = delegateType.GetMethod("Invoke");
            if (delegateMethod == null)
            {
                throw new ArgumentNullException(nameof(delegateMethod));
            }

            var delegateParameters = delegateMethod.GetParameters();
            var delegateParameterTypes = delegateParameters.Select(x => x.ParameterType).ToArray();

            var delegateInType = delegateParameterTypes[0];
            var delegateReturnType = delegateMethod.ReturnType;
            var returnNeedBox = delegateReturnType == typeof(object) && returnType.IsValueType;

            var dmd = new DynamicMethod($"OpenInstanceDelegate_{methodInfo.Name}", delegateReturnType,
                delegateParameterTypes, true);

            var ilGen = dmd.GetILGenerator();

            var isStatic = methodInfo.IsStatic;
            var num = !isStatic ? 1 : 0;

            Type[] parameterTypes;
            if (!isStatic)
            {
                var parameters = methodInfo.GetParameters();
                var numParameters = parameters.Length;
                parameterTypes = new Type[numParameters + 1];
                for (var i = 0; i < numParameters; i++)
                {
                    parameterTypes[i + 1] = parameters[i].ParameterType;
                }

                var delegateInIsObject = delegateInType == typeof(object);
                var inIsValueType = declaringType.IsValueType;

                if (!inIsValueType)
                {
                    ilGen.Emit(OpCodes.Ldarg_0);
                }
                else
                {
                    ilGen.Emit(OpCodes.Ldarga_S, 0);
                }

                if (delegateInIsObject)
                {
                    parameterTypes[0] = typeof(object);

                    ilGen.Emit(!inIsValueType ? OpCodes.Castclass : OpCodes.Unbox_Any, declaringType);
                }
                else
                {
                    parameterTypes[0] = delegateInType;
                }
            }
            else
            {
                parameterTypes = methodInfo.GetParameters().Select(x => x.ParameterType).ToArray();
            }

            for (var i = num; i < parameterTypes.Length; i++)
            {
                ilGen.Emit(OpCodes.Ldarg, i);

                var delegateParameterIsObject = delegateParameterTypes[i] == typeof(object);

                if (!delegateParameterIsObject)
                    continue;

                var parameterType = parameterTypes[i];

                var parameterIsValueType = parameterType.IsValueType;

                //DelegateParameterTypes i == parameterTypes i
                ilGen.Emit(!parameterIsValueType ? OpCodes.Castclass : OpCodes.Unbox_Any, parameterType);
            }

            ilGen.Emit(!isStatic ? OpCodes.Callvirt : OpCodes.Call, methodInfo);

            if (returnNeedBox)
            {
                ilGen.Emit(OpCodes.Box, returnType);
            }

            ilGen.Emit(OpCodes.Ret);

            return (TDelegateType)dmd.CreateDelegate(delegateType);
        }

        /// <summary>
        ///     Reflection Wrapper Base Class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TF"></typeparam>
        public abstract class RefBase<T, TF>
        {
            public abstract Type RefDeclaringType { get; }

            public abstract Type RefType { get; }

            public abstract TF GetValue(T instance);

            public abstract void SetValue(T instance, TF value);
        }

        /// <summary>
        ///     A Wrapper Property Delegate Class
        ///     <para>More convenient and fast Get or Set Property Value</para>
        ///     <para>If <typeparamref name="T" /> is object than use <see cref="ObjectMethodDelegate{DelegateType}" /></para>
        ///     <para>else use <see cref="AccessTools.MethodDelegate{DelegateType}" /> Generate Delegate</para>
        /// </summary>
        /// <typeparam name="T">Instance</typeparam>
        /// <typeparam name="TF">Return</typeparam>
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameterInConstructor")]
        public class PropertyRef<T, TF> : RefBase<T, TF>
        {
            private Func<T, TF> _refGetValue;

            private Action<T, TF> _refSetValue;

            private PropertyInfo _propertyInfo;

            private MethodInfo _getMethodInfo;

            private MethodInfo _setMethodInfo;

            private T _instance;

            public Type DeclaringType { get; private set; }

            public Type PropertyType => _propertyInfo.PropertyType;

            [Obsolete("Used DeclaringType", true)] public override Type RefDeclaringType => DeclaringType;

            [Obsolete("Used PropertyType", true)] public override Type RefType => PropertyType;

            public PropertyRef(PropertyInfo propertyInfo, object instance = null)
            {
                if (propertyInfo == null)
                {
                    throw new ArgumentNullException(nameof(propertyInfo));
                }

                Init(propertyInfo, instance);
            }

            public PropertyRef(Type type, string propertyName, bool declaredOnly = false, object instance = null)
            {
                var flags = declaredOnly ? AccessTools.allDeclared : AccessTools.all;

                var propertyInfo = type.GetProperty(propertyName, flags) ??
                                   throw new Exception($"{propertyName} Property not exist");

                Init(propertyInfo, instance);
            }

            private void Init(PropertyInfo propertyInfo, object instance)
            {
                _propertyInfo = propertyInfo;

                DeclaringType = propertyInfo.DeclaringType;

                if (instance != null)
                {
                    _instance = (T)instance;
                }

                var inIsObject = typeof(T) == typeof(object) || typeof(TF) == typeof(object);

                if (propertyInfo.CanRead)
                {
                    _getMethodInfo = propertyInfo.GetGetMethod(true);

                    _refGetValue = inIsObject
                        ? ObjectMethodDelegate<Func<T, TF>>(_getMethodInfo)
                        : AccessTools.MethodDelegate<Func<T, TF>>(_getMethodInfo);
                }

                if (!propertyInfo.CanWrite)
                    return;

                _setMethodInfo = propertyInfo.GetSetMethod(true);

                _refSetValue = inIsObject
                    ? ObjectMethodDelegate<Action<T, TF>>(_setMethodInfo)
                    : AccessTools.MethodDelegate<Action<T, TF>>(_setMethodInfo);
            }

            public static PropertyRef<T, TF> Create(PropertyInfo propertyInfo, object instance)
            {
                return new PropertyRef<T, TF>(propertyInfo, instance);
            }

            public static PropertyRef<T, TF> Create(string propertyName, bool declaredOnly = false,
                object instance = null)
            {
                return new PropertyRef<T, TF>(typeof(T), propertyName, declaredOnly, instance);
            }

            public static PropertyRef<T, TF> Create(Type type, string propertyName, bool declaredOnly = false,
                object instance = null)
            {
                return new PropertyRef<T, TF>(type, propertyName, declaredOnly, instance);
            }

            public override TF GetValue(T instance)
            {
                if (_refGetValue == null)
                {
                    throw new ArgumentNullException(nameof(_refGetValue));
                }

                if (instance != null && DeclaringType.IsInstanceOfType(instance))
                    return _refGetValue(instance);

                if (_instance != null && instance == null)
                    return _refGetValue(_instance);

                return default;
            }

            public override void SetValue(T instance, TF value)
            {
                if (_refSetValue == null)
                {
                    throw new ArgumentNullException(nameof(_refSetValue));
                }

                if (instance != null && DeclaringType.IsInstanceOfType(instance))
                {
                    _refSetValue(instance, value);
                }
                else if (_instance != null && instance == null)
                {
                    _refSetValue(_instance, value);
                }
            }
        }

        /// <summary>
        ///     A Wrapper Field Delegate Class
        ///     <para>More convenient and fast Get or Set Field Value</para>
        ///     <para>
        ///         If <typeparamref name="T" /> is object than use <see cref="ObjectFieldGetAccess{T,F}" /> and
        ///         <see cref="ObjectFieldSetAccess{T,F}" />
        ///     </para>
        ///     <para>else use <see cref="AccessTools.FieldRefAccess{T, F}(System.Reflection.FieldInfo)" /> Generate Delegate</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TF"></typeparam>
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameterInConstructor")]
        public class FieldRef<T, TF> : RefBase<T, TF>
        {
            private AccessTools.FieldRef<T, TF> _harmonyFieldRef;

            private Func<T, TF> _refGetValue;

            private Action<T, TF> _refSetValue;

            private FieldInfo _fieldInfo;

            private T _instance;

            private bool _useHarmony;

            public Type DeclaringType { get; private set; }

            public Type FieldType => _fieldInfo.FieldType;

            [Obsolete("Used DeclaringType", true)] public override Type RefDeclaringType => DeclaringType;

            [Obsolete("Used FieldType", true)] public override Type RefType => FieldType;

            public FieldRef(FieldInfo fieldInfo, object instance = null)
            {
                if (fieldInfo == null)
                {
                    throw new ArgumentNullException(nameof(fieldInfo));
                }

                Init(fieldInfo, instance);
            }

            public FieldRef(Type type, string fieldName, bool declaredOnly = false, object instance = null)
            {
                var flags = declaredOnly ? AccessTools.allDeclared : AccessTools.all;

                var fieldInfo = type.GetField(fieldName, flags) ?? throw new Exception($"{fieldName} Field not exist");

                Init(fieldInfo, instance);
            }

            public static FieldRef<T, TF> Create(FieldInfo fieldInfo, object instance = null)
            {
                return new FieldRef<T, TF>(fieldInfo, instance);
            }

            public static FieldRef<T, TF> Create(string fieldName, bool declaredOnly = false, object instance = null)
            {
                return new FieldRef<T, TF>(typeof(T), fieldName, declaredOnly, instance);
            }

            public static FieldRef<T, TF> Create(Type type, string fieldName, bool declaredOnly = false,
                object instance = null)
            {
                return new FieldRef<T, TF>(type, fieldName, declaredOnly, instance);
            }

            private void Init(FieldInfo fieldInfo, object instance)
            {
                _fieldInfo = fieldInfo;

                DeclaringType = fieldInfo.DeclaringType;

                if (instance != null)
                {
                    _instance = (T)instance;
                }

                if (typeof(TF) == typeof(object))
                {
                    _refGetValue = ObjectFieldGetAccess<T, TF>(fieldInfo);
                    _refSetValue = ObjectFieldSetAccess<T, TF>(fieldInfo);
                    _useHarmony = false;
                }
                else
                {
                    _harmonyFieldRef = AccessTools.FieldRefAccess<T, TF>(fieldInfo);
                    _useHarmony = true;
                }
            }

            public override TF GetValue(T instance)
            {
                if (_useHarmony)
                {
                    if (_harmonyFieldRef == null)
                    {
                        throw new ArgumentNullException(nameof(_harmonyFieldRef));
                    }

                    if (instance != null && DeclaringType.IsInstanceOfType(instance))
                        return _harmonyFieldRef(instance);

                    if (_instance != null && instance == null)
                        return _harmonyFieldRef(_instance);

                    return default;
                }

                if (_refGetValue == null)
                {
                    throw new ArgumentNullException(nameof(_refGetValue));
                }

                if (instance != null && DeclaringType.IsInstanceOfType(instance))
                    return _refGetValue(instance);

                if (_instance != null && instance == null)
                    return _refGetValue(_instance);

                return default;
            }

            public override void SetValue(T instance, TF value)
            {
                if (_useHarmony)
                {
                    if (_harmonyFieldRef == null)
                    {
                        throw new ArgumentNullException(nameof(_harmonyFieldRef));
                    }

                    if (instance != null && DeclaringType.IsInstanceOfType(instance))
                    {
                        _harmonyFieldRef(instance) = value;
                    }
                    else if (_instance != null && instance == null)
                    {
                        _harmonyFieldRef(_instance) = value;
                    }
                }
                else
                {
                    if (_refSetValue == null)
                    {
                        throw new ArgumentNullException(nameof(_refSetValue));
                    }

                    if (instance != null && DeclaringType.IsInstanceOfType(instance))
                    {
                        _refSetValue(instance, value);
                    }
                    else if (_instance != null && instance == null)
                    {
                        _refSetValue(_instance, value);
                    }
                }
            }
        }

        /// <summary>
        ///     A Wrapper HookPatch Class
        /// </summary>
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameterInConstructor")]
        public class HookRef
        {
            public readonly MethodBase TargetMethod;

            public HookRef(MethodBase targetMethod)
            {
                TargetMethod = targetMethod ?? throw new ArgumentNullException(nameof(targetMethod));
            }

            public HookRef(Type targetType, string targetMethodName)
            {
                TargetMethod = targetType.GetMethod(targetMethodName, AccessTools.allDeclared) ??
                               throw new Exception($"{targetType} can't find {targetMethodName} method");
            }

            public HookRef(Type targetType, Func<MethodInfo, bool> targetMethodPredicate)
            {
                TargetMethod = targetType.GetMethods(AccessTools.allDeclared).SingleOrDefault(targetMethodPredicate) ??
                               throw new Exception($"{targetType} can't find {targetMethodPredicate}");
            }

            public static HookRef Create(MethodBase targetMethod)
            {
                return new HookRef(targetMethod);
            }

            public static HookRef Create(Type targetType, string targetMethodName)
            {
                return new HookRef(targetType, targetMethodName);
            }

            public static HookRef Create(Type targetType, Func<MethodInfo, bool> targetMethodPredicate)
            {
                return new HookRef(targetType, targetMethodPredicate);
            }

            public void Add(object hookObject, string hookMethodName,
                HarmonyPatchType patchType = HarmonyPatchType.Postfix)
            {
                Add(hookObject.GetType().GetMethod(hookMethodName, AccessTools.allDeclared), patchType);
            }

            public void Add(Type hookType, string hookMethodName,
                HarmonyPatchType patchType = HarmonyPatchType.Postfix)
            {
                Add(hookType.GetMethod(hookMethodName, AccessTools.allDeclared), patchType);
            }

            public void Add(Delegate hookDelegate, HarmonyPatchType patchType = HarmonyPatchType.Postfix)
            {
                Add(hookDelegate.Method, patchType);
            }

            public void Add(MethodInfo hookMethod, HarmonyPatchType patchType = HarmonyPatchType.Postfix)
            {
                HookPatch.Add(TargetMethod, hookMethod, patchType);
            }

            public void Remove(Delegate hookDelegate)
            {
                Remove(hookDelegate.Method);
            }

            public void Remove(MethodBase hookMethod)
            {
                HookPatch.Remove(TargetMethod, (MethodInfo)hookMethod);
            }
        }
    }
}