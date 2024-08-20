using System;
using System.Runtime.ExceptionServices;
using System.Text;
using BepInEx;

namespace UnityDebuggerAssistant.Utils;

public static class ExceptionHandler
{

    internal static Exception? lastEvent;
    public static void Handle(Exception ex)
    {

        if (lastEvent != null && lastEvent == ex)
            return;

        lastEvent = ex;

        //Process the exception here
        StringBuilder sb = new("\n\n--- Exception Handler ---\n\n");

        sb.Append("Exception Caught: ");
        sb.AppendLine(ex.GetType().ToString());

        sb.Append("Target Site: ");
        sb.AppendLine(ex.TargetSite.Name);

        var declaringAssembly = ex.TargetSite.DeclaringType.Assembly;

        sb.Append("Declaring Assembly: ");
        sb.AppendLine(declaringAssembly.GetName().Name);

        if (PatchStorage.InfoCache.TryGetValue(declaringAssembly, out PluginInfo info))
        {
            sb.AppendLine("Plugin Info");
            sb.Append("  ");
            sb.AppendLine(info.Metadata.GUID);
            sb.Append("  ");
            sb.Append(info.Metadata.Name);
            sb.Append("@");
            sb.AppendLine(info.Metadata.Version.ToString());
        }

        var blames = PatchStorage.GetPatchInformation(ex.TargetSite);

        if (blames.Count > 0)
        {
            sb.AppendLine("Potential Blames:");

            foreach (var blame in blames)
            {
                sb.Append("  ");
                sb.AppendLine(blame.GetName().Name);
            }
        }
        else
        {
            sb.AppendLine("No blames!");
        }

        sb.AppendLine("\n--- End Exception Handler ---");
        Plugin.Log.LogError(sb);
    }

    internal static void DebugThrow()
    {
        throw new Exception("Debug throw");
    }
}
