using HarmonyLib;
using UnityDebuggerAssistant.Utils;

namespace UnityDebuggerAssistant.Patches;

internal class StartOfRoundPatches
{
    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.ChangeLevel))]
    [HarmonyPostfix]
    internal static void UpdateHarmonyListPatch() => HarmonyPatchMarshal.RunMarshal();

    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.StartGame))]
    [HarmonyPostfix]
    internal static void UpdateHarmonyListPatch2() => HarmonyPatchMarshal.RunMarshal();
}
