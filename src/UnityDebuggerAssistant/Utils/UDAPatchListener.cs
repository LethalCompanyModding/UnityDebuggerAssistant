using System.Reflection;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace UnityDebuggerAssistant.Utils;

public static class UDAPatchListener
{
    internal static bool ListenForPatch(ILHook hook, MethodBase @base, ILContext.Manipulator manipulator)
    {

        if (@base is null || manipulator is null || hook is null || manipulator.Target is null)
        {
            UDAPlugin.Log?.LogInfo("Skipping a null/empty patch");
            return true;
        }

        /*
        Plugin.Log?.LogInfo(hook.Manipulator);
        Plugin.Log?.LogInfo(@base.Name);
        Plugin.Log?.LogInfo(manipulator);
        */

        UDAPatchStorage.AddPatchInformation(@base, manipulator.Target.GetType().Assembly);
        return true;
    }
}
