using BepInEx;
using EFTApi.Patches;

namespace EFTApi
{
    [BepInPlugin("com.kmyuhkyuk.EFTApi", "EFTApi", "1.1.8")]
    [BepInDependency("com.kmyuhkyuk.EFTReflection", "1.1.8")]
    public class EFTApiPlugin : BaseUnityPlugin
    {
        private void Start()
        {
            EFTVersion.WriteVersionLog();

            //Player
            new GamePlayerOwnerPatchs().Enable();

            //GameWorld
            new TriggerWithIdPatchs().Enable();
        }
    }
}