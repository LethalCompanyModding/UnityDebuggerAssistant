using System;
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

#if DEBUG
        UDAExceptionHandler.DebugThrow();
#endif
    }

    private void Update()
    {
        var storage = ExceptionConstructorPatch.Storage;

        //Potential optimization: public int TryPopRange (T[] items);

        if (storage.TryPop(out Exception result))
        {
#if DEBUG
            Plugin.Log?.LogInfo("Popping from stack this frame");
#endif 
            ExceptionProcessor.Run(result);
        }

    }
}
