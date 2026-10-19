using Ryu.App;
using Ryu.Settings;

namespace Ryu.Tests;

/// <summary>Ryu's logo across the window, tray, and app icon.</summary>
public static class AppIconTests
{
    static uint Rgb(System.Drawing.Color c) => (uint)(c.R << 16 | c.G << 8 | c.B);

    [Test]
    public static void The_Ryu_logo_is_drawn_on_the_warm_plate()
    {
        using var mark = AppIconArt.Draw(64, ThemeColors.Accent);
        Assert.Equal(ThemeColors.Background, Rgb(mark.GetPixel(32, 4)), "the plate: the window's own brown");
        Assert.Equal(0, (int)mark.GetPixel(0, 0).A, "rounded corner");
        Assert.True(CountLogoPixels(mark) > 100, "the supplied metallic Ryu logo is visible");
    }

    static int CountLogoPixels(System.Drawing.Bitmap bitmap)
    {
        int count = 0;
        for (int y = 8; y < 56; y++)
            for (int x = 8; x < 56; x++)
                if (Rgb(bitmap.GetPixel(x, y)) != ThemeColors.Background && bitmap.GetPixel(x, y).A > 200)
                    count++;
        return count;
    }

    /// <summary>
    /// Confirms the tray icon carries the matching accent badge and legible state glyph.
    /// </summary>
    [Test]
    public static void Tray_icons_use_the_brand_badge_and_keep_their_status_glyphs()
    {
        foreach (string kind in new[] { "active", "waiting", "paused", "recording", "error", "saved" })
        {
            using var tray = TrayController.DrawBitmap(kind);
            var badge = tray.GetPixel(16, 8);
            Assert.Equal(TrayController.ColorFor(kind), Rgb(badge), $"tray icon '{kind}' accent badge");
            Assert.True(tray.GetPixel(16, 16).A > 0, $"tray icon '{kind}' has a center mark");
        }
    }

    [Test]
    public static void The_icon_file_has_every_size_windows_asks_for()
    {
        byte[] ico = AppIconArt.Ico();
        Assert.Equal((0, 1), (BitConverter.ToUInt16(ico, 0), BitConverter.ToUInt16(ico, 2)), "an icon file");
        Assert.Equal(AppIconArt.Sizes.Length, (int)BitConverter.ToUInt16(ico, 4), "frames");
        using var icon = new System.Drawing.Icon(new MemoryStream(ico), 32, 32);
        Assert.Equal(32, icon.Width, "Windows reads it");
    }
}
