using System;
using UnityDebuggerAssistant.Patches;
using UnityDebuggerAssistant.Utils;
using UnityEngine;

namespace UnityDebuggerAssistant.Components;

public class UDAExceptionBroker : MonoBehaviour
{

    private Exception[] popped = new Exception[3];

    private void Start()
    {
        UDAPlugin.Log?.LogInfo("UDA post-Chainloader startup");
        UDAPluginMarshal.Run();

        UDAPlugin.Log?.LogInfo("UDA starting patch processor cycle");

#if DEBUG
        UDAExceptionHandler.DebugThrow();
#endif
    }

    private void Update()
    {
        var storage = ExceptionConstructorPatch.Storage;
        var popAmount = storage.TryPopRange(popped);

        if (popAmount > 0)
        {
#if DEBUG
            UDAPlugin.Log?.LogInfo($"Popped from stack => {popAmount}");
#endif 

            for (int i = 0; i < popAmount; i++)
            {
                UDAExceptionHandler.Handle(popped[i]);
            }

        }

    }
}
