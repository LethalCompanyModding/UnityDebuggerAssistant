using UnityDebuggerAssistant.Utils;
using UnityEngine;

namespace UnityDebuggerAssistant.Components;

public class UDAPatchCollector : MonoBehaviour
{
    private void Start()
    {
        Plugin.Log.LogInfo("UDA post-Chainloader startup");
        BepinExPluginMarshal.Run();

        UDAExceptionHandler.DebugThrow();
    }
}
