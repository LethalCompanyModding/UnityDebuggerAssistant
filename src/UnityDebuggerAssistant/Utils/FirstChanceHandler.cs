using System;
using System.Runtime.ExceptionServices;
using System.Text;

namespace UnityDebuggerAssistant.Utils;

public static class FirstChanceHandler
{

    internal static Exception? lastEvent;
    public static void Handle(object? source, FirstChanceExceptionEventArgs e)
    {

        if (lastEvent != null && lastEvent == e.Exception)
            return;

        var item = lastEvent = e.Exception;

        //Process the exception here
        StringBuilder sb = new("\n--- First Chance Handler ---\n");

        sb.Append("Exception Caught: ");
        sb.AppendLine(item.GetType().ToString());

        sb.Append("Target Site: ");
        sb.AppendLine(item.TargetSite.Name);

        sb.Append("Target Assembly: ");
        sb.AppendLine(item.TargetSite.GetType().Assembly.GetName().Name);

        var blames = PatchStorage.GetPatchInformation(item.TargetSite);

        if (blames.Count > 0)
        {
            sb.AppendLine("Potential Blames:");

            foreach (var blame in blames)
            {
                sb.AppendLine(blame.GetName().Name);
            }
        }
        else
        {
            sb.AppendLine("No blames!");
        }

        Plugin.Log.LogError(sb);
    }

    internal static void DebugThrow()
    {
        throw new Exception("Debug throw");
    }
}
