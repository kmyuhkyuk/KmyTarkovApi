using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EFTReflection.Patching
{
    public abstract class ModulePatchs : BaseModulePatch
    {
        private const string Tip =
            "Please do not override GetTargetMethod () in ModulePatchs, Used GetTargetMethods ()";

#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
        [Obsolete(Tip, true)]
        protected override MethodBase GetTargetMethod()
        {
            return null;
        }
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member

        /// <summary>
        ///     Get original methods
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<MethodBase> GetTargetMethods();

        /// <summary>
        ///     Apply patchs to target
        /// </summary>
        public override void Enable()
        {
            var old = GetTargetMethod();

            if (old != null)
            {
                Logger.LogError(Tip);
                base.Enable();
            }

            var targets = GetTargetMethods().ToArray();

            if (targets.Length == 0)
            {
                throw new InvalidOperationException($"{Harmony.Id}: TargetMethods is null");
            }

            try
            {
                foreach (var prefix in PrefixList)
                {
                    foreach (var target in targets)
                    {
                        Harmony.Patch(target, prefix: prefix);
                    }
                }

                foreach (var postfix in PostfixList)
                {
                    foreach (var target in targets)
                    {
                        Harmony.Patch(target, postfix: postfix);
                    }
                }

                foreach (var transpiler in TranspilerList)
                {
                    foreach (var target in targets)
                    {
                        Harmony.Patch(target, transpiler: transpiler);
                    }
                }

                foreach (var finalizer in FinalizerList)
                {
                    foreach (var target in targets)
                    {
                        Harmony.Patch(target, finalizer: finalizer);
                    }
                }

                foreach (var ilmanipulator in IlmanipulatorList)
                {
                    foreach (var target in targets)
                    {
                        Harmony.Patch(target, ilmanipulator: ilmanipulator);
                    }
                }

                foreach (var reverse in ReverseList)
                {
                    foreach (var target in targets)
                    {
                        Harmony.CreateReversePatcher(target, reverse).Patch();
                    }
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
        ///     Remove applied patchs from target
        /// </summary>
        public override void Disable()
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