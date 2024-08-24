using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using HarmonyLib;

namespace UnityDebuggerAssistant.Utils;

internal static class UDAPatchStorage
{
    private static readonly Dictionary<MethodBase, List<Assembly>> Patches = [];
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
        UDAPlugin.Log?.LogInfo($"blame {assembly.GetName().Name} for {method.FullDescription()}");
        return true;
    }

    internal static List<Assembly> GetPatchInformation(MethodBase method)
    {
        if (!Patches.TryGetValue(method, out List<Assembly> inList))
            return [];

        return inList;
    }
}
