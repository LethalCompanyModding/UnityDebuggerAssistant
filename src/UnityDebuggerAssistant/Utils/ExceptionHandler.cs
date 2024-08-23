using System;
using System.Text;
using BepInEx;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine.Rendering.HighDefinition;

namespace UnityDebuggerAssistant.Utils;

public static class UDAExceptionHandler
{
    internal readonly static List<Assembly> AssemblyWhiteList = [
        GetAssemblyByName("Assembly-CSharp"),
        GetAssemblyByName("MMHOOK_Assembly-CSharp")
    ];
    public static void Handle(Exception ex)
    {

        if (ex.TargetSite is null)
        {
#if DEBUG
            Plugin.Log?.LogInfo($"Throwing away a bad exception {ex.GetType()}");
#endif
            return;
        }

        var trace = new StackTrace(ex, true);
        int outFrames = 0;

        static string Tabs(int n)
        {
            return new string(' ', n * 2);
        }

        static bool ShouldUseWhitelist()
        {
            if (Plugin.EnableWhitelisting is not null && Plugin.EnableWhitelisting.Value)
                return true;

            return false;
        }

        static bool WhiteListContains(Assembly assembly)
        {
            return PatchStorage.InfoCache.ContainsKey(assembly) || AssemblyWhiteList.Contains(assembly);
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

        static bool DumpFrame(StringBuilder sb, StackFrame frame, int Indent)
        {

            var method = frame.GetMethod();
            var ILOffset = frame.GetILOffset();
            bool dumpedAnyFrames = false;

            //We have a method
            if (frame.GetMethod() is not null)
            {
                var InAssembly = frame.GetMethod().DeclaringType.Assembly;

                if (!ShouldUseWhitelist() || (ShouldUseWhitelist() && WhiteListContains(InAssembly)))
                {
                    //Dump the information about this frame's method
                    //and the user wants to see this frame so dump it

                    dumpedAnyFrames = true;

                    sb.Append(Tabs(Indent));
                    sb.Append("In Assembly: ");
                    sb.AppendLine(InAssembly.GetName().Name);

                    if (frame.HasSource())
                    {
                        sb.Append(Tabs(Indent));
                        sb.Append("Source: ");
                        sb.Append(frame.GetFileName());
                        sb.Append(':');
                        sb.Append(frame.GetFileLineNumber());
                        sb.Append(',');
                        sb.AppendLine(frame.GetFileColumnNumber().ToString());
                    }

                    sb.Append(Tabs(Indent));
                    sb.Append("Target Method: ");
                    sb.Append(method.DeclaringType.Name);
                    sb.Append('.');
                    sb.AppendLine(method.Name);

                    sb.AppendLine();

                    static void DumpPatch(StringBuilder sb, Assembly assembly, int Indent)
                    {
                        sb.Append(Tabs(Indent));
                        sb.AppendLine(assembly.GetName().Name);

                        if (PatchStorage.InfoCache.TryGetValue(assembly, out PluginInfo bInfo))
                        {
                            WritePluginInfo(sb, bInfo, Indent);
                        }
                    }

                    var monoModBlames = PatchStorage.GetPatchInformation(method);
                    var harmonyBlames = Harmony.GetPatchInfo(method);

                    foreach (var blame in monoModBlames)
                    {
                        DumpPatch(sb, blame, Indent + 1);
                    }

                    if (harmonyBlames is not null)
                    {
                        sb.AppendLine("Patched By:");

                        if (harmonyBlames.Prefixes is not null)
                            foreach (var patch in harmonyBlames.Prefixes)
                            {
                                DumpPatch(sb, patch.PatchMethod.GetType().Assembly, Indent + 1);
                            }

                        if (harmonyBlames.Postfixes is not null)
                            foreach (var patch in harmonyBlames.Postfixes)
                            {
                                DumpPatch(sb, patch.PatchMethod.GetType().Assembly, Indent + 1);
                            }

                        if (harmonyBlames.Finalizers is not null)
                            foreach (var patch in harmonyBlames.Finalizers)
                            {
                                DumpPatch(sb, patch.PatchMethod.GetType().Assembly, Indent + 1);
                            }
                    }
                }
                else
                {
#if DEBUG
                    Plugin.Log?.LogInfo($"Skipping frame, not on whitelist {InAssembly.GetName().Name}");
#endif
                }

            }

            return dumpedAnyFrames;
        }

        //Process the exception here
        StringBuilder sb = new("\n\n--- Exception Handler ---\n\n");

        sb.Append("Exception Caught: ");
        sb.AppendLine(ex.GetType().ToString());

        sb.Append("Message: ");
        sb.AppendLine(ex.Message);

        if (PatchStorage.InfoCache.TryGetValue(ex.TargetSite.DeclaringType.Assembly, out PluginInfo info))
        {
            WritePluginInfo(sb, info, 1);
        }

        sb.AppendLine();

        sb.Append(Tabs(1));
        sb.AppendLine("--- Begin Frames ---");
        sb.AppendLine();

        foreach (var item in trace.GetFrames())
        {
            if (DumpFrame(sb, item, 2))
            {
                outFrames++;
            }
        }

        sb.Append(Tabs(1));
        sb.AppendLine("--- End Frames ---");

        sb.AppendLine("\n--- End Exception Handler ---");

        if (outFrames > 0)
            Plugin.Log?.LogError(sb);
    }

    static Assembly GetAssemblyByName(string name)
    {
        return AppDomain.CurrentDomain.GetAssemblies().
               SingleOrDefault(assembly => assembly.GetName().Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void DebugThrow()
    {
        throw new Exception("Debug throw");
    }
}
