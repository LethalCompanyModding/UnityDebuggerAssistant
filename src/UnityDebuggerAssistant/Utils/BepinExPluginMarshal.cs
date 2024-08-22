using System.Diagnostics;

namespace UnityDebuggerAssistant.Utils;

public static class BepinExPluginMarshal
{
    public static void Run()
    {
        Stopwatch timer = new();
        timer.Start();

        //Get a list of all plugin GUIDs from the Chain loader
        var guids = BepInEx.Bootstrap.Chainloader.PluginInfos.Keys;

        foreach (var guid in guids)
        {
            var assembly = BepInEx.Bootstrap.Chainloader.PluginInfos[guid].Instance.GetType().Assembly;
            var info = BepInEx.Bootstrap.Chainloader.PluginInfos[guid];

            //Add to relational database Assembly => Info
            if (info is not null)
                PatchStorage.AddToInfoCache(assembly, info);
        }

        //Announce time taken
        timer.Stop();
        Plugin.Log?.LogInfo($"BepinEx Plugins Marshaled in {timer.ElapsedMilliseconds}ms");
    }
}
