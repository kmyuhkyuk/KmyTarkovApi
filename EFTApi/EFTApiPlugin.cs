using BepInEx;
using EFTApi.Patches;

namespace EFTApi
{
    [BepInPlugin("com.kmyuhkyuk.EFTApi", "kmyuhkyuk-EFTApi", "1.1.0")]
    [BepInDependency("com.kmyuhkyuk.EFTReflection")]
    public class EFTApiPlugin : BaseUnityPlugin
    {
        private void Start()
        {
            //GameWorld
            new TriggerWithIdPatch().Enable();
        }
    }
}