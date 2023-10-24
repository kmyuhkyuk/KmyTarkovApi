using BepInEx;
using BepInEx.Logging;
using EFTApi.Patches;

namespace EFTApi
{
    [BepInPlugin("com.kmyuhkyuk.EFTApi", "kmyuhkyuk-EFTApi", "1.1.8")]
    [BepInDependency("com.kmyuhkyuk.EFTReflection", "1.1.8")]
    public class EFTApiPlugin : BaseUnityPlugin
    {
        private static readonly ManualLogSource EFTVersionLogSource =
            BepInEx.Logging.Logger.CreateLogSource("EFTVersion");

        private void Start()
        {
            EFTVersionLogSource.LogMessage($"GameVersion:{EFTVersion.GameVersion} AkiVersion:{EFTVersion.AkiVersion}");

            //GameWorld
            new TriggerWithIdPatchs().Enable();
        }
    }
}