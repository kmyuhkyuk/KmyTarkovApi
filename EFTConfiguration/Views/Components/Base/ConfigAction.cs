using System;

namespace EFTConfiguration.Views.Components.Base
{
    public abstract class ConfigAction : ConfigBase
    {
        protected Action ButtonAction;

        public virtual void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            Action buttonAction)
        {
            Init(modName, configNameKey, descriptionNameKey, isAdvanced);

            ButtonAction = buttonAction;
        }
    }
}