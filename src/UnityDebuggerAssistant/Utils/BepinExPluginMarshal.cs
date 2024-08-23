using System.Diagnostics;

namespace UnityDebuggerAssistant.Utils;

public static class BepinExPluginMarshal
{
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
                PatchStorage.AddPatchInformation(typeof(UDAExceptionHandler).GetMethod(nameof(UDAExceptionHandler.DebugThrow)), assembly);
            }
#endif

            //Add to relational database Assembly => Info
            if (info is not null)
                PatchStorage.AddToInfoCache(assembly, info);
        }

        //Announce time taken
        timer.Stop();
        Plugin.Log?.LogInfo($"BepinEx Plugins Marshaled in {timer.ElapsedMilliseconds}ms");
    }
}
