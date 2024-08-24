using System;
using System.Reflection;
using System.Collections.Generic;
using HarmonyLib;
using UnityDebuggerAssistant.Processing;

namespace UnityDebuggerAssistant.Patches;


public static class ExceptionStackGetterPatch
{

    private static bool LockRoot = false;

    [HarmonyPatch(typeof(Exception), nameof(Exception.StackTrace), MethodType.Getter)]
    [HarmonyPostfix]
    internal static void ExceptionStackGot(Exception __instance)
    {
        if (LockRoot)
            return;

        LockRoot = true;

        //Skip the line
        UDAExceptionProcessor.Run(__instance);

        LockRoot = false;
    }
}

[HarmonyPatch]
public static class ExceptionConstructorPatch
{

    static IEnumerable<MethodBase> TargetMethods()
    {
        return typeof(Exception).GetConstructors(BindingFlags.Public | BindingFlags.Instance);
    }

    static void Postfix(Exception __instance)
    {
        UDAExceptionProcessor.PushException(__instance);
    }
}
