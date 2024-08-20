using System.Linq;
using System.Text;
using HarmonyLib;

namespace UnityDebuggerAssistant.Utils;

public static class HarmonyPatchMarshal
{
    public static void RunMarshal()
    {
        //Get a list of all plugin GUIDs from the Chain loader
        var guids = BepInEx.Bootstrap.Chainloader.PluginInfos.Keys;

        //Access the harmony instance for each one
        Harmony harmony;

        //Use a builder cuz why not
        StringBuilder sb = new("\n\n");

        sb.AppendLine("--- BEGIN MARSHAL ---\n");

        foreach (var guid in guids)
        {
            sb.Append('[');
            sb.Append(guid);
            sb.Append(']');
            sb.AppendLine();

            harmony = new(guid);
            var methods = harmony.GetPatchedMethods();

            if (methods.Count() == 0)
            {
                sb.AppendLine("  Patches nothing or uses MonoMod");
                sb.AppendLine();
                continue;
            }

            //Access each MethodBase for each patch
            foreach (var method in methods)
            {
                sb.Append("  Patches ");
                sb.AppendLine(method.FullDescription());
            }

            sb.AppendLine();
        }

        sb.AppendLine("--- END MARSHAL ---\n");

        Plugin.Log.LogInfo(sb);
    }
}
