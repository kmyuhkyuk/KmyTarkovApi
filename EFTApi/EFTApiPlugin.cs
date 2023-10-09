using BepInEx;
using EFTApi.Patches;

namespace EFTApi
{
    [BepInPlugin("com.kmyuhkyuk.EFTApi", "kmyuhkyuk-EFTApi", "1.1.7")]
    [BepInDependency("com.kmyuhkyuk.EFTReflection", "1.1.7")]
    public class EFTApiPlugin : BaseUnityPlugin
    {
        private void Start()
        {
            //GameWorld
            new TriggerWithIdPatchs().Enable();
        }
    }
}