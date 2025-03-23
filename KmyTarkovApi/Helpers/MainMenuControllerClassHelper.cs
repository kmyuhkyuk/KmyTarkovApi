using System;
using System.Threading.Tasks;
using KmyTarkovReflection;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable NotAccessedField.Global

namespace KmyTarkovApi.Helpers
{
    public class MainMenuControllerClassHelper
    {
        private static readonly Lazy<MainMenuControllerClassHelper> Lazy =
            new Lazy<MainMenuControllerClassHelper>(() => new MainMenuControllerClassHelper());

        public static MainMenuControllerClassHelper Instance => Lazy.Value;

        public MainMenuControllerClass MainMenuControllerClass { get; private set; }

        /// <summary>
        ///     Init Action
        /// </summary>
        public readonly RefHelper.HookRef Execute;

        /// <summary>
        ///     Unsubscribe Action
        /// </summary>
        public readonly RefHelper.HookRef Unsubscribe;

        private MainMenuControllerClassHelper()
        {
            var mainMenuControllerClassType = typeof(MainMenuControllerClass);

            Execute = RefHelper.HookRef.Create(mainMenuControllerClassType, "Execute");
            Unsubscribe = RefHelper.HookRef.Create(mainMenuControllerClassType, "Unsubscribe");
        }

        [EFTHelperHook]
        private void Hook()
        {
            Execute.Add(this, nameof(OnExecute));
        }

        private static async void OnExecute(Task<MainMenuControllerClass> __result)
        {
            Instance.MainMenuControllerClass = await __result;
        }
    }
}