using System.Reflection;
using System.Text.RegularExpressions;

namespace Ryu;

/// <summary>Every name the app registers with Windows, in one place so a rename is a single edit.</summary>
public static class AppInfo
{
    /// <summary>Stable internal id used for settings, IPC, update channels, and the existing install folder.</summary>
    public const string Name = "Ryu";
    /// <summary>The executable and the user-facing Windows application name.</summary>
    public const string ExeName = "Ryu Replay";
    public const string EngineMutex = @"Local\Ryu.Engine";
    public const string SettingsMutex = @"Local\Ryu.Settings";
    public const string SettingsShowEvent = @"Local\Ryu.Settings.Show";
    public const string PipeName = "Ryu";
    public const string RunValueName = "Ryu";

    /// <summary>The full version, a prerelease part included ("1.0.0-beta.1"), without the build details after "+".</summary>
    public static string Version { get; } =
        (typeof(AppInfo).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "0.0.0").Split('+')[0];

    /// <summary>A version as people read it: "1.0.0-beta.3" is "1.0.0 beta 3" in their language; any other stays as it is.</summary>
    public static string Display(string version) => Regex.Match(version, @"^(\d+\.\d+\.\d+)-beta\.(\d+)$") is { Success: true } m
        ? App.Text.Format("version.beta", m.Groups[1].Value, m.Groups[2].Value)
        : version;
}
