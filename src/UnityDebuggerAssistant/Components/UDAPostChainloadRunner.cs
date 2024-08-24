using UnityDebuggerAssistant.Utils;
using UnityEngine;

namespace UnityDebuggerAssistant.Components;

public class UDAPostChainloadRunner : MonoBehaviour
{
    private void Start()
    {
        UDAPlugin.Log?.LogInfo("UDAPostChainloadRunner Starting");
        UDAPluginMarshal.Run();

#if DEBUG
        UDAExceptionHandler.DebugThrow();
#endif
    }
}
