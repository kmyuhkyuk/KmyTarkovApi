using System;
using System.Reflection.Emit;
using System.Threading.Tasks;
using EFT;
using HarmonyLib;
using KmyTarkovReflection;
using UnityEngine;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberHidesStaticFromOuterClass
// ReSharper disable UnusedMember.Global

namespace KmyTarkovApi.Helpers
{
    public class SessionHelper
    {
        private static readonly Lazy<SessionHelper> Lazy = new Lazy<SessionHelper>(() => new SessionHelper());

        public static SessionHelper Instance => Lazy.Value;

        public ISession Session { get; private set; }

        public TraderSettingsData TraderSettingsHelper => TraderSettingsData.Instance;

        public ExperienceData ExperienceHelper => ExperienceData.Instance;

        /// <summary>
        ///     Init Action
        /// </summary>
        public readonly RefHelper.HookRef CreateBackend;

        public readonly RefHelper.HookRef AfterApplicationLoaded;

        private SessionHelper()
        {
            var applicationType = typeof(TarkovApplication);

            CreateBackend = RefHelper.HookRef.Create(applicationType,
                x => x.IsAsync() && x.ReturnType == typeof(Task) &&
                     x.ReadMethodBody().ContainsIL(OpCodes.Ldstr,
                         "_backEnd.Session.GetGlobalConfig+GetClientSettingsConfig"));

            AfterApplicationLoaded = RefHelper.HookRef.Create(applicationType,
                x => x.GetParameters().Length == 0 && x.ReturnType == typeof(void) &&
                     x.ReadMethodBody().ContainsIL(OpCodes.Callvirt, AccessTools.Method(typeof(Action), "Invoke")));
        }

        [EFTHelperHook]
        private void Hook()
        {
            CreateBackend.Add(this, nameof(OnCreateBackend));
        }

        private static async void OnCreateBackend(TarkovApplication __instance, Task __result)
        {
            await __result;

            var session = __instance.GetClientBackEndSession();

            Instance.Session = session;
        }

        public class TraderSettingsData
        {
            private static readonly Lazy<TraderSettingsData> Lazy =
                new Lazy<TraderSettingsData>(() => new TraderSettingsData());

            public static TraderSettingsData Instance => Lazy.Value;

            private TraderSettingsData()
            {
            }

            public Task<Sprite> GetAvatar(string traderId)
            {
                var traders = SessionHelper.Instance.Session?.Traders;

                if (traders == null)
                    return Task.FromResult<Sprite>(null);

                foreach (var trader in traders)
                {
                    var settings = trader.Settings;

                    if (settings.Id == traderId)
                        return settings.GetAvatar();
                }

                return Task.FromResult<Sprite>(null);
            }

            public async void BindAvatar(string traderId, Action<Sprite> action)
            {
                var sprite = await GetAvatar(traderId);

                action?.Invoke(sprite);
            }
        }

        public class ExperienceData
        {
            private static readonly Lazy<ExperienceData> Lazy = new Lazy<ExperienceData>(() => new ExperienceData());

            public static ExperienceData Instance => Lazy.Value;

            public object Experience => RefExperience.GetValue(SessionHelper.Instance.Session?.BackEndConfig.Config);

            public object Kill => RefKill.GetValue(Experience);

            public int VictimLevelExp => RefVictimLevelExp.GetValue(Kill);

            public int VictimBotLevelExp => RefVictimBotLevelExp.GetValue(Kill);

            public float PmcHeadShotMult => RefPmcHeadShotMult.GetValue(Kill);

            public float BotHeadShotMult => RefBotHeadShotMult.GetValue(Kill);

            public readonly RefHelper.FieldRef<BackendConfigSettingsClass, object> RefExperience;

            public readonly RefHelper.FieldRef<object, object> RefKill;

            public readonly RefHelper.FieldRef<object, int> RefVictimLevelExp;

            public readonly RefHelper.FieldRef<object, int> RefVictimBotLevelExp;

            public readonly RefHelper.FieldRef<object, float> RefPmcHeadShotMult;

            public readonly RefHelper.FieldRef<object, float> RefBotHeadShotMult;

            private readonly Func<object, int, int> _refGetKillingBonusPercent;

            private ExperienceData()
            {
                RefExperience = RefHelper.FieldRef<BackendConfigSettingsClass, object>.Create("Experience");
                RefKill = RefHelper.FieldRef<object, object>.Create(RefExperience.FieldType, "Kill");
                RefVictimLevelExp = RefHelper.FieldRef<object, int>.Create(RefKill.FieldType, "VictimLevelExp");
                RefVictimBotLevelExp = RefHelper.FieldRef<object, int>.Create(RefKill.FieldType, "VictimBotLevelExp");

                RefPmcHeadShotMult = RefHelper.FieldRef<object, float>.Create(RefKill.FieldType, "PmcHeadShotMult");
                RefBotHeadShotMult = RefHelper.FieldRef<object, float>.Create(RefKill.FieldType, "BotHeadShotMult");

                _refGetKillingBonusPercent = RefHelper.ObjectMethodDelegate<Func<object, int, int>>(
                    RefTool.GetEftMethod(x => x.GetMethod("GetKillingBonusPercent") != null, RefTool.Public,
                        x => x.Name == "GetKillingBonusPercent"));
            }

            public int GetBaseExp(int exp, EPlayerSide side, WildSpawnType role, float scavKillExpPenalty,
                bool hasMarkOfUnknown, bool isAI)
            {
                switch (side)
                {
                    case EPlayerSide.Usec:
                    case EPlayerSide.Bear:
                        return VictimLevelExp;
                    case EPlayerSide.Savage:
                        if (exp < 0)
                        {
                            exp = VictimBotLevelExp;
                        }

                        if ((role == WildSpawnType.assault || role == WildSpawnType.marksman || !isAI) &&
                            hasMarkOfUnknown)
                        {
                            exp = Mathf.CeilToInt(exp * scavKillExpPenalty);
                        }

                        return exp;
                    default:
                        return 0;
                }
            }

            public int GetHeadExp(int baseExp, EPlayerSide side)
            {
                return (int)(baseExp * (side != EPlayerSide.Savage ? PmcHeadShotMult : BotHeadShotMult));
            }

            public int GetStreakExp(int baseExp, int kills)
            {
                return (int)(baseExp * (Kill != null ? GetKillingBonusPercent(Kill, kills) : 0 / 100f));
            }

            public int GetKillingBonusPercent(object instance, int killed)
            {
                return _refGetKillingBonusPercent(instance, killed);
            }
        }
    }
}