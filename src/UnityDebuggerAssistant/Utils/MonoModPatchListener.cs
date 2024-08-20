using System.Reflection;
using HarmonyLib;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace UnityDebuggerAssistant.Utils;

public static class MonoModPatchListener
{
    internal static bool ListenForPatch(ILHook hook, MethodBase @base, ILContext.Manipulator manipulator)
    {
        PatchStorage.AddPatchInformation(@base, manipulator.Target.GetType().Assembly);
        return true;
    }
}
