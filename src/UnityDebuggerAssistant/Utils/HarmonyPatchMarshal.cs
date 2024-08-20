using System.Diagnostics;
using System.Linq;
using System.Text;
using HarmonyLib;

namespace UnityDebuggerAssistant.Utils;

public static class HarmonyPatchMarshal
{
    public static void RunMarshal()
    {
        Stopwatch timer = new();
        timer.Start();

        //Get a list of all plugin GUIDs from the Chain loader
        var guids = BepInEx.Bootstrap.Chainloader.PluginInfos.Keys;

        //Access the harmony instance for each one
        Harmony harmony;

        foreach (var guid in guids)
        {

            var assembly = BepInEx.Bootstrap.Chainloader.PluginInfos[guid].Instance.GetType().Assembly;
            var info = BepInEx.Bootstrap.Chainloader.PluginInfos[guid];

            if (info is not null)
                PatchStorage.AddToInfoCache(assembly, info);

            harmony = new(guid);
            var methods = harmony.GetPatchedMethods();

            if (methods.Count() == 0)
                continue;

            //Access each MethodBase for each patch
            foreach (var method in methods)
            {
                PatchStorage.AddPatchInformation(method, assembly);
            }
        }

        //Announce
        timer.Stop();
        Plugin.Log.LogInfo($"Harmony Marshal is updated in {timer.ElapsedMilliseconds}ms");
    }
}
