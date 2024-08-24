using BepInEx.Configuration;

internal class PluginConfiguration(ConfigFile Config)
{
    public readonly ConfigEntry<bool> EnableWhitelistPerException =
    Config.Bind(new("Whitelist", "EnableWhiteListPerException"), true,
    new("The per-exception whitelist filters out exceptions that do not originate from common game assemblies or BepinEx plugins.\nDisable this to catch everything. Will increase the log file size."));

    public readonly ConfigEntry<bool> EnableWhitelistPerFrame =
    Config.Bind(new("Whitelist", "EnableWhiteListPerFrame"), false,
    new("The per-frame whitelist will remove frames inside of exceptions that do not originate from common game assemblies.\nEnable this to reduce log file size by cutting out less relevant frames"));

    public readonly ConfigEntry<bool> EnableBlacklistPerException =
    Config.Bind(new("Blacklist", "EnableBlackListPerException"), false,
    new("The per-exception blacklist filters out exceptions that match the given patterns.\nThis is useful if you know a specific assembly throws errors you can ignore"));

    public readonly ConfigEntry<bool> EnableBlacklistPerFrame =
    Config.Bind(new("Blacklist", "EnableBlackListPerFrame"), false,
    new("The per-frame blacklist filters out exceptions that match the given patterns.\nThis is useful if you know a specific assembly throws errors you can ignore"));

    public readonly ConfigEntry<string> ExceptionBlacklist =
    Config.Bind(new("Blacklist", "ExceptionBlacklist"), "UniverseLib.Mono, UnityExplorer.",
    new("A comma-separated list of assembly names to ignore in exception processing\nThis will be matched against the beginning of each assembly name so e.g. `UnityEngine.` will match all UnityEngine assemblies"));

    public readonly ConfigEntry<string> FrameBlacklist =
    Config.Bind(new("Blacklist", "FrameBlacklist"), "UniverseLib.Mono, UnityExplorer.",
    new("A comma-separated list of assembly names to ignore in individual frame processing\nThis will be matched against the beginning of each assembly name so e.g. `UnityEngine.` will match all UnityEngine assemblies"));
}
