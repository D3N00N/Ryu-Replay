using System.Windows;
using System.Windows.Media;

namespace Ryu.Ui;

/// <summary>Applies one complete interface palette. Dynamic resource references update the open settings window live.</summary>
public static class ThemePalette
{
    public static Brush GlassSheenBrush => Sheen;

    public static void Apply(ResourceDictionary resources, string? theme)
    {
        theme ??= "glass";
        var palette = theme switch
        {
            "dracula" => Dracula,
            "nord" => Nord,
            "catppuccin" => Catppuccin,
            "rose-pine" => RosePine,
            "gruvbox" => Gruvbox,
            "forest" => Forest,
            "ocean" => Ocean,
            "blackout" => Blackout,
            "warm" => Warm,
            _ => Glass,
        };

        foreach (var (key, hex) in palette.Colors)
            resources[key] = Brush(hex);
        var style = StyleFor(theme);
        resources["GlassBackground"] = style.WindowGradient ? Gradient(palette.Main) : Brush(palette.Main[^1]);
        resources["GlassSidebar"] = style.SidebarGradient ? Gradient(palette.Sidebar) : Brush(palette.Sidebar[^1]);
        resources["GlassSheen"] = style.Sheen ? Sheen : Transparent;
        resources["Surface"] = style.SurfaceGradient ? Gradient([palette.Colors["Surface"], palette.Colors["CardHover"]]) : Brush(palette.Colors["Surface"]);
        resources["ThemeCardRadius"] = new CornerRadius(style.CardRadius);
        resources["ThemeControlRadius"] = new CornerRadius(style.ControlRadius);
        resources["ThemeNavRadius"] = new CornerRadius(style.NavRadius);
        resources["ThemeChoiceRadius"] = new CornerRadius(style.ChoiceRadius);
        resources["ThemeCardPadding"] = style.CardPadding;
        resources["ThemeChoicePadding"] = style.ChoicePadding;
        resources["ThemeNavPadding"] = style.NavPadding;
        resources["ThemeSegmentPadding"] = style.SegmentPadding;
        resources["ThemeOutlineThickness"] = new Thickness(style.OutlineThickness);
    }

    static StyleProfile StyleFor(string theme) => theme switch
    {
        // Independent visual profiles: the same theme reshapes cards, controls, navigation and surface treatment.
        "glass" => new(14, 12, 12, 14, new(22, 18, 22, 18), new(20, 17, 20, 17), new(14, 11, 14, 11), new(16, 9, 16, 9), 1, true, true, true, true),
        "dracula" => new(10, 7, 7, 10, new(18, 14, 18, 14), new(16, 13, 16, 13), new(12, 9, 12, 9), new(13, 8, 13, 8), 1, false, false, false, false),
        "nord" => new(5, 5, 4, 7, new(16, 12, 16, 12), new(14, 12, 14, 12), new(10, 8, 10, 8), new(12, 7, 12, 7), 1, false, false, false, false),
        "catppuccin" => new(16, 12, 14, 16, new(22, 18, 22, 18), new(20, 16, 20, 16), new(14, 11, 14, 11), new(16, 9, 16, 9), 1, true, true, true, false),
        "rose-pine" => new(18, 12, 12, 18, new(22, 17, 22, 17), new(20, 16, 20, 16), new(14, 11, 14, 11), new(16, 9, 16, 9), 1, false, true, true, false),
        "gruvbox" => new(4, 4, 4, 4, new(16, 12, 16, 12), new(14, 12, 14, 12), new(10, 8, 10, 8), new(12, 7, 12, 7), 1.5, false, false, false, false),
        "forest" => new(8, 6, 6, 8, new(18, 14, 18, 14), new(16, 13, 16, 13), new(12, 9, 12, 9), new(14, 8, 14, 8), 1.5, false, false, false, false),
        "ocean" => new(15, 11, 12, 16, new(20, 16, 20, 16), new(18, 15, 18, 15), new(13, 10, 13, 10), new(15, 9, 15, 9), 1, true, true, true, true),
        "blackout" => new(6, 5, 3, 5, new(18, 14, 18, 14), new(16, 13, 16, 13), new(10, 8, 10, 8), new(12, 7, 12, 7), 2, false, false, false, false),
        _ => new(12, 8, 8, 12, new(20, 16, 20, 16), new(18, 14, 18, 14), new(12, 10, 12, 10), new(14, 8, 14, 8), 1, false, false, false, false),
    };

    sealed record StyleProfile(
        double CardRadius, double ControlRadius, double NavRadius, double ChoiceRadius,
        Thickness CardPadding, Thickness ChoicePadding, Thickness NavPadding, Thickness SegmentPadding,
        double OutlineThickness, bool WindowGradient, bool SidebarGradient, bool SurfaceGradient, bool Sheen);

    static Palette Create(
        string[] main, string[] sidebar,
        string bg, string nav, string surface, string hover, string stroke, string strokeStrong,
        string controlStroke, string text, string muted, string accentSoft, string field, string pointer)
    {
        var colors = new Dictionary<string, string>
        {
            ["Bg"] = bg, ["NavBg"] = nav, ["Surface"] = surface, ["CardHover"] = hover,
            ["Stroke"] = stroke, ["StrokeStrong"] = strokeStrong, ["ControlStroke"] = controlStroke,
            ["TextPrimary"] = text, ["TextSecondary"] = muted,
            ["FieldFill"] = field, ["ComboBoxBackgroundPointerOver"] = pointer, ["ComboBoxBackgroundFocused"] = field,
            ["ComboBoxBorderBrushFocused"] = strokeStrong, ["TextControlBackgroundPointerOver"] = pointer,
            ["TextControlBackgroundFocused"] = field, ["ComboBoxBackgroundDisabled"] = surface,
            ["ComboBoxBorderBrushDisabled"] = stroke, ["TextControlBackgroundDisabled"] = surface,
            ["TextControlBorderBrushDisabled"] = stroke, ["ComboBoxDropDownBackground"] = surface,
            ["ComboBoxDropDownBorderBrush"] = stroke, ["ComboBoxItemBackgroundSelected"] = hover,
            ["ContextMenuBackground"] = surface, ["ContextMenuBorderBrush"] = stroke,
            ["FlyoutBackground"] = surface, ["FlyoutBorderBrush"] = stroke,
            ["ToolTipBackground"] = surface, ["ToolTipBorderBrush"] = stroke,
        };
        return new Palette(colors, main, sidebar);
    }

    static SolidColorBrush Brush(string hex)
    {
        uint rgb = Convert.ToUInt32(hex, 16);
        var brush = new SolidColorBrush(Color.FromRgb((byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb));
        brush.Freeze();
        return brush;
    }

    static LinearGradientBrush Gradient(string[] colors)
    {
        var brush = new LinearGradientBrush { StartPoint = new Point(0, 0), EndPoint = new Point(0, 1) };
        for (int i = 0; i < colors.Length; i++)
            brush.GradientStops.Add(new GradientStop(Brush(colors[i]).Color, i / (double)(colors.Length - 1)));
        return brush;
    }

    static readonly LinearGradientBrush Sheen = CreateSheen();
    static LinearGradientBrush CreateSheen()
    {
        var brush = new LinearGradientBrush { StartPoint = new Point(0, 0), EndPoint = new Point(0, 1) };
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(24, 255, 255, 255), 0));
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(6, 255, 255, 255), 0.14));
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(0, 255, 255, 255), 0.42));
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 1));
        brush.Freeze();
        return brush;
    }
    static readonly SolidColorBrush Transparent = new(Colors.Transparent);

    sealed record Palette(Dictionary<string, string> Colors, string[] Main, string[] Sidebar);

    // Palette studies based on the independently distributed themes commonly featured in Spicetify Marketplace.
    // These are original Ryu palettes; the app keeps its own baby-blue accent and does not copy theme assets.
    static readonly Palette Dracula = Create(
        ["343746", "2D3040", "242633"], ["2D3040", "262936", "20222E"],
        "292B3A", "232532", "37394A", "424456", "515366", "707287", "9A9CB0",
        "F4F3F8", "C5C4D0", "36384A", "292B3A", "383A4C");

    static readonly Palette Nord = Create(
        ["3B4657", "323D4D", "283341"], ["303B4A", "283341", "222C38"],
        "2E3948", "26313E", "3A4656", "465364", "566274", "778598", "A2AFBF",
        "ECEFF4", "C4CBD5", "384657", "2A3543", "3A4655");

    static readonly Palette Catppuccin = Create(
        ["34364B", "2C2E42", "242638"], ["2D2F43", "26283A", "202233"],
        "292B3F", "232538", "36384D", "42445A", "51536A", "74768D", "A3A5BA",
        "E6E8F5", "BFC3D8", "35384F", "292B3F", "393B50");

    static readonly Palette RosePine = Create(
        ["383447", "302D3F", "272536"], ["302D3F", "292637", "232131"],
        "2D2A3C", "272436", "393548", "454153", "555063", "777286", "A5A0B1",
        "EAE7F2", "C8C2D6", "383448", "2D2A3C", "3D394B");

    static readonly Palette Gruvbox = Create(
        ["403B35", "36322D", "2C2925"], ["37332E", "2D2A26", "252320"],
        "332F2B", "2B2824", "403B35", "4B453E", "5A534B", "7D7469", "A79D8F",
        "EBDBB2", "C6BDA5", "443E35", "332F2B", "423D36");

    static readonly Palette Forest = Create(
        ["354A3C", "2A3C31", "1C2D23"], ["2A3D31", "1F3026", "17271E"],
        "293B30", "203128", "36483B", "42574A", "52675A", "74897A", "92A397",
        "F3F7F3", "BECCC2", "3B5142", "24352B", "34483A");

    static readonly Palette Ocean = Create(
        ["35535D", "2A444E", "1C323B"], ["2D4751", "223942", "182C34"],
        "2A414A", "22363E", "354B54", "405A64", "506A73", "708992", "91A6AC",
        "F3F8FA", "BDCDD2", "354E59", "223840", "304A54");

    static readonly Palette Glass = Create(
        ["405660", "354A54", "293B44"], ["394F59", "2D414A", "23353D"],
        "2D404A", "253740", "3B4D57", "465963", "51646E", "6A808A", "8499A1",
        "F4F8FA", "BDC9CD", "3C515A", "263840", "324751");

    static readonly Palette Blackout = Create(
        ["22272C", "191D22", "111418"], ["1B2025", "14181C", "0E1114"],
        "171B20", "121619", "23282D", "2D3339", "3A4148", "5B646D", "87919B",
        "F2F5F7", "B7C0C7", "252B30", "171B20", "252B30");

    static readonly Palette Warm = Create(
        ["2E2726", "302927", "2E2726"], ["282120", "282120", "282120"],
        "2E2726", "282120", "3A3130", "443A39", "4E4241", "625453", "8A7875",
        "F5ECEA", "BBAAA7", "3A3D40", "2A2322", "302827");
}
