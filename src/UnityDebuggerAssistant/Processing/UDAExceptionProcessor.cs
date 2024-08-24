using System;
using System.Collections.Concurrent;

namespace UnityDebuggerAssistant.Processing;
internal static class UDAExceptionProcessor
{
    internal static readonly ConcurrentStack<Exception> Storage = [];
    internal static readonly int[] LastInExceptions = new int[10];
    internal static int LastInPosition = 0;
    static bool OperationUnderway = false;

    internal static void PushException(Exception ex)
    {
        int hash = ex.GetHashCode();

        foreach (var item in LastInExceptions)
        {
            if (hash == item)
            {
#if DEBUG
                UDAPlugin.Log?.LogInfo("Seen this one before");
#endif
                return;
            }

        }

        LastInExceptions[LastInPosition] = hash;
        LastInPosition++;
        LastInPosition %= LastInExceptions.Length;

        Storage.Push(ex);
    }

    internal static void Run(Exception __instance)
    {
        if (OperationUnderway)
        {
            UDAPlugin.Log?.LogWarning("Exception processor is busy");
            return;
        }

        OperationUnderway = true;

        try
        {

#if DEBUG
            UDAPlugin.Log?.LogInfo(__instance.GetType());
#endif
            UDAExceptionHandler.Handle(__instance);
        }
        catch (Exception e)
        {
            UDAPlugin.Log?.LogError("The exception handler failed while running. Please report the next line to Robyn:");
            UDAPlugin.Log?.LogError(e);
        }
        finally
        {
            OperationUnderway = false;
        }
    }
}
