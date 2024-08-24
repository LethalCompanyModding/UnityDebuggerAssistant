using System.Reflection;

namespace UnityDebuggerAssistant.Filtering;

internal static class UDABlacklist
{
    internal static string[] ExceptionBlackList = [];
    internal static string[] FrameBlackList = [];

    internal static bool IsOnExceptionBlacklist(Assembly assembly)
    {
        //If we're not blacklisting then nothing is on the blacklist
        if (!UDAPlugin.UDASettings.EnableBlacklistPerException.Value)
            return false;

        return IsOnList_Internal(assembly.GetName().Name, in ExceptionBlackList);
    }

    internal static bool IsOnFrameBlacklist(Assembly assembly)
    {
        //If we're not blacklisting then nothing is on the blacklist
        if (!UDAPlugin.UDASettings.EnableBlacklistPerException.Value)
            return false;

        return IsOnList_Internal(assembly.GetName().Name, in FrameBlackList);
    }

    internal static bool IsOnList_Internal(string name, in string[] blacklist)
    {
        foreach (var item in blacklist)
        {
            if (name.StartsWith(item))
                return true;
        }

        return false;
    }

    internal static void UpdateLists_Internal()
    {
        ExceptionBlackList = UDAPlugin.UDASettings.ExceptionBlacklist.Value.Split(',');
        FrameBlackList = UDAPlugin.UDASettings.FrameBlacklist.Value.Split(',');

        for (int i = 0; i < ExceptionBlackList.Length; i++)
            ExceptionBlackList[i] = ExceptionBlackList[i].Trim();

        for (int i = 0; i < FrameBlackList.Length; i++)
            FrameBlackList[i] = FrameBlackList[i].Trim();
    }

}
