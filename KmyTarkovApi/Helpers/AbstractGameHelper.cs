﻿using System;
using EFT;
using HarmonyLib;
using KmyTarkovReflection;

// ReSharper disable MemberCanBePrivate.Global

namespace KmyTarkovApi.Helpers
{
    public class AbstractGameHelper
    {
        private static readonly Lazy<AbstractGameHelper> Lazy =
            new Lazy<AbstractGameHelper>(() => new AbstractGameHelper());

        public static AbstractGameHelper Instance => Lazy.Value;

        public AbstractGame AbstractGame { get; private set; }

        /// <summary>
        ///     Init Action
        /// </summary>
        public readonly RefHelper.HookRef Constructor;

        private AbstractGameHelper()
        {
            var abstractGameType = typeof(AbstractGame);

            Constructor = RefHelper.HookRef.Create(abstractGameType.GetConstructors(AccessTools.all)[0]);
        }

        [EFTHelperHook]
        private void Hook()
        {
            Constructor.Add(this, nameof(OnConstructor));
        }

        private static void OnConstructor(AbstractGame __instance)
        {
            Instance.AbstractGame = __instance;
        }
    }
}