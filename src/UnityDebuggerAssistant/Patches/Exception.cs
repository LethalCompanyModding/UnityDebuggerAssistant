using System;
using HarmonyLib;
using UnityDebuggerAssistant.Utils;

namespace UnityDebuggerAssistant.Patches;

public static class ExceptionPatches
{

    static bool OperationUnderway = false;

    [HarmonyPatch(typeof(Exception), nameof(Exception.StackTrace), MethodType.Getter)]
    [HarmonyPostfix]
    public static void ExceptionPatch(Exception __instance)
    {
        //Plugin.Log.LogInfo(__instance.GetType());

        if (OperationUnderway)
        {
            Plugin.Log.LogInfo("I'm busy");
            return;
        }

        OperationUnderway = true;

        try
        {
            UDAExceptionHandler.Handle(__instance);
        }
        catch (Exception e)
        {
            Plugin.Log.LogError("The exception handler failed while running. Please report the next line to Robyn:");
            Plugin.Log.LogError(e);
        }
        finally
        {
            OperationUnderway = false;
        }
    }
}
