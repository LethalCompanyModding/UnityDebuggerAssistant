using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MonoMod.RuntimeDetour;
using UnityDebuggerAssistant.Patches;
using UnityDebuggerAssistant.Utils;
using UnityEngine;

/*
  Here are some basic resources on code style and naming conventions to help
  you in your first CSharp plugin!

  https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions
  https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names
  https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/names-of-namespaces
*/

[BepInPlugin(LCMPluginInfo.PLUGIN_GUID, LCMPluginInfo.PLUGIN_NAME, LCMPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
  public static ManualLogSource Log = null!;

  private void Awake()
  {
    /*
      BepinEx makes you a ManualLogSource for free called "Logger"
      and I created a static value above to hold on to it so other
      parts of your plugin's code can find it by using Plugin.Log

      We assign it here
    */
    Log = Logger;

    // Log our awake here so we can see it in LogOutput.txt file
    Log.LogInfo($"Plugin {LCMPluginInfo.PLUGIN_NAME} is loaded!");

    // Add listeners
    ILHook.OnDetour += MonoModPatchListener.ListenForPatch;

    Harmony harmony = new(LCMPluginInfo.PLUGIN_GUID);
    harmony.PatchAll(typeof(StartOfRoundPatches));
    harmony.PatchAll(typeof(ExceptionPatches));

    PatchStorage.AddPatchInformation(typeof(HarmonyPatchMarshal).GetMethod("RunMarshal"), this.GetType().Assembly);

    //Uncomment this to get a demo exception to test with
    //ExceptionHandler.DebugThrow();

  }

}
