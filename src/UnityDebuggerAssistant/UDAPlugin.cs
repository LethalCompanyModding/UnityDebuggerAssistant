using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using MonoMod.RuntimeDetour;
using UnityDebuggerAssistant.Components;
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
public class UDAPlugin : BaseUnityPlugin
{
  public static ManualLogSource? Log;
  internal static ConfigEntry<bool> EnableWhitelisting = null!;

  private void Awake()
  {
    /*
      BepinEx makes you a ManualLogSource for free called "Logger"
      and I created a static value above to hold on to it so other
      parts of your plugin's code can find it by using Plugin.Log

      We assign it here
    */
    Log = Logger;

    EnableWhitelisting = Config.Bind(new("ExceptionHandler", "EnableWhiteList"), true, new("By default the whitelist ensures we only check exceptions inside Assembly-Csharp, plugins and common game assemblies.\nDisable this to catch everything. Will somewhat effect performance."));

    // Log our awake here so we can see it in LogOutput.txt file
    Log.LogInfo($"Plugin {LCMPluginInfo.PLUGIN_NAME} is loaded!");
    Log.LogInfo($"Using whitelist: {EnableWhitelisting.Value}");

    // Add listeners
    ILHook.OnDetour += UDAPatchListener.ListenForPatch;

    Harmony harmony = new(LCMPluginInfo.PLUGIN_GUID);

    harmony.PatchAll(typeof(ExceptionConstructorPatch));

    Log.LogInfo("Deferring plugin collection..");

    var go = new GameObject("UDA Patch Collector", [
      typeof(UDAExceptionStacker)
    ])
    {
      hideFlags = HideFlags.HideAndDontSave
    };
    DontDestroyOnLoad(go);

  }

}
