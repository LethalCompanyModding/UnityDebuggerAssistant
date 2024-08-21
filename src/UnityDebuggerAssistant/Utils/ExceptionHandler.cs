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

        static string Tabs(int n)
        {
            return new string(' ', n * 2);
        }

        static void WritePluginInfo(StringBuilder sb, PluginInfo info, int Indent)
        {
            sb.Append(Tabs(Indent));
            sb.AppendLine("Plugin Info");
            sb.Append(Tabs(Indent + 1));
            sb.AppendLine(info.Metadata.GUID);
            sb.Append(Tabs(Indent + 1));
            sb.Append(info.Metadata.Name);
            sb.Append("@");
            sb.AppendLine(info.Metadata.Version.ToString());
        }

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
            WritePluginInfo(sb, info, 0);
        }

        var blames = PatchStorage.GetPatchInformation(ex.TargetSite);

        if (blames.Count > 0)
        {
            sb.AppendLine("Potential Blames:\n");

            foreach (var blame in blames)
            {
                sb.Append(Tabs(1));
                sb.AppendLine(blame.GetName().Name);

                if (PatchStorage.InfoCache.TryGetValue(blame, out PluginInfo bInfo))
                {
                    WritePluginInfo(sb, bInfo, 1);
                }
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
