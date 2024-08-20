using HarmonyLib;
using UnityDebuggerAssistant.Utils;

namespace UnityDebuggerAssistant.Patches;

internal class StartOfRoundPatches
{
    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.Start))]
    [HarmonyPostfix]
    internal static void UpdateHarmonyListStart() => HarmonyPatchMarshal.RunMarshal();

    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.StartGame))]
    [HarmonyPostfix]
    internal static void UpdateHarmonyListStartGame() => HarmonyPatchMarshal.RunMarshal();
}
