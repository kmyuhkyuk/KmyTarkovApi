using System;
using EFT.UI;

namespace EFTApi.Helpers
{
    public class GameUIHelper
    {
        public GameUI GameUI { get; private set; }

        public event Action<GameUI> Awake;

        public event Action<GameUI> OnDestroy;

        internal void Trigger_Awake(GameUI gameUI)
        {
            GameUI = gameUI;

            Awake?.Invoke(gameUI);
        }

        internal void Trigger_Destroy(GameUI gameUI)
        {
            OnDestroy?.Invoke(gameUI);
        }
    }
}
