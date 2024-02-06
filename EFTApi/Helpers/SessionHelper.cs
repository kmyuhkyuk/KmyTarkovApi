using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Threading.Tasks;
using EFT;
using EFTReflection;
using UnityEngine;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberHidesStaticFromOuterClass
// ReSharper disable UnusedMember.Global

namespace EFTApi.Helpers
{
    public class SessionHelper
    {
        private static readonly Lazy<SessionHelper> Lazy = new Lazy<SessionHelper>(() => new SessionHelper());

        public static SessionHelper Instance => Lazy.Value;

        public ISession Session { get; private set; }

        public object BackEndConfig { get; private set; }

        public TradersData TradersHelper => TradersData.Instance;

        public ExperienceData ExperienceHelper => ExperienceData.Instance;

        /// <summary>
        ///     Init Action
        /// </summary>
        public readonly RefHelper.HookRef CreateBackend;

        public readonly RefHelper.PropertyRef<object, object> RefBackEndConfig;

        public readonly Func<object, ISession> GetClientBackEndSession;

        private SessionHelper()
        {
            var applicationType = EFTVersion.AkiVersion > EFTVersion.Parse("3.3.0")
                ? RefTool.GetEftType(x => x.Name == "TarkovApplication")
                : RefTool.GetEftType(x => x.Name == "MainApplication");

            GetClientBackEndSession =
                RefHelper.ObjectMethodDelegate<Func<object, ISession>>(
                    applicationType.GetMethod("GetClientBackEndSession", RefTool.Public));

            RefBackEndConfig = RefHelper.PropertyRef<object, object>.Create(
                RefTool.GetEftType(x =>
                    x.IsAbstract && x.IsClass && x.GetProperty("BackEndConfig", RefTool.Public) != null),
                "BackEndConfig");

            CreateBackend = RefHelper.HookRef.Create(EFTVersion.AkiVersion > EFTVersion.Parse("3.3.0")
                    ? RefTool.GetEftType(x => x.Name == "TarkovApplication")
                    : RefTool.GetEftType(x => x.Name == "MainApplication"),
                x => x.IsAsync() && x.ReturnType == typeof(Task) &&
                     x.ContainsIL(OpCodes.Ldstr, "_backEnd.Session.GetGlobalConfig"));

            CreateBackend.Add(this, nameof(OnCreateBackend));
        }

        private static async void OnCreateBackend(object __instance, Task __result)
        {
            await __result;

            var session = Instance.GetClientBackEndSession(__instance);

            Instance.Session = session;

            var backEndConfig = Instance.RefBackEndConfig.GetValue(session);

            Instance.BackEndConfig = backEndConfig;
        }

        public class TradersData
        {
            private static readonly Lazy<TradersData> Lazy = new Lazy<TradersData>(() => new TradersData());

            public static TradersData Instance => Lazy.Value;

            public List<object> Traders
            {
                get
                {
                    var session = SessionHelper.Instance.Session;

                    if (session == null)
                        return null;

                    var list = new List<object>();

                    foreach (var key in RefTraders.GetValue(session))
                    {
                        list.Add(key);
                    }

                    return list;
                }
            }

            public readonly RefHelper.PropertyRef<object, IEnumerable> RefTraders;

            public TradersAvatarData TradersAvatarHelper => TradersAvatarData.Instance;

            private TradersData()
            {
                var sessionType = RefTool.GetEftType(x =>
                    typeof(ISession).IsAssignableFrom(x) && x.DeclaringType == null &&
                    x.GetProperty("Traders", RefTool.Public) != null);

                RefTraders = RefHelper.PropertyRef<object, IEnumerable>.Create(sessionType, "Traders");
            }

            public class TradersAvatarData
            {
                private static readonly Lazy<TradersAvatarData> Lazy =
                    new Lazy<TradersAvatarData>(() => new TradersAvatarData());

                public static TradersAvatarData Instance => Lazy.Value;

                public readonly RefHelper.PropertyRef<object, object> RefSettings;

                /// <summary>
                ///     Settings.Id
                /// </summary>
                public readonly RefHelper.FieldRef<object, string> RefId;

                private readonly Func<object, Task<Sprite>> _refGetAvatar;

                private TradersAvatarData()
                {
                    var traderType = RefTool.GetEftType(x => x.GetMethod("GetAssortmentPrice", RefTool.Public) != null);

                    RefSettings = RefHelper.PropertyRef<object, object>.Create(traderType, "Settings");
                    RefId = RefHelper.FieldRef<object, string>.Create(RefSettings.PropertyType, "Id");

                    _refGetAvatar =
                        RefHelper.ObjectMethodDelegate<Func<object, Task<Sprite>>>(
                            RefSettings.PropertyType.GetMethod("GetAvatar", RefTool.Public));
                }

                public Task<Sprite> GetAvatar(string traderId)
                {
                    var traders = TradersData.Instance.Traders;

                    if (traders == null)
                        return Task.FromResult<Sprite>(null);

                    foreach (var trader in traders)
                    {
                        var settings = RefSettings.GetValue(trader);

                        if (RefId.GetValue(settings) == traderId)
                            return _refGetAvatar(settings);
                    }

                    return Task.FromResult<Sprite>(null);
                }

                public async void BindAvatar(string traderId, Action<Sprite> action)
                {
                    var sprite = await GetAvatar(traderId);

                    action?.Invoke(sprite);
                }
            }
        }

        public class ExperienceData
        {
            private static readonly Lazy<ExperienceData> Lazy = new Lazy<ExperienceData>(() => new ExperienceData());

            public static ExperienceData Instance => Lazy.Value;

            public object Config => RefConfig.GetValue(SessionHelper.Instance.BackEndConfig);

            public object Experience => RefExperience.GetValue(Config);

            public object Kill => RefKill.GetValue(Experience);

            public int VictimLevelExp => RefVictimLevelExp.GetValue(Kill);

            public int VictimBotLevelExp => RefVictimBotLevelExp.GetValue(Kill);

            public float PmcHeadShotMult => RefPmcHeadShotMult.GetValue(Kill);

            public float BotHeadShotMult => RefBotHeadShotMult.GetValue(Kill);

            public readonly RefHelper.FieldRef<object, object> RefConfig;

            public readonly RefHelper.FieldRef<object, object> RefExperience;

            public readonly RefHelper.FieldRef<object, object> RefKill;

            public readonly RefHelper.FieldRef<object, int> RefVictimLevelExp;

            public readonly RefHelper.FieldRef<object, int> RefVictimBotLevelExp;

            public readonly RefHelper.FieldRef<object, float> RefPmcHeadShotMult;

            public readonly RefHelper.FieldRef<object, float> RefBotHeadShotMult;

            private readonly Func<object, int, int> _refKillingBonusPercent;

            private ExperienceData()
            {
                var backEndConfigType =
                    RefTool.GetEftType(x => x.GetField("BotWeaponScatterings", RefTool.Public) != null);

                RefConfig = RefHelper.FieldRef<object, object>.Create(backEndConfigType, "Config");
                RefExperience = RefHelper.FieldRef<object, object>.Create(RefConfig.FieldType, "Experience");
                RefKill = RefHelper.FieldRef<object, object>.Create(RefExperience.FieldType, "Kill");
                RefVictimLevelExp = RefHelper.FieldRef<object, int>.Create(RefKill.FieldType, "VictimLevelExp");
                RefVictimBotLevelExp = RefHelper.FieldRef<object, int>.Create(RefKill.FieldType, "VictimBotLevelExp");

                if (EFTVersion.AkiVersion > EFTVersion.Parse("3.6.1"))
                {
                    RefPmcHeadShotMult = RefHelper.FieldRef<object, float>.Create(RefKill.FieldType, "PmcHeadShotMult");
                    RefBotHeadShotMult = RefHelper.FieldRef<object, float>.Create(RefKill.FieldType, "BotHeadShotMult");
                }
                else
                {
                    var refHeadShotMult = RefHelper.FieldRef<object, float>.Create(RefKill.FieldType, "HeadShotMult");

                    RefPmcHeadShotMult = refHeadShotMult;
                    RefBotHeadShotMult = refHeadShotMult;
                }

                _refKillingBonusPercent = RefHelper.ObjectMethodDelegate<Func<object, int, int>>(
                    RefTool.GetEftMethod(x => x.GetMethod("GetKillingBonusPercent") != null, RefTool.Public,
                        x => x.Name == "GetKillingBonusPercent"));
            }

            public int GetBaseExp(int exp, EPlayerSide side)
            {
                switch (side)
                {
                    case EPlayerSide.Usec:
                    case EPlayerSide.Bear:
                        return VictimLevelExp;
                    case EPlayerSide.Savage:
                        return exp < 0 ? VictimBotLevelExp : exp;
                    default:
                        return 0;
                }
            }

            public int GetHeadExp(int exp, EPlayerSide side)
            {
                return (int)(GetBaseExp(exp, side) *
                             (side != EPlayerSide.Savage ? PmcHeadShotMult : BotHeadShotMult));
            }

            public int GetStreakExp(int exp, EPlayerSide side, int kills)
            {
                return (int)(GetBaseExp(exp, side) * (GetKillingBonusPercent(kills) / 100f));
            }

            public int GetKillingBonusPercent(int killed)
            {
                return Kill != null ? _refKillingBonusPercent(Kill, killed) : 0;
            }
        }
    }
}