using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using BepInEx;
using UnityDebuggerAssistant.Processing;

namespace UnityDebuggerAssistant.Utils;

public static class UDAPluginMarshal
{
    internal static readonly Dictionary<Assembly, PluginInfo> InfoCache = [];
    public static void Run()
    {
        Stopwatch timer = new();
        timer.Start();

#if DEBUG
        int blames = 0;
#endif

        //Get a list of all plugin GUIDs from the Chain loader
        var guids = BepInEx.Bootstrap.Chainloader.PluginInfos.Keys;

        foreach (var guid in guids)
        {
            var assembly = BepInEx.Bootstrap.Chainloader.PluginInfos[guid].Instance.GetType().Assembly;
            var info = BepInEx.Bootstrap.Chainloader.PluginInfos[guid];

#if DEBUG
            if (blames < 3)
            {
                blames++;
                UDAPatchStorage.AddPatchInformation(typeof(UDAExceptionHandler).GetMethod(nameof(UDAExceptionHandler.DebugThrow)), assembly);
            }
#endif

            //Add to relational database Assembly => Info
            if (info is not null)
                AddToInfoCache(assembly, info);
        }

        //Announce time taken
        timer.Stop();
        UDAPlugin.Log?.LogInfo($"BepinEx Plugins Marshaled in {timer.ElapsedMilliseconds}ms");
    }

    internal static bool AddToInfoCache(Assembly assembly, PluginInfo info)
    {
        //waah
        if (InfoCache.ContainsKey(assembly))
            return false;

        InfoCache.Add(assembly, info);

        return true;
    }
}
