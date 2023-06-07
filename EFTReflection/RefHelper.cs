using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

// ReSharper disable MemberCanBePrivate.Global

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
        /// <returns><see cref="Func{T, F}" /> from <paramref name="fieldInfo" /></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Func<T, TF> ObjectFieldGetAccess<T, TF>(FieldInfo fieldInfo) where T : class
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
                new[] { delegateInstanceType });

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
        /// <returns><see cref="Action{T, F}" /> from <paramref name="fieldInfo" /></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Action<T, TF> ObjectFieldSetAccess<T, TF>(FieldInfo fieldInfo) where T : class
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
                new[] { delegateInstanceType, delegateParameterType });

            var ilGen = dmd.GetILGenerator();

            if (!fieldInfo.IsStatic)
            {
                var delegateInIsObject = delegateInstanceType == typeof(object);
                var inIsValueType = declaringType == typeof(object);

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
        /// <param name="method"></param>
        /// <returns><see cref="Delegate" /> from <paramref name="method" /></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static TDelegateType ObjectMethodDelegate<TDelegateType>(MethodInfo method)
            where TDelegateType : Delegate
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            var declaringType = method.DeclaringType;
            if (declaringType == null)
            {
                throw new ArgumentNullException(nameof(declaringType));
            }

            var returnType = method.ReturnType;

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

            var dmd = new DynamicMethod($"OpenInstanceDelegate_{method.Name}", delegateReturnType,
                delegateParameterTypes);

            var ilGen = dmd.GetILGenerator();

            var isStatic = method.IsStatic;
            var num = !isStatic ? 1 : 0;

            Type[] parameterTypes;
            if (!isStatic)
            {
                var parameters = method.GetParameters();
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
                parameterTypes = method.GetParameters().Select(x => x.ParameterType).ToArray();
            }

            for (var i = num; i < parameterTypes.Length; i++)
            {
                ilGen.Emit(OpCodes.Ldarg, i);

                var delegateParameterIsObject = delegateParameterTypes[i] == typeof(object);

                if (delegateParameterIsObject)
                {
                    var parameterType = parameterTypes[i];

                    var parameterIsValueType = parameterType.IsValueType;

                    //DelegateParameterTypes i == parameterTypes i
                    ilGen.Emit(!parameterIsValueType ? OpCodes.Castclass : OpCodes.Unbox_Any, parameterType);
                }
            }

            ilGen.Emit(!isStatic ? OpCodes.Callvirt : OpCodes.Call, method);

            if (returnNeedBox)
            {
                ilGen.Emit(OpCodes.Box, returnType);
            }

            ilGen.Emit(OpCodes.Ret);

            return (TDelegateType)dmd.CreateDelegate(delegateType);
        }

        /// <summary>
        ///     A Wrapper Property Delegate Class
        ///     <para>More convenient and fast Get or Set Property Value</para>
        ///     <para>If <typeparamref name="T" /> is object then use <see cref="ObjectMethodDelegate{DelegateType}" /></para>
        ///     <para>else use <see cref="AccessTools.MethodDelegate{DelegateType}" /> Generate Delegate</para>
        /// </summary>
        /// <typeparam name="T">Instance</typeparam>
        /// <typeparam name="TF">Return</typeparam>
        public class PropertyRef<T, TF> where T : class
        {
            private Func<T, TF> _refGetValue;

            private Action<T, TF> _refSetValue;

            private PropertyInfo _propertyInfo;

            private MethodInfo _getMethodInfo;

            private MethodInfo _setMethodInfo;

            private T _instance;

            public Type DeclaringType { get; private set; }

            public Type PropertyType => _propertyInfo.PropertyType;

            public PropertyRef(PropertyInfo propertyInfo, object instance)
            {
                if (propertyInfo == null)
                {
                    throw new Exception("PropertyInfo is null");
                }

                Init(propertyInfo, instance);
            }

            public PropertyRef(Type type, string propertyName, bool declaredOnly, object instance)
            {
                var flags = declaredOnly ? AccessTools.allDeclared : AccessTools.all;

                var propertyInfo = type.GetProperty(propertyName, flags);

                if (propertyInfo == null)
                {
                    throw new Exception($"{propertyName} is null");
                }

                Init(propertyInfo, instance);
            }

            public PropertyRef(Type type, string[] propertyNames, bool declaredOnly, object instance)
            {
                var flags = declaredOnly ? AccessTools.allDeclared : AccessTools.all;

                var propertyInfo = propertyNames.Select(x => type.GetProperty(x, flags)).FirstOrDefault(x => x != null);

                if (propertyInfo == null)
                {
                    throw new Exception($"{propertyNames.First()} is null");
                }

                Init(propertyInfo, instance);
            }

            private void Init(PropertyInfo propertyInfo, object instance)
            {
                _propertyInfo = propertyInfo;

                DeclaringType = _propertyInfo.DeclaringType;

                _instance = (T)instance;

                var inIsObject = typeof(T) == typeof(object);

                if (_propertyInfo.CanRead)
                {
                    _getMethodInfo = _propertyInfo.GetGetMethod(true);

                    _refGetValue = inIsObject
                        ? ObjectMethodDelegate<Func<T, TF>>(_getMethodInfo)
                        : AccessTools.MethodDelegate<Func<T, TF>>(_getMethodInfo);
                }

                if (_propertyInfo.CanWrite)
                {
                    _setMethodInfo = _propertyInfo.GetSetMethod(true);

                    _refSetValue = inIsObject
                        ? ObjectMethodDelegate<Action<T, TF>>(_setMethodInfo)
                        : AccessTools.MethodDelegate<Action<T, TF>>(_setMethodInfo);
                }
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

            public static PropertyRef<T, TF> Create(string[] propertyNames, bool declaredOnly = false,
                object instance = null)
            {
                return new PropertyRef<T, TF>(typeof(T), propertyNames, declaredOnly, instance);
            }

            public static PropertyRef<T, TF> Create(Type type, string propertyName, bool declaredOnly = false,
                object instance = null)
            {
                return new PropertyRef<T, TF>(type, propertyName, declaredOnly, instance);
            }

            public static PropertyRef<T, TF> Create(Type type, string[] propertyNames, bool declaredOnly = false,
                object instance = null)
            {
                return new PropertyRef<T, TF>(type, propertyNames, declaredOnly, instance);
            }

            public TF GetValue(T instance)
            {
                if (_refGetValue == null)
                {
                    throw new ArgumentNullException(nameof(_refGetValue));
                }

                if (instance != null && DeclaringType.IsInstanceOfType(instance))
                {
                    return _refGetValue(instance);
                }
                else if (_instance != null && instance == null)
                {
                    return _refGetValue(_instance);
                }
                else
                {
                    return default;
                }
            }

            public void SetValue(T instance, TF value)
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
        ///         If <typeparamref name="T" /> is object then use <see cref="ObjectFieldGetAccess{T,F}" /> and
        ///         <see cref="RefHelper.ObjectFieldSetAccess{T,F}" />
        ///     </para>
        ///     <para>else use <see cref="AccessTools.FieldRefAccess{T, F}(System.Reflection.FieldInfo)" /> Generate Delegate</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TF"></typeparam>
        public class FieldRef<T, TF> where T : class
        {
            private AccessTools.FieldRef<T, TF> _harmonyFieldRef;

            private Func<T, TF> _refGetValue;

            private Action<T, TF> _refSetValue;

            private FieldInfo _fieldInfo;

            private T _instance;

            private bool _useHarmony;

            public Type DeclaringType { get; private set; }

            public Type FieldType => _fieldInfo.FieldType;

            public FieldRef(FieldInfo fieldInfo, object instance)
            {
                if (fieldInfo == null)
                {
                    throw new Exception("FieldInfo is null");
                }

                Init(fieldInfo, instance);
            }

            public FieldRef(Type type, string fieldName, bool declaredOnly, object instance)
            {
                var flags = declaredOnly ? AccessTools.allDeclared : AccessTools.all;

                var fieldInfo = type.GetField(fieldName, flags);

                if (fieldInfo == null)
                {
                    throw new Exception($"{fieldName} is null");
                }

                Init(fieldInfo, instance);
            }

            public FieldRef(Type type, string[] fieldNames, bool declaredOnly, object instance)
            {
                var flags = declaredOnly ? AccessTools.allDeclared : AccessTools.all;

                var fieldInfo = fieldNames.Select(x => type.GetField(x, flags)).FirstOrDefault(x => x != null);

                if (fieldInfo == null)
                {
                    throw new Exception($"{fieldNames.First()} is null");
                }

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

            public static FieldRef<T, TF> Create(string[] fieldNames, bool declaredOnly = false, object instance = null)
            {
                return new FieldRef<T, TF>(typeof(T), fieldNames, declaredOnly, instance);
            }

            public static FieldRef<T, TF> Create(Type type, string fieldName, bool declaredOnly = false,
                object instance = null)
            {
                return new FieldRef<T, TF>(type, fieldName, declaredOnly, instance);
            }

            public static FieldRef<T, TF> Create(Type type, string[] fieldNames, bool declaredOnly = false,
                object instance = null)
            {
                return new FieldRef<T, TF>(type, fieldNames, declaredOnly, instance);
            }

            private void Init(FieldInfo fieldInfo, object instance = null)
            {
                _fieldInfo = fieldInfo;

                DeclaringType = _fieldInfo.DeclaringType;

                _instance = (T)instance;

                if (typeof(TF) == typeof(object))
                {
                    _refGetValue = ObjectFieldGetAccess<T, TF>(_fieldInfo);
                    _refSetValue = ObjectFieldSetAccess<T, TF>(_fieldInfo);
                    _useHarmony = false;
                }
                else
                {
                    _harmonyFieldRef = AccessTools.FieldRefAccess<T, TF>(_fieldInfo);
                    _useHarmony = true;
                }
            }

            public TF GetValue(T instance)
            {
                if (_useHarmony)
                {
                    if (_harmonyFieldRef == null)
                    {
                        throw new ArgumentNullException(nameof(_harmonyFieldRef));
                    }

                    if (instance != null && DeclaringType.IsInstanceOfType(instance))
                    {
                        return _harmonyFieldRef(instance);
                    }
                    else if (_instance != null && instance == null)
                    {
                        return _harmonyFieldRef(_instance);
                    }
                    else
                    {
                        return default;
                    }
                }
                else
                {
                    if (_refGetValue == null)
                    {
                        throw new ArgumentNullException(nameof(_refGetValue));
                    }

                    if (instance != null && DeclaringType.IsInstanceOfType(instance))
                    {
                        return _refGetValue(instance);
                    }
                    else if (_instance != null && instance == null)
                    {
                        return _refGetValue(_instance);
                    }
                    else
                    {
                        return default;
                    }
                }
            }

            public void SetValue(T instance, TF value)
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
    }
}