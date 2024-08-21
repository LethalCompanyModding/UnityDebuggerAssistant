using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using HarmonyLib;

namespace UnityDebuggerAssistant.Utils;

internal static class PatchStorage
{
    private static readonly Dictionary<MethodBase, List<Assembly>> Patches = [];
    internal static readonly Dictionary<Assembly, PluginInfo> InfoCache = [];
    private static readonly Assembly HarmonyAssembly = typeof(Harmony).Assembly;

    internal static bool AddPatchInformation(MethodBase method, Assembly assembly)
    {
        //Ignore Harmony because its doing its own thing
        if (assembly is null || assembly == HarmonyAssembly)
            return false;

        if (!Patches.TryGetValue(method, out List<Assembly> inList))
            Patches[method] = inList = [];

        if (inList.Contains(assembly))
            return false;

        inList.Add(assembly);
        Plugin.Log.LogInfo($"blame {assembly.GetName().Name} for {method.FullDescription()}");
        return true;
    }

    internal static List<Assembly> GetPatchInformation(MethodBase method)
    {
        if (!Patches.TryGetValue(method, out List<Assembly> inList))
            return [];

        return inList;
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
