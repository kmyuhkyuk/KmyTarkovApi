using BepInEx;
using EFTApi.Patches;

namespace EFTApi
{
    [BepInPlugin("com.kmyuhkyuk.EFTApi", "kmyuhkyuk-EFTApi", "1.0.0")]
    [BepInDependency("com.kmyuhkyuk.EFTReflection")]
    public class EFTApiPlugin : BaseUnityPlugin
    {
        private void Start()
        {
            new ApplicationPatch().Enable();

            //GameWorld
            new GameWorldAwakePatch().Enable();
            new GameWorldOnGameStartedPatch().Enable();
            new GameWorldDisposePatch().Enable();
            new LevelSettingsPatch().Enable();
            new LevelSettingsOnDestroyPatch().Enable();
            new TriggerWithIdPatch().Enable();

            //Player
            new PlayerPatch().Enable();
            new PlayerOnDeadPatch().Enable();
            new PlayerDisposePatch().Enable();
            new PlayerApplyDamageInfoPatch().Enable();
            new PlayerOnBeenKilledByAggressorPatch().Enable();
            new FirearmControllerInitiateShotPatch().Enable();
            new ArmorComponentApplyDurabilityDamagePatch().Enable();
            new ArmorComponentApplyDamagePatch().Enable();

            //Quest
            new OnConditionValueChangedPatch().Enable();

            //UI
            new GameUIPatch().Enable();
            new GameUIOnDestroyPatch().Enable();
            new MainMenuControllerPatch().Enable();
            new MainMenuControllerUnsubscribePatch().Enable();

            //Airdrop
            if (EFTVersion.Is350Up)
            {
                new AirdropBoxPatch().Enable();
            }
        }
    }
}