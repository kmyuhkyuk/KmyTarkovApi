using System.Collections.Generic;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace KmyTarkovUtils
{
    public interface IUpdate
    {
        void CustomUpdate();
    }

    public class UpdateManger
    {
        protected readonly List<IUpdate> Updates = new List<IUpdate>();

        protected readonly List<IUpdate> StopUpdates = new List<IUpdate>();

        protected readonly List<IUpdate> RemoveUpdates = new List<IUpdate>();

        public virtual void Register(IUpdate update)
        {
            if (!Updates.Contains(update))
            {
                Updates.Add(update);
            }
        }

        public virtual void Run(IUpdate update)
        {
            StopUpdates.Remove(update);
        }

        public virtual void Stop(IUpdate update)
        {
            if (!StopUpdates.Contains(update))
            {
                StopUpdates.Add(update);
            }
        }

        public virtual void Remove(IUpdate update)
        {
            if (!RemoveUpdates.Contains(update))
            {
                RemoveUpdates.Add(update);
            }
        }

        public virtual void Update()
        {
            if (Updates.Count == 0)
                return;

            for (var i = 0; i < Updates.Count; i++)
            {
                try
                {
                    var update = Updates[i];

                    if (RemoveUpdates.Contains(update))
                    {
                        var num = RemoveUpdates.IndexOf(update);

                        Updates.RemoveAt(i);

                        RemoveUpdates.RemoveAt(num);
                    }
                    else if (!StopUpdates.Contains(update))
                    {
                        update.CustomUpdate();
                    }
                }
                catch
                {
                    Updates.RemoveAt(i);

                    throw;
                }
            }
        }
    }
}