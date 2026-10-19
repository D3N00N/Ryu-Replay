using Ryu.Settings;

namespace Ryu.Tests;

public static class AudioSourceTests
{
    [Test]
    public static void Added_source_settings_round_trip()
    {
        var before = new AppSettings();
        var source = new AudioSourceSettings
        {
            Kind = AudioSourceKind.Output,
            DeviceId = "speaker-device",
            Enabled = true,
            Volume = 0.75,
            Track = 5,
        };
        before.Audio.Sources.Add(source);

        var after = SettingsStore.Deserialize(SettingsStore.Serialize(before));
        var saved = after.Audio.Sources.Single();
        Assert.Equal(source.Id, saved.Id, "stable meter id");
        Assert.Equal(AudioSourceKind.Output, saved.Kind, "device type");
        Assert.Equal("speaker-device", saved.DeviceId, "device");
        Assert.Equal(true, saved.Enabled, "enabled");
        Assert.Near(0.75, saved.Volume, 0.001, "volume");
        Assert.Equal(5, saved.Track, "track 6 mixer");
    }

    [Test]
    public static void Selecting_a_device_keeps_its_kind_and_device_id()
    {
        var source = new AudioSourceSettings();
        source.Selection = "Output|speaker-device";

        Assert.Equal(AudioSourceKind.Output, source.Kind, "output device");
        Assert.Equal("speaker-device", source.DeviceId, "device id");
        Assert.Equal("Output|speaker-device", source.Selection, "selection value");
    }

    [Test]
    public static void Changing_an_added_source_rebuilds_the_capture_pipeline()
    {
        var before = new AppSettings();
        var after = SettingsStore.Deserialize(SettingsStore.Serialize(before));
        after.Audio.Sources.Add(new AudioSourceSettings { DeviceId = "input-device" });

        Assert.True(SettingsDiff.NeedsRebuild(before, after), "source configuration changes capture");
    }

    [Test]
    public static void Settings_keep_at_most_three_unique_meter_sources()
    {
        var settings = new AppSettings();
        settings.Audio.Sources = Enumerable.Range(0, 5).Select(_ => new AudioSourceSettings()).ToList();
        settings.Normalize();

        Assert.Equal(AudioSettings.MaxSources, settings.Audio.Sources.Count, "supported source count");
        Assert.Equal(AudioSettings.MaxSources, settings.Audio.Sources.Select(s => s.Id).Distinct().Count(), "unique meters");
    }
}
