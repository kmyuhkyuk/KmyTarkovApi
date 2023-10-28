#if !UNITY_EDITOR

using BepInEx.Logging;

namespace EFTConfiguration.Models
{
    public class LogModel
    {
        public readonly object Sender;

        public readonly LogEventArgs EventArgs;

        public LogModel(object sender, LogEventArgs eventArgs)
        {
            Sender = sender;
            EventArgs = eventArgs;
        }
    }
}

#endif