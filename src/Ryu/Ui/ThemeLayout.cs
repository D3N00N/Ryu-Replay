using System.Windows;
using System.Windows.Media;

namespace Ryu.Ui;

/// <summary>Applies the Ryu workspace layouts shown in Appearance settings.</summary>
public static class ThemeLayout
{
    public static void Apply(ResourceDictionary resources, string? layout)
    {
        var profile = layout switch
        {
            "compact" => new Profile(72, 1000, 1280, new(8, 8, 8, 12), new(18, 8, 18, 18), 52, 32, Visibility.Collapsed, Visibility.Collapsed, 7, 10, 10),
            "studio" => new Profile(196, 1360, 1500, new(16, 8, 16, 16), new(24, 4, 24, 16), 84, 48, Visibility.Visible, Visibility.Visible, 9, 8, 12),
            "spotify" => new Profile(236, 1440, 1680, new(16, 12, 16, 18), new(24, 12, 24, 20), 80, 46, Visibility.Visible, Visibility.Visible, 16, 10, 16),
            "glass" => new Profile(220, 1160, 1320, new(14, 10, 14, 16), new(30, 8, 30, 20), 82, 48, Visibility.Visible, Visibility.Visible, 16, 13, 14),
            _ => new Profile(212, 1100, 1280, new(14, 10, 14, 16), new(36, 8, 36, 24), 78, 46, Visibility.Visible, Visibility.Visible, 12, 8, 12),
        };

        resources["SidebarWidth"] = new GridLength(profile.SidebarWidth);
        resources["LayoutWindowWidth"] = (double)profile.WindowWidth;
        resources["LayoutWindowMaxWidth"] = (double)profile.WindowMaxWidth;
        resources["NavMargin"] = profile.NavMargin;
        resources["ContentMargin"] = profile.ContentMargin;
        resources["ContentMaxWidth"] = Math.Max(720, profile.WindowMaxWidth - profile.SidebarWidth - profile.ContentMargin.Left - profile.ContentMargin.Right);
        resources["BrandLogoWidth"] = (double)profile.LogoWidth;
        resources["BrandLogoHeight"] = (double)profile.LogoHeight;
        resources["NavLabelVisibility"] = profile.LabelVisibility;
        resources["NavGroupVisibility"] = profile.GroupVisibility;
        resources["ThemeCardRadius"] = new CornerRadius(profile.CardRadius);
        resources["ThemeControlRadius"] = new CornerRadius(profile.ControlRadius);
        resources["ThemeNavRadius"] = new CornerRadius(profile.NavRadius);
        resources["ThemeNavPadding"] = layout switch
        {
            "compact" => new Thickness(8, 10, 8, 10),
            "spotify" => new Thickness(16, 12, 16, 12),
            _ => new Thickness(12, 10, 12, 10),
        };
        resources["ThemeCardPadding"] = layout switch
        {
            "compact" => new Thickness(14, 12, 14, 12),
            "spotify" => new Thickness(22, 20, 22, 20),
            _ => new Thickness(20, 16, 20, 16),
        };
        resources["ThemeChoicePadding"] = layout == "spotify" ? new Thickness(20, 18, 20, 18) : new Thickness(18, 16, 18, 16);
        resources["ThemeSegmentPadding"] = layout == "spotify" ? new Thickness(16, 9, 16, 9) : new Thickness(14, 8, 14, 8);
        resources["ThemeOutlineThickness"] = layout == "spotify" ? new Thickness(0.8) : new Thickness(1);
        if (layout == "spotify")
        {
            // Spotify-inspired layout is a complete visual preset: the dark canvas, grouped surfaces and green focus
            // color make it immediately distinct from the other palettes, while preserving the same controls/pages.
            resources["Bg"] = Brush("121212");
            resources["NavBg"] = Brush("000000");
            resources["GlassBackground"] = Brush("121212");
            resources["GlassSidebar"] = Brush("000000");
            resources["GlassSheen"] = Brush("00000000");
            resources["Surface"] = Brush("242424");
            resources["CardHover"] = Brush("2A2A2A");
            resources["Stroke"] = Brush("3D3D3D");
            resources["StrokeStrong"] = Brush("555555");
            resources["ControlStroke"] = Brush("747474");
            resources["TextPrimary"] = Brush("FFFFFF");
            resources["TextSecondary"] = Brush("B3B3B3");
            resources["Accent"] = Brush("1ED760");
            resources["AccentHover"] = Brush("3BE477");
            resources["AccentSoft"] = Brush("183522");
            resources["OnAccent"] = Brush("000000");
            resources["Success"] = Brush("1ED760");
            resources["SuccessSoft"] = Brush("183522");
            resources["Idle"] = Brush("A7A7A7");
            resources["IdleSoft"] = Brush("303030");
        }
        if (layout == "glass")
            resources["GlassSheen"] = ThemePalette.GlassSheenBrush;
    }

    static SolidColorBrush Brush(string hex)
    {
        var color = (Color)ColorConverter.ConvertFromString($"#{hex}");
        var brush = new SolidColorBrush(color);
        brush.Freeze();
        return brush;
    }

    sealed record Profile(
        double SidebarWidth, int WindowWidth, int WindowMaxWidth,
        Thickness NavMargin, Thickness ContentMargin,
        int LogoWidth, int LogoHeight,
        Visibility LabelVisibility, Visibility GroupVisibility,
        double CardRadius, double ControlRadius, double NavRadius);
}
