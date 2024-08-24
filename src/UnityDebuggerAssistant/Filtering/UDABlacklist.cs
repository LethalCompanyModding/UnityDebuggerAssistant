using System.Reflection;

namespace UnityDebuggerAssistant.Filtering;

internal static class UDABlacklist
{
    internal static string[] ExceptionBlackList = [];
    internal static string[] FrameBlackList = [];

    internal static bool ExceptionBlacklist(Assembly assembly)
    {
        return false;
    }

    internal static bool FrameBlacklist(Assembly assembly)
    {
        return false;
    }
}
