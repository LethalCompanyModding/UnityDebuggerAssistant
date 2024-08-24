using System;
using System.Text;
using BepInEx;
using System.Diagnostics;
using System.Reflection;
using HarmonyLib;
using System.Runtime.CompilerServices;
using System.IO;
using System.Collections.ObjectModel;
using UnityDebuggerAssistant.Filtering;
using UnityDebuggerAssistant.Utils;

namespace UnityDebuggerAssistant.Processing;

public static class UDAExceptionHandler
{
    public static void Handle(Exception ex)
    {
        /*
        if (targets is null)
        {
#if DEBUG
            UDAPlugin.Log?.LogInfo($"Skipping {ex.GetType()}");
#endif
            return;
        }*/

        int outFrames = 0;
        var targets = ex.TargetSite;
        var trace = new StackTrace(ex, true);
        var assembly = targets?.DeclaringType.Assembly;

        //Filter the main assembly
        if (assembly is not null)
            if (!UDAWhitelist.IsOnExceptionWhitelist(assembly) || UDABlacklist.IsOnExceptionBlacklist(assembly))
            {
#if DEBUG
                UDAPlugin.Log?.LogInfo($"Skipping {assembly.GetName().Name}, failed filter");
#endif
                return;
            }

        static string Tabs(int n)
        {
            return new string(' ', n * 2);
        }

        static void WritePluginInfo(StringBuilder sb, PluginInfo info, int Indent)
        {

            static string GetPluginDirSandboxed(PluginInfo info)
            {
                int start = info.Location.IndexOf($"{Path.DirectorySeparatorChar}plugins{Path.DirectorySeparatorChar}");

                if (start < 0)
                    return "Unknown";

                int length = info.Location.Length - start;
                return new(info.Location.ToCharArray(), start, length);
            }

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
            sb.AppendLine();

        }

        static bool DumpFrame(StringBuilder sb, StackFrame frame, int Indent)
        {

            var method = frame.GetMethod();
            var ILOffset = frame.GetILOffset();
            bool dumpedAnyFrames = false;

            //We have a method
            if (method is not null)
            {
                var InAssembly = method.DeclaringType.Assembly;

                //Only dump frames that are both on the white list and not on the blacklist
                if (UDAWhitelist.IsOnFrameWhitelist(InAssembly) && !UDABlacklist.IsOnFrameBlacklist(InAssembly))
                {
                    //Dump the information about this frame's method

                    dumpedAnyFrames = true;

                    sb.Append(Tabs(Indent));
                    sb.Append("In Assembly: ");
                    sb.AppendLine(InAssembly.GetName().Name);

                    if (frame.GetFileName() is not null)
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

                        if (UDAPluginMarshal.InfoCache.TryGetValue(assembly, out PluginInfo bInfo))
                        {
                            WritePluginInfo(sb, bInfo, Indent);
                        }
                    }

                    var monoModBlames = UDAPatchStorage.GetPatchInformation(method);
                    var harmonyBlames = Harmony.GetPatchInfo(method);

                    if (monoModBlames.Count > 0 || harmonyBlames is not null)
                    {
                        sb.Append(Tabs(Indent));
                        sb.AppendLine("Patched By:");
                    }

                    foreach (var blame in monoModBlames)
                    {
                        DumpPatch(sb, blame, Indent + 1);
                    }

                    if (harmonyBlames is not null)
                    {

                        static void DoHarmonyBlames(StringBuilder sb, ReadOnlyCollection<Patch>? Patches, int Indent)
                        {
                            if (Patches is null)
                                return;

                            foreach (var patch in Patches)
                            {
                                DumpPatch(sb, patch.PatchMethod.DeclaringType.Assembly, Indent);
                            }
                        }

                        DoHarmonyBlames(sb, harmonyBlames.Prefixes, Indent + 1);
                        DoHarmonyBlames(sb, harmonyBlames.Postfixes, Indent + 1);
                        DoHarmonyBlames(sb, harmonyBlames.Finalizers, Indent + 1);
                    }
                }
                else
                {
#if DEBUG
                    UDAPlugin.Log?.LogInfo($"Skipping frame {InAssembly.GetName().Name}, failed filter");
#endif
                }

            }

            return dumpedAnyFrames;
        }

        //Process the exception here
        StringBuilder sb = new("\n\n--- Exception Handler ---\n\n");

        sb.Append("Exception Caught: ");
        sb.AppendLine(ex.GetType().ToString());

        sb.Append("Assembly: ");
        if (assembly is not null)
        {
            sb.AppendLine(assembly.GetName().Name);
        }
        else
        {
            sb.AppendLine("Unknown");

        }

        if (assembly is not null)
            if (UDAPluginMarshal.InfoCache.TryGetValue(assembly, out PluginInfo info))
            {
                WritePluginInfo(sb, info, 1);
            }

        sb.Append("Message: ");
        sb.AppendLine(ex.Message);

        if (ex.Source is not null)
        {
            sb.Append("Source: ");
            sb.AppendLine(ex.Source);
        }

        sb.AppendLine();

        sb.Append(Tabs(1));
        sb.AppendLine("--- Begin Frames ---");
        sb.AppendLine();

        foreach (var item in trace.GetFrames())
        {

            var sbi = new StringBuilder();

            if (DumpFrame(sbi, item, 2))
            {
                sb.Append(Tabs(1));
                sb.Append("--FRAME ");

                outFrames++;
                sb.Append(outFrames);
                sb.AppendLine(":");

                sb.Append(sbi.ToString());
            }

        }

        sb.Append(Tabs(1));
        sb.AppendLine("--- End Frames ---");

        sb.AppendLine("\n--- End Exception Handler ---");

        if (outFrames > 0)
            UDAPlugin.Log?.LogError(sb);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
#if DEBUG
    public static void DebugThrow()
#endif
#if !DEBUG
    internal static void DebugThrow()
#endif
    {
        throw new Exception("Debug throw");
    }
}
