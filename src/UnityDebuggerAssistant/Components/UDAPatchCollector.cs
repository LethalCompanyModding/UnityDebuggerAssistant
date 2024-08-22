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

        if (Plugin.EnableExperimentalMode.Value)
        {
            Plugin.Log?.LogInfo("UDA starting patch processor cycle");
            StartCoroutine(CheckPatches());
        }

#if DEBUG
        UDAExceptionHandler.DebugThrow();
#endif
    }

    internal IEnumerator CheckPatches()
    {
        yield return new WaitForSeconds(1f);

        if (ExceptionConstructorPatch.Storage.Count > 0)
        {

#if DEBUG
            Plugin.Log?.LogInfo($"No. to process {ExceptionConstructorPatch.Storage.Count}");
#endif
            List<CollectedException> process = [];
            List<CollectedException> remove = [];

            foreach (var item in ExceptionConstructorPatch.Storage)
            {

                item.Tries++;

                if (item.ex.TargetSite is not null)
                {
                    process.Add(item);
                    continue;
                }

                if (item.Tries > 59)
                {
                    remove.Add(item);
                }
            }

            foreach (var item in process)
            {
                ExceptionConstructorPatch.Storage.Remove(item);
                ExceptionProcessor.Run(item.ex);
            }

            foreach (var item in remove)
            {
                ExceptionConstructorPatch.Storage.Remove(item);
            }
        }

        StartCoroutine(CheckPatches());
        yield break;
    }
}

internal class CollectedException(Exception ex)
{
    public int Tries = 0;
    public readonly Exception ex = ex;
}
