using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MonoMod.RuntimeDetour;
using System.Linq;
using System.Text;
using UnityDebuggerAssistant.Components;
using UnityDebuggerAssistant.Filtering;
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
  internal static ManualLogSource? Log;
  internal static PluginConfiguration UDASettings = null!;

  private void Awake()
  {
    /*
      BepinEx makes you a ManualLogSource for free called "Logger"
      and I created a static value above to hold on to it so other
      parts of your plugin's code can find it by using Plugin.Log

      We assign it here
    */
    Log = Logger;
    UDASettings = new(Config);

    //Setup filters
    UDABlacklist.UpdateLists_Internal();

    /************************************************************
      Output big warning message here to help people understand
      the actual use of this plugin, also log configuration
    ************************************************************/

    StringBuilder sb = new("""


                  >------------------------------------------------------------------------<
                    Unity Debugger assistant is loaded and receiving exceptions!

                    Note: That UDA will catch ALL exceptions, even harmless ones
                    Please ONLY report logs to modders if you have an issue with their mod

                    Config:

                  """);
    sb.Append("    Using Per-Exception Whitelist: ");
    sb.AppendLine(UDASettings.EnableWhitelistPerException.Value.ToString());
    sb.Append("    Using Per-Exception Blacklist: ");
    sb.AppendLine(UDASettings.EnableBlacklistPerException.Value.ToString());
    sb.Append("    Using Per-Frame Whitelist: ");
    sb.AppendLine(UDASettings.EnableWhitelistPerFrame.Value.ToString());
    sb.Append("    Using Per-Frame Blacklist: ");
    sb.AppendLine(UDASettings.EnableBlacklistPerFrame.Value.ToString());
    sb.Append("    Total Filters: ");
    sb.AppendLine((UDABlacklist.ExceptionBlackList.Count() + UDABlacklist.FrameBlackList.Count()).ToString());
    sb.AppendLine(">------------------------------------------------------------------------<");

    Log.LogMessage(sb);

    // Add listeners
    ILHook.OnDetour += UDAPatchListener.ListenForPatch;

    Harmony harmony = new(LCMPluginInfo.PLUGIN_GUID);

    harmony.PatchAll(typeof(DebugPatches));
    harmony.PatchAll(typeof(ExceptionConstructorPatch));
    //harmony.PatchAll(typeof(ExceptionStackGetterPatch));

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
