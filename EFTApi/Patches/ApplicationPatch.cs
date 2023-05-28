using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Aki.Reflection.Patching;
using EFTReflection;
using EFTReflection.Patching;
using HarmonyLib;

namespace EFTApi.Patches
{
    public class ApplicationPatch : BaseModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            var flags = BindingFlags.DeclaredOnly | RefTool.NonPublic;

            var applicationType = EFTVersion.Is330Up
                ? RefTool.GetEftType(x => x.Name == "TarkovApplication")
                : RefTool.GetEftType(x => x.Name == "MainApplication");

            return RefTool.GetEftMethod(applicationType, flags,
                x => x.IsAsync() && x.ReturnType == typeof(Task) &&
                     x.ContainsIL(OpCodes.Ldstr, "_backEnd.Session.GetGlobalConfig"));
        }

        [PatchPostfix]
        private static async void PatchPostfix(object __instance, Task __result)
        {
            await __result;

            EFTHelpers._SessionHelper.Trigger_CreateBackend(EFTVersion.Is330Up
                ? Traverse.Create(__instance).Field("ClientBackEnd").Property("Session").GetValue<ISession>()
                : Traverse.Create(__instance).Field("_backEnd").Property("Session").GetValue<ISession>());
        }
    }
}