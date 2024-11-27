using BepInEx;
using EFTApi.Patches;

namespace EFTApi
{
    [BepInPlugin("com.kmyuhkyuk.EFTApi", "EFTApi", "1.3.0")]
    [BepInDependency("com.kmyuhkyuk.EFTReflection", "1.3.0")]
    public class EFTApiPlugin : BaseUnityPlugin
    {
        private void Start()
        {
            EFTVersion.WriteVersionLog();

            //Init EFTHelpers Hooks
            EFTHelpers.InitHooks();

            //Player
            new GamePlayerOwnerPatchs().Enable();

            //GameWorld
            new TriggerWithIdPatchs().Enable();
        }
    }
}