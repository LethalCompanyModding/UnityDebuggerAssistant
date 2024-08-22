using System;
using System.Reflection;
using System.Collections.Generic;
using HarmonyLib;
using UnityDebuggerAssistant.Utils;
using UnityDebuggerAssistant.Components;

namespace UnityDebuggerAssistant.Patches;

public static class ExceptionTracePatch
{

    [HarmonyPatch(typeof(Exception), nameof(Exception.StackTrace), MethodType.Getter)]
    [HarmonyPostfix]
    public static void Patch(Exception __instance) => ExceptionProcessor.Run(__instance);
}

[HarmonyPatch]
public static class ExceptionConstructorPatch
{

    internal static readonly List<CollectedException> Storage = [];

    static IEnumerable<MethodBase> TargetMethods()
    {
        return typeof(Exception).GetConstructors(BindingFlags.Public | BindingFlags.Instance);
    }

    static void Postfix(Exception __instance)
    {
        foreach (var item in Storage)
        {
            if (item.ex == __instance)
                return;
        }

        Storage.Add(new(__instance));
    }
}
internal static class ExceptionProcessor
{
    static bool OperationUnderway = false;
    static Exception LastInEx = null!;

    internal static void Run(Exception __instance)
    {
        if (OperationUnderway)
        {
            Plugin.Log?.LogWarning("Exception processor is busy");
            return;
        }

        if (LastInEx == __instance)
        {
#if DEBUG
            Plugin.Log?.LogInfo("Seen this one before");
#endif
            return;
        }


        OperationUnderway = true;
        LastInEx = __instance;

        try
        {

#if DEBUG
            Plugin.Log?.LogInfo(__instance.GetType());
#endif
            UDAExceptionHandler.Handle(__instance);
        }
        catch (Exception e)
        {
            Plugin.Log?.LogError("The exception handler failed while running. Please report the next line to Robyn:");
            Plugin.Log?.LogError(e);
        }
        finally
        {
            OperationUnderway = false;
        }
    }
}
