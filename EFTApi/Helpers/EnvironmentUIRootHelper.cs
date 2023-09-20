using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFT.UI;
using EFTReflection;

namespace EFTApi.Helpers
{
    public class EnvironmentUIRootHelper
    {
        public static readonly EnvironmentUIRootHelper Instance = new EnvironmentUIRootHelper();

        public EnvironmentUIRoot EnvironmentUIRoot { get; private set; }

        public readonly RefHelper.HookRef Init;
        
        private EnvironmentUIRootHelper()
        {
            Init = new RefHelper.HookRef(typeof(EnvironmentUIRoot), "Init");

            Init.Add(this, nameof(OnInit));
        }

        private static void OnInit(EnvironmentUIRoot __instance)
        {
            Instance.EnvironmentUIRoot = __instance;
        }
    }
}
