using BepInEx;
using EFTApi.Patches;

namespace EFTApi
{
    [BepInPlugin("com.kmyuhkyuk.EFTApi", "EFTApi", "1.2.0")]
    [BepInDependency("com.kmyuhkyuk.EFTReflection", "1.2.0")]
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