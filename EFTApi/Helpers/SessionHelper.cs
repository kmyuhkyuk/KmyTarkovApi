using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Threading.Tasks;
using EFT;
using EFTReflection;
using HarmonyLib;
using UnityEngine;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace EFTApi.Helpers
{
    public class SessionHelper
    {
        public static readonly SessionHelper Instance = new SessionHelper();

        public ISession Session { get; private set; }

        public object BackEndConfig { get; private set; }

        public readonly TradersData TradersHelper = TradersData.Instance;

        public readonly ExperienceData ExperienceHelper = ExperienceData.Instance;

        /// <summary>
        ///     Init Action
        /// </summary>
        public readonly RefHelper.HookRef CreateBackend;

        private SessionHelper()
        {
            CreateBackend = RefHelper.HookRef.Create(EFTVersion.AkiVersion > Version.Parse("3.3.0")
                    ? RefTool.GetEftType(x => x.Name == "TarkovApplication")
                    : RefTool.GetEftType(x => x.Name == "MainApplication"),
                x => x.IsAsync() && x.ReturnType == typeof(Task) &&
                     x.ContainsIL(OpCodes.Ldstr, "_backEnd.Session.GetGlobalConfig"));

            CreateBackend.Add(this, nameof(OnCreateBackend));
        }

        private static async void OnCreateBackend(object __instance, Task __result)
        {
            await __result;

            var session = EFTVersion.AkiVersion > Version.Parse("3.3.0")
                ? Traverse.Create(__instance).Field("ClientBackEnd").Property("Session").GetValue<ISession>()
                : Traverse.Create(__instance).Field("_backEnd").Property("Session").GetValue<ISession>();

            Instance.Session = session;

            var backEndConfig = Traverse.Create(session).Property("BackEndConfig").GetValue<object>();

            Instance.BackEndConfig = backEndConfig;

            Instance.TradersHelper.Init(session);

            Instance.ExperienceHelper.Init(backEndConfig);
        }

        public class TradersData
        {
            public static readonly TradersData Instance = new TradersData();

            public object[] Traders { get; private set; }

            public readonly AvatarData TradersAvatarData = AvatarData.Instance;

            private TradersData()
            {
            }

            internal void Init(ISession session)
            {
                var list = Traverse.Create(session).Property("Traders").GetValue<IList>();

                Traders = new object[list.Count];

                list.CopyTo(Traders, 0);

                TradersAvatarData.Init(Traders);
            }

            public class AvatarData
            {
                public static readonly AvatarData Instance = new AvatarData();

                private readonly Dictionary<string, object> _avatar = new Dictionary<string, object>();

                private readonly Dictionary<string, Task<Sprite>> _avatarSprites =
                    new Dictionary<string, Task<Sprite>>();

                private AvatarData()
                {
                }

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
            public static readonly ExperienceData Instance = new ExperienceData();

            private object _config;

            private object _experience;

            private object _kill;

            private int _victimLevelExp;

            private int _victimBotLevelExp;

            private float _headShotMult;

            private readonly Func<object, int, int> _refKillingBonusPercent;

            private bool _isReady;

            private ExperienceData()
            {
                _refKillingBonusPercent = RefHelper.ObjectMethodDelegate<Func<object, int, int>>(
                    RefTool.GetEftMethod(x => x.GetMethod("GetKillingBonusPercent") != null, RefTool.Public,
                        x => x.Name == "GetKillingBonusPercent"));
            }

            internal void Init(object backEndConfig)
            {
                _config = Traverse.Create(backEndConfig).Field("Config").GetValue<object>();

                _experience = Traverse.Create(_config).Field("Experience").GetValue<object>();

                _kill = Traverse.Create(_experience).Field("Kill").GetValue<object>();

                _victimLevelExp = Traverse.Create(_kill).Field("VictimLevelExp").GetValue<int>();

                _victimBotLevelExp = Traverse.Create(_kill).Field("VictimBotLevelExp").GetValue<int>();

                _headShotMult = Traverse.Create(_kill).Field("HeadShotMult").GetValue<float>();

                _isReady = true;
            }

            public int GetBaseExp(int exp, EPlayerSide side)
            {
                switch (side)
                {
                    case EPlayerSide.Usec:
                    case EPlayerSide.Bear:
                        return _victimLevelExp;
                    case EPlayerSide.Savage:
                        return exp < 0 ? _victimBotLevelExp : exp;
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
                return _isReady ? _refKillingBonusPercent(_kill, killed) : 0;
            }
        }
    }
}