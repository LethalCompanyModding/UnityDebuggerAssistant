using System;
using System.Text;
using BepInEx;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using System.Linq;

namespace UnityDebuggerAssistant.Utils;

public static class UDAExceptionHandler
{

    internal static Exception? lastEvent;
    internal readonly static List<Assembly> AssemblyWhiteList = [
        GetAssemblyByName("Assembly-CSharp"),
        GetAssemblyByName("MMHOOK_Assembly-CSharp")
    ];
    public static void Handle(Exception ex)
    {

        static string Tabs(int n)
        {
            return new string(' ', n * 2);
        }

        static void WritePluginInfo(StringBuilder sb, PluginInfo info, int Indent)
        {

            static string GetPluginDirSandboxed(PluginInfo info) => $"plugins{info.Location.Split("plugins")[1]}";

            sb.AppendLine();
            sb.Append(Tabs(Indent));
            sb.AppendLine("Plugin Info");
            sb.Append(Tabs(Indent + 1));
            sb.Append("GUID:     ");
            sb.AppendLine(info.Metadata.GUID);
            sb.Append(Tabs(Indent + 1));
            sb.Append("NAME/VER: ");
            sb.Append(info.Metadata.Name);
            sb.Append("@");
            sb.AppendLine(info.Metadata.Version.ToString());
            sb.Append(Tabs(Indent + 1));
            sb.Append("LOCATION: ");
            sb.AppendLine(GetPluginDirSandboxed(info));

        }

        if (lastEvent != null && lastEvent == ex)
            return;

        lastEvent = ex;

        //Process the exception here
        StringBuilder sb = new("\n\n--- Exception Handler ---\n\n");

        sb.Append("Exception Caught: ");
        sb.AppendLine(ex.GetType().ToString());

        var target = ex.TargetSite;

        if (target is null)
        {
            //Plugin.Log?.LogInfo("targets null");
            return;
        }

        sb.Append("Target: ");
        sb.Append(target.DeclaringType.Name);
        sb.Append('.');
        sb.AppendLine(target.Name);

        var declaringAssembly = target.DeclaringType.Assembly;

        static bool ShouldUseWhitelist()
        {
            if (Plugin.EnableWhitelisting is not null && Plugin.EnableWhitelisting.Value)
                return true;

            return false;
        }

        //Check for whitelist config
        if (ShouldUseWhitelist())
            //Filter for only exceptions thrown by Assembly-Csharp or Plugins
            if (!(AssemblyWhiteList.Contains(declaringAssembly) || PatchStorage.InfoCache.ContainsKey(declaringAssembly)))
            {
                //Plugin.Log?.LogInfo($"Not on whitelist or plugin: {declaringAssembly}");
                return;
            }


        //Plugin.Log?.LogInfo("Stack trace");
        StackTrace stackTrace = new(ex);

        if (stackTrace.FrameCount > 1)
        {
            var caller = stackTrace.GetFrame(1).GetMethod();

            sb.Append("Caller: ");
            sb.Append(caller.DeclaringType.Name);
            sb.Append('.');
            sb.AppendLine(caller.Name);
        }

        sb.Append("Declaring Assembly: ");
        sb.AppendLine(declaringAssembly.GetName().Name);

        if (PatchStorage.InfoCache.TryGetValue(declaringAssembly, out PluginInfo info))
        {
            WritePluginInfo(sb, info, 0);
        }

        var blames = PatchStorage.GetPatchInformation(target);

        var patches = Harmony.GetPatchInfo(target);

        if (patches is not null)
        {
            if (patches.Prefixes is not null)
                foreach (var patch in patches.Prefixes)
                {
                    blames.Add(patch.PatchMethod.GetType().Assembly);
                }

            if (patches.Postfixes is not null)
                foreach (var patch in patches.Postfixes)
                {
                    blames.Add(patch.PatchMethod.GetType().Assembly);
                }

            if (patches.Finalizers is not null)
                foreach (var patch in patches.Finalizers)
                {
                    blames.Add(patch.PatchMethod.GetType().Assembly);
                }
        }

        if (blames.Count > 0)
        {
            sb.AppendLine("\nPotential Blames:\n");

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
            sb.AppendLine("\nNo blames!");
        }

        sb.AppendLine("\n--- End Exception Handler ---");
        Plugin.Log?.LogError(sb);
    }

    static Assembly GetAssemblyByName(string name)
    {
        return AppDomain.CurrentDomain.GetAssemblies().
               SingleOrDefault(assembly => assembly.GetName().Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
    }

    internal static void DebugThrow()
    {
        throw new Exception("Debug throw");
    }
}
