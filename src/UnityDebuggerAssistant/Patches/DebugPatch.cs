using HarmonyLib;
using UnityDebuggerAssistant.Processing;

namespace UnityDebuggerAssistant.Patches;

public class DebugPatches
{
    [HarmonyPatch(typeof(UDAExceptionHandler), nameof(UDAExceptionHandler.DebugThrow))]
    [HarmonyPrefix]
    public static void Prefix()
    {
        UDAPlugin.Log?.LogInfo("Debug Throw Prefix");
    }
}
