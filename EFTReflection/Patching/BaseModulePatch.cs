using System;
using System.Collections.Generic;
using System.Reflection;
using Aki.Reflection.Patching;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;

namespace EFTReflection.Patching
{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Method)]
    public class PatchReverseAttribute : Attribute
    {
    }

    //Modified from Aki.Reflection.Patching.ModulePatch
    //Add Reverse Patch
    public abstract class BaseModulePatch
    {
        protected readonly Harmony Harmony;
        protected readonly List<HarmonyMethod> PrefixList;
        protected readonly List<HarmonyMethod> PostfixList;
        protected readonly List<HarmonyMethod> TranspilerList;
        protected readonly List<HarmonyMethod> FinalizerList;
        protected readonly List<HarmonyMethod> IlmanipulatorList;
        protected readonly List<HarmonyMethod> ReverseList;

        protected static ManualLogSource Logger { get; private set; }

        protected BaseModulePatch() : this(null)
        {
            if (Logger == null)
            {
                Logger = BepInEx.Logging.Logger.CreateLogSource(nameof(ModulePatch));
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="name"></param>
        protected BaseModulePatch(string name = null)
        {
            Harmony = new Harmony(name ?? GetType().Name);
            PrefixList = GetPatchMethods(typeof(PatchPrefixAttribute));
            PostfixList = GetPatchMethods(typeof(PatchPostfixAttribute));
            TranspilerList = GetPatchMethods(typeof(PatchTranspilerAttribute));
            FinalizerList = GetPatchMethods(typeof(PatchFinalizerAttribute));
            IlmanipulatorList = GetPatchMethods(typeof(PatchILManipulatorAttribute));
            ReverseList = GetPatchMethods(typeof(PatchReverseAttribute));

            if (PrefixList.Count == 0
                && PostfixList.Count == 0
                && TranspilerList.Count == 0
                && FinalizerList.Count == 0
                && IlmanipulatorList.Count == 0
                && ReverseList.Count == 0)
            {
                throw new Exception($"{Harmony.Id}: At least one of the patch methods must be specified");
            }
        }

        /// <summary>
        ///     Get original method
        /// </summary>
        /// <returns></returns>
        protected abstract MethodBase GetTargetMethod();

        /// <summary>
        ///     Get HarmonyMethod from string
        /// </summary>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        protected List<HarmonyMethod> GetPatchMethods(Type attributeType)
        {
            var T = GetType();
            var methods = new List<HarmonyMethod>();

            foreach (var method in T.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public |
                                                BindingFlags.DeclaredOnly))
            {
                if (method.GetCustomAttribute(attributeType) != null)
                {
                    methods.Add(new HarmonyMethod(method));
                }
            }

            return methods;
        }

        /// <summary>
        ///     Apply patch to target
        /// </summary>
        public virtual void Enable()
        {
            var target = GetTargetMethod();

            if (target == null)
            {
                throw new InvalidOperationException($"{Harmony.Id}: TargetMethod is null");
            }

            try
            {
                foreach (var prefix in PrefixList)
                {
                    Harmony.Patch(target, prefix: prefix);
                }

                foreach (var postfix in PostfixList)
                {
                    Harmony.Patch(target, postfix: postfix);
                }

                foreach (var transpiler in TranspilerList)
                {
                    Harmony.Patch(target, transpiler: transpiler);
                }

                foreach (var finalizer in FinalizerList)
                {
                    Harmony.Patch(target, finalizer: finalizer);
                }

                foreach (var ilmanipulator in IlmanipulatorList)
                {
                    Harmony.Patch(target, ilmanipulator: ilmanipulator);
                }

                foreach (var reverse in ReverseList)
                {
                    Harmony.CreateReversePatcher(target, reverse).Patch();
                }

                Logger.LogInfo($"Enabled patch {Harmony.Id}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"{Harmony.Id}: {ex}");
                throw new Exception($"{Harmony.Id}:", ex);
            }
        }

        /// <summary>
        ///     Remove applied patch from target
        /// </summary>
        public virtual void Disable()
        {
            try
            {
                Harmony.UnpatchSelf();
                Logger.LogInfo($"Disabled patch {Harmony.Id}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"{Harmony.Id}: {ex}");
                throw new Exception($"{Harmony.Id}:", ex);
            }
        }
    }
}