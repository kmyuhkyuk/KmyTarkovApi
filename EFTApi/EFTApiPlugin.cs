using BepInEx;
using EFTApi.Patches;

namespace EFTApi
{
    [BepInPlugin("com.kmyuhkyuk.EFTApi", "kmyuhkyuk-EFTApi", "1.1.4")]
    [BepInDependency("com.kmyuhkyuk.EFTReflection", "1.1.4")]
    public class EFTApiPlugin : BaseUnityPlugin
    {
        private void Start()
        {
            //GameWorld
            new TriggerWithIdPatchs().Enable();
        }
    }
}