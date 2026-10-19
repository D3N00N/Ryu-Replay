using System.Windows.Media;
using Ryu.App;
using Ryu.Settings;
using Ryu.Ui;

namespace Ryu.Tests;

public static class AppearanceTests
{
    [Test]
    public static void Ryu_has_one_baby_blue_theme_on_warm_brown()
    {
        Assert.Equal(0x89CFF0u, ThemeColors.Accent, "baby blue accent");
        Assert.Equal(0x2E2726u, ThemeColors.Background, "warm brown-grey window");
    }

    [Test]
    public static void Settings_saved_with_a_theme_color_still_load()
    {
        var s = SettingsStore.Deserialize("""{"general":{"accent":"red","language":"en"}}""");
        Assert.Equal("en", s.General.Language, "the rest is read; the old color choice is ignored");
    }

    [Test]
    public static void Text_on_the_accent_stays_readable()
    {
        Assert.True(ThemeColors.Contrast(ThemeColors.Accent, ThemeColors.OnAccent) >= 4.5,
            $"dark text on baby blue: {ThemeColors.Contrast(ThemeColors.Accent, ThemeColors.OnAccent):F1}:1");
        Assert.True(ThemeColors.Contrast(ThemeColors.Accent, 0xFFFFFF) < 3, "dark text is used on the light accent");
    }

    [Test]
    public static void Hover_is_lighter_and_the_soft_tint_stays_a_tint_of_the_background()
    {
        Assert.True(ThemeColors.Luminance(ThemeColors.Hover) > ThemeColors.Luminance(ThemeColors.Accent), "hover");
        Assert.True(ThemeColors.Contrast(ThemeColors.Soft, ThemeColors.Background) < 1.6, "soft is a tint, not a fill");
        Assert.True(ThemeColors.Contrast(ThemeColors.Soft, 0xF5ECEA) >= 7, "text stays easy to read on it");
    }

    [Test]
    public static void Theme_resources_take_the_one_theme_and_fluent_controls_follow_it()
    {
        var r = new System.Windows.ResourceDictionary();
        ThemeAccent.Apply(r);
        Assert.Equal(Color.FromRgb(0x89, 0xCF, 0xF0), ((SolidColorBrush)r["Accent"]).Color, "accent");
        Assert.Equal(Color.FromRgb(0x22, 0x1C, 0x1B), ((SolidColorBrush)r["OnAccent"]).Color, "dark text on baby blue");
        Assert.Equal(Color.FromRgb(0x89, 0xCF, 0xF0), (Color)r["SystemAccentColor"], "sliders and switches, not the Windows accent");
        Assert.True(r.Contains("AccentHover") && r.Contains("AccentSoft"), "hover and soft");
    }

    [Test]
    public static void Theme_choices_change_interface_shape_as_well_as_colours()
    {
        var r = new System.Windows.ResourceDictionary();
        ThemeAccent.Apply(r);
        ThemePalette.Apply(r, "glass");
        Assert.Equal(14d, ((System.Windows.CornerRadius)r["ThemeCardRadius"]).TopLeft, "soft glass cards");
        Assert.True(r["GlassBackground"] is LinearGradientBrush, "glass background is layered");
        Assert.True(((LinearGradientBrush)r["GlassSheen"]).GradientStops[0].Color.A > 0, "glass has a subtle highlight without backdrop blur");

        ThemePalette.Apply(r, "gruvbox");
        Assert.Equal(4d, ((System.Windows.CornerRadius)r["ThemeCardRadius"]).TopLeft, "sharp retro cards");
        Assert.Equal(1.5, ((System.Windows.Thickness)r["ThemeOutlineThickness"]).Left, "strong retro outlines");
        Assert.True(r["GlassBackground"] is SolidColorBrush, "flat theme background");
        Assert.Equal((byte)0, ((SolidColorBrush)r["GlassSheen"]).Color.A, "non-glass theme has no sheen");

        ThemePalette.Apply(r, "catppuccin");
        Assert.Equal(16d, ((System.Windows.CornerRadius)r["ThemeCardRadius"]).TopLeft, "rounded pastel cards");
        Assert.Equal(Color.FromRgb(0x89, 0xCF, 0xF0), ((SolidColorBrush)r["Accent"]).Color, "baby blue accent stays fixed");
    }

    [Test]
    public static void Progress_ring_uses_the_baby_blue_theme_by_default()
    {
        var fill = (SolidColorBrush)ProgressRing.FillProperty.GetMetadata(typeof(ProgressRing)).DefaultValue;
        Assert.Equal(Color.FromRgb(0x89, 0xCF, 0xF0), fill.Color, "default progress ring fill");
    }

    [Test]
    public static void Tray_icon_uses_the_theme_color()
    {
        Assert.Equal(ThemeColors.Accent, TrayController.ColorFor("active"), "active");
        Assert.Equal(ThemeColors.Accent, TrayController.ColorFor("recording"), "recording");
        Assert.Equal(0x2ED47Au, TrayController.ColorFor("saved"), "saved stays green");
        Assert.Equal(0xFFB020u, TrayController.ColorFor("error"), "error stays amber");
    }

    [Test]
    public static void Appearance_language_sound_and_hotkeys_keep_the_buffer()
    {
        var before = new AppSettings();
        var after = SettingsStore.Deserialize(SettingsStore.Serialize(before));
        after.General.Language = "en";
        after.General.SoundOnSave = false;
        after.General.ToastOnSave = false;
        after.General.StartWithWindows = false;
        after.Hotkeys.Save = "Ctrl+F8";
        Assert.Equal(false, SettingsDiff.NeedsRebuild(before, after), "no restart");
    }

    [Test]
    public static void Capture_and_output_changes_restart_the_buffer()
    {
        var before = new AppSettings();
        AppSettings Changed(Action<AppSettings> change)
        {
            var s = SettingsStore.Deserialize(SettingsStore.Serialize(before));
            change(s);
            return s;
        }
        Assert.Equal(true, SettingsDiff.NeedsRebuild(before, Changed(s => s.Video.Fps = 30)), "fps");
        Assert.Equal(true, SettingsDiff.NeedsRebuild(before, Changed(s => s.Replay.Seconds = 120)), "length");
        Assert.Equal(true, SettingsDiff.NeedsRebuild(before, Changed(s => s.Audio.Mic = false)), "mic");
        Assert.Equal(true, SettingsDiff.NeedsRebuild(before, Changed(s => s.Output.Folder = @"D:\Other")), "folder");
        Assert.Equal(true, SettingsDiff.NeedsRebuild(before, Changed(s => s.General.OnlyWhenGaming = !s.General.OnlyWhenGaming)), "games only");
    }
}
