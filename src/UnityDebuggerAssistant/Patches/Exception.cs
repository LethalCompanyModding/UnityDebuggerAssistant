using System;
using HarmonyLib;
using UnityDebuggerAssistant.Utils;

namespace UnityDebuggerAssistant.Patches;

public static class ExceptionPatches
{
    [HarmonyPatch(typeof(Exception), nameof(Exception.StackTrace), MethodType.Getter)]
    [HarmonyPostfix]
    public static void ExceptionPatch(Exception __instance) => ExceptionHandler.Handle(__instance);
}
