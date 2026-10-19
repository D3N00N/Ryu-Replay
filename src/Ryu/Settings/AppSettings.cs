using Ryu.Engine.Share;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Ryu.Settings;

public enum CaptureMode { Screen, Window, Game }

public enum VideoCodec { Av1, Hevc, H264 }

public enum QualityPreset { Ultra, High, Balanced, Light, Custom }

public enum OutputResolution { Native, P2160, P1440, P1080, P720 }

public enum VoiceSensitivity { Strict, Normal, Easy }

public sealed class AppSettings
{
    public int Version { get; set; } = 1;
    public CaptureSettings Capture { get; set; } = new();
    public VideoSettings Video { get; set; } = new();
    public ReplaySettings Replay { get; set; } = new();
    public AudioSettings Audio { get; set; } = new();
    public HotkeySettings Hotkeys { get; set; } = new();
    public OutputSettings Output { get; set; } = new();
    public GeneralSettings General { get; set; } = new();
    public VoiceSettings Voice { get; set; } = new();
    public ShareSettings Share { get; set; } = new();

    /// <summary>
    /// Windows' own language when Ryu speaks it: the culture or one it falls back to (zh-CN is zh-Hans, es-MX is
    /// es), else the same language in another region (Portugal's Portuguese is Brazil's), never across scripts
    /// (Traditional Chinese is not Simplified); otherwise English.
    /// </summary>
    public static string DefaultLanguage(System.Globalization.CultureInfo uiCulture)
    {
        for (var culture = uiCulture; culture.Name.Length > 0; culture = culture.Parent)
            if (App.Text.IsSupported(culture.Name))
                return culture.Name;
        var regional = App.Text.Languages.FirstOrDefault(l =>
            l.Code.Split('-') is [var language, { Length: 2 }] && language == uiCulture.TwoLetterISOLanguageName);
        return regional?.Code ?? App.Text.Fallback;
    }

    /// <summary>Clamps every value into its supported range and replaces missing sections.</summary>
    public void Normalize()
    {
        Capture ??= new();
        Video ??= new();
        Replay ??= new();
        Audio ??= new();
        Hotkeys ??= new();
        Output ??= new();
        General ??= new();
        Voice ??= new();
        Share ??= new();

        Capture.MonitorId ??= "";
        Capture.Window ??= "";

        Video.Fps = Math.Clamp(Video.Fps, 10, 240);
        Video.CustomCq = Math.Clamp(Video.CustomCq, 1, 63);
        Video.CustomMbps = Math.Clamp(Video.CustomMbps, 2, 300);
        if (string.IsNullOrWhiteSpace(Video.Preset)) Video.Preset = "p5";

        Replay.Seconds = Math.Clamp(Replay.Seconds, 15, 1200);
        Replay.QuickSeconds = Math.Clamp(Replay.QuickSeconds, 5, 120);

        Audio.SystemVolume = Math.Clamp(Audio.SystemVolume, 0, 2);
        Audio.MicVolume = Math.Clamp(Audio.MicVolume, 0, 2);
        Audio.BitrateKbps = Math.Clamp(Audio.BitrateKbps, 64, 320);
        if (string.IsNullOrWhiteSpace(Audio.SystemDevice)) Audio.SystemDevice = "default";
        if (string.IsNullOrWhiteSpace(Audio.MicDevice)) Audio.MicDevice = "default";
        Audio.Sources ??= [];
        Audio.Sources = [.. Audio.Sources.Where(s => s != null).Take(AudioSettings.MaxSources)];
        var audioSourceIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var source in Audio.Sources)
        {
            if (!Guid.TryParseExact(source.Id, "N", out _) || !audioSourceIds.Add(source.Id))
            {
                source.Id = Guid.NewGuid().ToString("N");
                audioSourceIds.Add(source.Id);
            }
            source.DeviceId ??= "default";
            source.Volume = double.IsFinite(source.Volume) ? Math.Clamp(source.Volume, 0, 2) : 1;
            if (source.Track is < 3 or > 5) source.Track = 3;
        }

        Hotkeys.Save ??= "";
        Hotkeys.Quick ??= "";
        Hotkeys.ToggleBuffer ??= "";
        Hotkeys.Record ??= "";
        Hotkeys.PauseRecord ??= App.HotkeyParser.WithShift(Hotkeys.Record);

        // A relative folder would resolve inside the program folder and be lost when the app is replaced.
        if (string.IsNullOrWhiteSpace(Output.Folder) || !Path.IsPathFullyQualified(Output.Folder)) Output.Folder = OutputSettings.DefaultFolder;
        Output.MaxFolderGb = Math.Clamp(Output.MaxFolderGb, 0, 100_000);

        if (!App.Text.IsSupported(General.Language)) General.Language = App.Text.Fallback;
        if (General.Theme is not ("glass" or "dracula" or "nord" or "catppuccin" or "rose-pine" or "gruvbox" or "forest" or "ocean" or "blackout" or "warm")) General.Theme = "glass";
        if (General.Layout is not ("dashboard" or "compact" or "studio" or "spotify" or "glass")) General.Layout = "dashboard";
        if (!Enum.IsDefined(Voice.Sensitivity)) Voice.Sensitivity = VoiceSensitivity.Normal;

        Share.CropPosition = double.IsFinite(Share.CropPosition) ? Math.Clamp(Share.CropPosition, 0, 1) : 0.5;
        Share.Tags ??= "";
        Share.PreviewVolume = double.IsFinite(Share.PreviewVolume) && Share.PreviewVolume is >= 0 and <= 1 ? Share.PreviewVolume : 1;
        Share.VerticalFor = (Share.VerticalFor ?? []).Where(p => Enum.IsDefined(p)).Distinct().ToList();
    }
}

public sealed class CaptureSettings
{
    public CaptureMode Mode { get; set; } = CaptureMode.Screen;
    /// <summary>libobs monitor id; empty means the primary monitor.</summary>
    public string MonitorId { get; set; } = "";
    /// <summary>libobs window string "title:class:exe" used by Window mode.</summary>
    public string Window { get; set; } = "";
    public bool GameFallbackToScreen { get; set; } = true;
    public bool CaptureCursor { get; set; } = true;
}

public sealed class VideoSettings
{
    public VideoCodec Codec { get; set; } = VideoCodec.Av1;
    public int Fps { get; set; } = 60;
    public OutputResolution Resolution { get; set; } = OutputResolution.P1440;
    public QualityPreset Quality { get; set; } = QualityPreset.Ultra;
    public int CustomCq { get; set; } = 22;
    public int CustomMbps { get; set; } = 40;
    public string Preset { get; set; } = "p5";
}

public sealed class ReplaySettings
{
    public bool Enabled { get; set; } = true;
    public int Seconds { get; set; } = 300;
    public bool QuickEnabled { get; set; }
    public int QuickSeconds { get; set; } = 30;
}

public sealed class AudioSettings
{
    public const int MaxSources = 3;
    public bool System { get; set; } = true;
    public string SystemDevice { get; set; } = "default";
    public double SystemVolume { get; set; } = 1.0;
    public bool Mic { get; set; } = true;
    public string MicDevice { get; set; } = "default";
    public double MicVolume { get; set; } = 1.0;
    public bool SeparateTracks { get; set; } = true;
    /// <summary>Window mode: only the captured app's audio. Game mode: only the game's audio.</summary>
    public bool SourceAudioOnly { get; set; }
    public int BitrateKbps { get; set; } = 192;
    public List<AudioSourceSettings> Sources { get; set; } = [];

    /// <summary>Picking a microphone means the user wants it recorded.</summary>
    public void SelectMic(string deviceId)
    {
        MicDevice = deviceId;
        Mic = true;
    }
}

public enum AudioSourceKind { Input, Output }

/// <summary>A user-added WASAPI source. Track is the zero-based libobs mixer index (3 = recording track 4).</summary>
public sealed class AudioSourceSettings : INotifyPropertyChanged
{
    string _id = Guid.NewGuid().ToString("N");
    AudioSourceKind _kind;
    string _deviceId = "default";
    bool _enabled = true;
    double _volume = 1;
    int _track = 3;

    public string Id { get => _id; set => Set(ref _id, value ?? Guid.NewGuid().ToString("N")); }
    public AudioSourceKind Kind { get => _kind; set { if (Set(ref _kind, value)) OnPropertyChanged(nameof(Selection)); } }
    public string DeviceId { get => _deviceId; set { if (Set(ref _deviceId, value ?? "default")) OnPropertyChanged(nameof(Selection)); } }
    public bool Enabled { get => _enabled; set => Set(ref _enabled, value); }
    public double Volume { get => _volume; set => Set(ref _volume, double.IsFinite(value) ? Math.Clamp(value, 0, 2) : 1); }
    public int Track { get => _track; set => Set(ref _track, value is >= 3 and <= 5 ? value : 3); }

    [JsonIgnore]
    public string Selection
    {
        get => $"{Kind}|{DeviceId}";
        set
        {
            if (string.IsNullOrEmpty(value)) return;
            int split = value.IndexOf('|');
            if (split < 1 || !Enum.TryParse(value[..split], out AudioSourceKind kind)) return;
            Kind = kind;
            DeviceId = value[(split + 1)..];
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(name);
        return true;
    }

    void OnPropertyChanged(string? name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public sealed class HotkeySettings
{
    public string Save { get; set; } = "Alt+F10";
    public string Quick { get; set; } = "";
    public string ToggleBuffer { get; set; } = "Alt+Shift+F10";
    public string Record { get; set; } = "Alt+F9";
    /// <summary>Pauses and resumes a full recording. Missing (settings from before it existed): the recording keys plus Shift.</summary>
    public string? PauseRecord { get; set; }
    /// <summary>Takes a screenshot. Alt+F12: free of OBS's F9–F11 and RTSS's Ctrl+F12.</summary>
    public string Screenshot { get; set; } = "Alt+F12";
}

public sealed class OutputSettings
{
    public static readonly string DefaultFolder =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), AppInfo.Name);

    public string Folder { get; set; } = DefaultFolder;
    public bool PerAppFolders { get; set; } = true;
    /// <summary>0 disables the clean-up of old clips.</summary>
    public int MaxFolderGb { get; set; }
}

public sealed class GeneralSettings
{
    public bool StartWithWindows { get; set; } = true;
    public bool SoundOnSave { get; set; } = true;
    public bool ToastOnSave { get; set; } = true;
    public bool OnlyWhenGaming { get; set; }
    public string Language { get; set; } = "ar";
    /// <summary>Window appearance preset chosen in General settings.</summary>
    public string Theme { get; set; } = "glass";
    /// <summary>Interface layout preset (navigation density and workspace spacing).</summary>
    public string Layout { get; set; } = "dashboard";
    /// <summary>Look for a newer Ryu once a day (off: Ryu never connects for it).</summary>
    public bool CheckUpdates { get; set; } = true;
}

/// <summary>Smart Save: save the replay when the user says their recorded phrase.</summary>
public sealed class VoiceSettings
{
    public bool Enabled { get; set; }
    public VoiceSensitivity Sensitivity { get; set; } = VoiceSensitivity.Normal;
    /// <summary>The status-page card that introduces Smart Save was closed.</summary>
    public bool HintDismissed { get; set; }

    /// <summary>
    /// Normal talk scored 0.114 and up against a real user's phrase while their attempts scored 0.05-0.08,
    /// so no threshold goes past 0.100, keeping a margin against saving on ordinary conversation.
    /// </summary>
    public const double MaxThreshold = 0.100;

    /// <summary>
    /// Highest phrase-matching cost that still counts as the phrase. <paramref name="spread"/> is how far the
    /// user's own samples are from each other (<see cref="Engine.Voice.VoicePhrase.Spread"/>): a real voice varies
    /// more than the synthetic voices the floors came from, so the threshold grows with it.
    /// </summary>
    public static double Threshold(VoiceSensitivity s, double spread = 0)
    {
        (double floor, double factor) = s switch
        {
            VoiceSensitivity.Strict => (0.052, 1.5),
            VoiceSensitivity.Easy => (0.066, 2.1),
            _ => (0.060, 1.8),
        };
        return Math.Min(MaxThreshold, Math.Max(floor, factor * spread));
    }
}

/// <summary>The share window's last choices, so the next share starts where the user left off.</summary>
public sealed class ShareSettings
{
    /// <summary>The signed-in X account has Premium (seen on its page while posting); null until known.</summary>
    public bool? XPremium { get; set; }
    /// <summary>Platforms the user turned the 9:16 cut on for (off for every platform at first).</summary>
    public List<SharePlatform> VerticalFor { get; set; } = [];
    /// <summary>Where the 9:16 window sits: 0 = left edge, 1 = right edge.</summary>
    public double CropPosition { get; set; } = 0.5;
    /// <summary>The user's own hashtags, added after the game's tag.</summary>
    public string Tags { get; set; } = "#gaming";
    /// <summary>Loudness of the share panel's preview (0..1); the shared clip keeps its own sound.</summary>
    public double PreviewVolume { get; set; } = 1;
    public bool PreviewMuted { get; set; }

    public ShareSettings Copy()
    {
        var copy = (ShareSettings)MemberwiseClone();
        copy.VerticalFor = [.. VerticalFor];
        return copy;
    }
}
