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

    /************************************************************
      Output big warning message here to help people understand
      the actual use of this plugin
    ************************************************************/

    Log.LogMessage("""

                  >------------------------------------------------------------------------<
                    Unity Debugger assistant is loaded and receiving exceptions!

                    Note: That UDA will catch ALL exceptions, even harmless ones
                    Please ONLY report logs to modders if you have an issue with their mod
                  >------------------------------------------------------------------------<
                  """);

    EnableWhitelisting = Config.Bind(new("ExceptionHandler", "EnableWhiteList"), true, new("By default the whitelist ensures we only check exceptions inside Assembly-Csharp, plugins and common game assemblies.\nDisable this to catch everything. Will somewhat effect performance."));

    //Log configuration here
    Log.LogInfo($"Using whitelist: {EnableWhitelisting.Value}");

    // Add listeners
    ILHook.OnDetour += UDAPatchListener.ListenForPatch;

    Harmony harmony = new(LCMPluginInfo.PLUGIN_GUID);

    harmony.PatchAll(typeof(ExceptionConstructorPatch));

    var go = new GameObject("UDAMain", [
      typeof(UDAExceptionBroker),
      typeof(UDAPostChainloadRunner)
    ])
    {
      hideFlags = HideFlags.HideAndDontSave
    };
    DontDestroyOnLoad(go);

  }

}
