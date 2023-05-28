using System;
using EFT.UI;

namespace EFTApi.Helpers
{
    public class MainMenuControllerHelper
    {
        public MainMenuController MainMenuController { get; private set; }

        /// <summary>
        /// Init Action
        /// </summary>
        public event Action<MainMenuController, object, EnvironmentUI, MenuUI, CommonUI, PreloaderUI, object, object, Action, Action> Execute;

        public event Action<MainMenuController> Unsubscribe;

        internal void Trigger_Execute(MainMenuController mainMenuController, object backEnd, EnvironmentUI environmentUI, MenuUI menuUI, CommonUI commonUI, PreloaderUI preloaderUI, object raidSettings, object hideoutController, Action onLogoutPressed, Action reconnectAction)
        {
            MainMenuController = mainMenuController;

            Execute?.Invoke(mainMenuController, backEnd, environmentUI, menuUI, commonUI, preloaderUI, raidSettings, hideoutController, onLogoutPressed, reconnectAction);
        }

        internal void Trigger_Unsubscribe(MainMenuController mainMenuController)
        {
            Unsubscribe?.Invoke(mainMenuController);
        }
    }
}
