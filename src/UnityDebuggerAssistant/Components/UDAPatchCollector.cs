using System;
using System.Collections;
using System.Collections.Generic;
using UnityDebuggerAssistant.Patches;
using UnityDebuggerAssistant.Utils;
using UnityEngine;

namespace UnityDebuggerAssistant.Components;

public class UDAPatchCollector : MonoBehaviour
{
    private void Start()
    {
        Plugin.Log?.LogInfo("UDA post-Chainloader startup");
        BepinExPluginMarshal.Run();

        Plugin.Log?.LogInfo("UDA starting patch processor cycle");
        StartCoroutine(CheckPatches());

#if DEBUG
        UDAExceptionHandler.DebugThrow();
#endif
    }

    internal IEnumerator CheckPatches()
    {
        yield return new WaitForEndOfFrame();

        if (ExceptionConstructorPatch.Storage.Count > 0)
        {

#if DEBUG
            Plugin.Log?.LogInfo($"No. to process {ExceptionConstructorPatch.Storage.Count}");
#endif

            foreach (var item in ExceptionConstructorPatch.Storage)
            {
                UDAExceptionHandler.Handle(item);
            }

            ExceptionConstructorPatch.Storage.Clear();
        }

        StartCoroutine(CheckPatches());
        yield break;
    }
}
