using System;
using System.Reflection;
using EFTReflection;
using HarmonyLib;

// ReSharper disable UnusedMember.Global

namespace EFTApi.Helpers
{
    public class RequestHandlerHelper
    {
        private static readonly Lazy<RequestHandlerHelper> Lazy =
            new Lazy<RequestHandlerHelper>(() => new RequestHandlerHelper());

        public static RequestHandlerHelper Instance => Lazy.Value;

        private readonly Func<string, string, string> _refPutJson;

        private readonly Action<string, string> _refBelow382PutJson;

        private readonly Action<string, string, bool> _refBelow380PutJson;

        private readonly Func<string, string, string> _refPostJson;

        private readonly Func<string, string, bool, string> _refBelow380PostJson;

        private RequestHandlerHelper()
        {
            var requestHandlerType = EFTVersion.AkiVersion > EFTVersion.Parse("3.8.3")
                ? EFTPlugins.AkiCommon.GetType("SPT.Common.Http.RequestHandler")
                : EFTPlugins.AkiCommon.GetType("Aki.Common.Http.RequestHandler");

            if (EFTVersion.AkiVersion > EFTVersion.Parse("3.8.1"))
            {
                _refPutJson = AccessTools.MethodDelegate<Func<string, string, string>>(
                    RefTool.GetEftMethod(requestHandlerType, BindingFlags.Static | RefTool.Public,
                        x => x.Name == "PutJson" && x.GetParameters().Length == 2));

                _refPostJson = AccessTools.MethodDelegate<Func<string, string, string>>(
                    RefTool.GetEftMethod(requestHandlerType, BindingFlags.Static | RefTool.Public,
                        x => x.Name == "PostJson" && x.GetParameters().Length == 2));
            }
            else if (EFTVersion.AkiVersion > EFTVersion.Parse("3.7.6"))
            {
                _refBelow382PutJson = AccessTools.MethodDelegate<Action<string, string>>(
                    RefTool.GetEftMethod(requestHandlerType, BindingFlags.Static | RefTool.Public,
                        x => x.Name == "PutJson" && x.GetParameters().Length == 2));

                _refPostJson = AccessTools.MethodDelegate<Func<string, string, string>>(
                    RefTool.GetEftMethod(requestHandlerType, BindingFlags.Static | RefTool.Public,
                        x => x.Name == "PostJson" && x.GetParameters().Length == 2));
            }
            else
            {
                _refBelow380PutJson = AccessTools.MethodDelegate<Action<string, string, bool>>(
                    RefTool.GetEftMethod(requestHandlerType, BindingFlags.Static | RefTool.Public,
                        x => x.Name == "PutJson" && x.GetParameters().Length == 3));

                _refBelow380PostJson = AccessTools.MethodDelegate<Func<string, string, bool, string>>(
                    RefTool.GetEftMethod(requestHandlerType, BindingFlags.Static | RefTool.Public,
                        x => x.Name == "PostJson" && x.GetParameters().Length == 3));
            }
        }

        public string PutJson(string path, string json)
        {
            if (EFTVersion.AkiVersion > EFTVersion.Parse("3.8.3"))
                return _refPutJson(path, json);

            if (EFTVersion.AkiVersion > EFTVersion.Parse("3.7.6"))
            {
                _refBelow382PutJson(path, json);
            }
            else
            {
                _refBelow380PutJson(path, json, false);
            }

            return string.Empty;
        }

        public string PostJson(string path, string json)
        {
            return EFTVersion.AkiVersion > EFTVersion.Parse("3.7.6")
                ? _refPostJson(path, json)
                : _refBelow380PostJson(path, json, false);
        }
    }
}