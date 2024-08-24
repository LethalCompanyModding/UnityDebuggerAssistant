using System.Reflection;
using System.Collections.Generic;
using UnityDebuggerAssistant.Utils;

namespace UnityDebuggerAssistant.Filtering;

internal static class UDAWhitelist
{
    internal readonly static List<Assembly> AssemblyWhiteList = [
        Helpers.GetAssemblyByName("Assembly-CSharp"),
        Helpers.GetAssemblyByName("MMHOOK_Assembly-CSharp")
    ];

    internal static bool ExceptionWhitelist(Assembly assembly) => Whitelist(UDAPlugin.UDASettings.EnableWhitelistPerException.Value, assembly);
    internal static bool FrameWhitelist(Assembly assembly) => Whitelist(UDAPlugin.UDASettings.EnableWhitelistPerFrame.Value, assembly);
    private static bool Whitelist(bool Setting, Assembly assembly)
    {
        //If its a plugin, its always a valid target
        if (UDAPluginMarshal.InfoCache.ContainsKey(assembly))
            return true;

        //If the whitelist is enabled then check for it in the list
        return !Setting || (Setting && AssemblyWhiteList.Contains(assembly));
    }
}
