using System;
using UnityDebuggerAssistant.Processing;
using UnityEngine;

namespace UnityDebuggerAssistant.Components;

public class UDAExceptionBroker : MonoBehaviour
{

    private readonly Exception[] popped = new Exception[3];

    private void Awake()
    {
        UDAPlugin.Log?.LogInfo("UDAExceptionBroker active");
    }

    private void Update()
    {
        var storage = UDAExceptionProcessor.Storage;
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
