using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using EFT;
using EFTReflection;
using HarmonyLib;
using UnityEngine;

namespace EFTApi.Helpers
{
    public class SessionHelper
    {
        public ISession Session { get; private set; }

        public object BackEndConfig { get; private set; }

        /// <summary>
        /// Traders Helper
        /// </summary>
        public readonly TradersData TradersHelper = new TradersData();

        /// <summary>
        /// Experience Helper
        /// </summary>
        public readonly ExperienceData ExperienceHelper = new ExperienceData();

        public event Action<ISession> CreateBackend;

        internal void Trigger_CreateBackend(ISession session)
        {
            Session = session;

            BackEndConfig = Traverse.Create(session).Property("BackEndConfig").GetValue<object>();

            TradersHelper.Init(session);

            CreateBackend?.Invoke(session);

            ExperienceHelper.Init(BackEndConfig);
        }

        public class TradersData
        {
            public object[] Traders { get; private set; } = Array.Empty<object>();

            public readonly AvatarData TradersAvatarData = new AvatarData();

            internal void Init(ISession session)
            {
                var list = Traverse.Create(session).Property("Traders").GetValue<IList>();

                Traders = new object[list.Count];

                list.CopyTo(Traders, 0);

                TradersAvatarData.Init(Traders);
            }

            public class AvatarData
            {
                private readonly Dictionary<string, object> _avatar = new Dictionary<string, object>();

                private readonly Dictionary<string, Task<Sprite>> _avatarSprites = new Dictionary<string, Task<Sprite>>();

                internal void Init(object[] traders)
                {
                    Clear();

                    foreach (var trader in traders)
                    {
                        var settings = Traverse.Create(trader).Property("Settings").GetValue<object>();

                        var id = Traverse.Create(settings).Field("Id").GetValue<string>();

                        _avatar.Add(id, settings);
                    }
                }

                public async Task<Sprite> GetAvatar(string traderId)
                {
                    if (_avatarSprites.TryGetValue(traderId, out var sprite))
                    {
                        return await sprite;
                    }
                    else
                    {
                        if (_avatar.TryGetValue(traderId, out var avatar))
                        {
                            var avatarSprite = Traverse.Create(avatar).Method("GetAvatar").GetValue<Task<Sprite>>();
                            _avatarSprites.Add(traderId, avatarSprite);

                            return await avatarSprite;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }

                public async void GetAvatar(string traderId, Action<Sprite> action)
                {
                    var sprite = await GetAvatar(traderId);

                    action?.Invoke(sprite);
                }

                private void Clear()
                {
                    _avatar.Clear();
                    _avatarSprites.Clear();
                }
            }
        }

        public class ExperienceData
        {
            private object _config;

            private object _experience;

            private object _kill;

            private int _victimLevelExp;

            private int _victimBotLevelExp;

            private float _headShotMult;

            private readonly Func<object, int, int> _refKillingBonusPercent;

            public ExperienceData()
            {
                _refKillingBonusPercent = RefHelper.ObjectMethodDelegate<Func<object, int, int>>(RefTool.GetEftMethod(x => x.GetMethod("GetKillingBonusPercent") != null, RefTool.Public, x => x.Name == "GetKillingBonusPercent"));
            }

            public void Init(object backEndConfig)
            {
                _config = Traverse.Create(backEndConfig).Field("Config").GetValue<object>();

                _experience = Traverse.Create(_config).Field("Experience").GetValue<object>();

                _kill = Traverse.Create(_experience).Field("Kill").GetValue<object>();

                _victimLevelExp = Traverse.Create(_kill).Field("VictimLevelExp").GetValue<int>();

                _victimBotLevelExp = Traverse.Create(_kill).Field("VictimBotLevelExp").GetValue<int>();

                _headShotMult = Traverse.Create(_kill).Field("HeadShotMult").GetValue<float>();
            }

            public int GetBaseExp(int exp, EPlayerSide side)
            {
                switch (side)
                {
                    case EPlayerSide.Usec:
                    case EPlayerSide.Bear:
                        return _victimLevelExp;
                    case EPlayerSide.Savage:
                        if (exp < 0)
                            return _victimBotLevelExp;
                        else
                            return exp;
                    default:
                        return 0;
                }
            }

            public int GetHeadExp(int exp, EPlayerSide side)
            {
                return (int)(GetBaseExp(exp, side) * _headShotMult);
            }

            public int GetStreakExp(int exp, EPlayerSide side, int kills)
            {
                return (int)(GetBaseExp(exp, side) * (GetKillingBonusPercent(kills) / 100f));
            }

            public int GetKillingBonusPercent(int killed)
            {
                return _refKillingBonusPercent(_kill, killed);
            }
        }
    }
}
