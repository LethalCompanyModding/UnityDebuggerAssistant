using System;
using System.Reflection;
using System.Collections.Generic;
using HarmonyLib;
using UnityDebuggerAssistant.Processing;

namespace UnityDebuggerAssistant.Patches;


public static class ExceptionStackGetterPatch
{
    [HarmonyPatch(typeof(Exception), nameof(Exception.StackTrace), MethodType.Getter)]
    [HarmonyPostfix]
    internal static void ExceptionStackGot(Exception __instance)
    {
        UDAExceptionProcessor.PushException(__instance);
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
