using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Reflection;
using Ryu.Settings;

namespace Ryu.App;

/// <summary>
/// Ryu's logo, used by the window, tray, and app icon.
/// </summary>
public static class AppIconArt
{
    public static readonly int[] Sizes = [16, 20, 24, 32, 40, 48, 64, 128, 256];
    static readonly Bitmap Logo = LoadLogo();

    /// <summary>Draw the Ryu logo inside a centered accent badge.</summary>
    public static void DrawBadge(Graphics g, RectangleF box, Color accent, Color mark)
    {
        float diameter = Math.Min(box.Width, box.Height);
        var badge = new RectangleF(box.X + (box.Width - diameter) / 2, box.Y + (box.Height - diameter) / 2, diameter, diameter);
        using (var fill = new SolidBrush(accent))
            g.FillEllipse(fill, badge);

        DrawLogo(g, badge, Math.Max(2, diameter * 0.12f));
    }

    public static Bitmap Draw(int size, uint accent)
    {
        var bmp = new Bitmap(size, size, PixelFormat.Format32bppArgb);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Color.Transparent);
        float d = size * 0.44f, w = size - 1;
        using (var plate = GdiShapes.Rounded(new RectangleF(0, 0, w, w), d / 2))
        {
            using var fill = new SolidBrush(Opaque(ThemeColors.Background));
            g.FillPath(fill, plate);
        }
        Color color = Opaque(accent);
        float inset = size * 0.055f;
        DrawLogo(g, new RectangleF(inset, inset, size - 2 * inset, size - 2 * inset), size * 0.07f);
        return bmp;
    }

    static Bitmap LoadLogo()
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Ryu.App.ryu-logo.png")
            ?? throw new InvalidOperationException("The Ryu logo resource is missing.");
        using var source = new Bitmap(stream);
        return new Bitmap(source);
    }

    static void DrawLogo(Graphics g, RectangleF bounds, float padding)
    {
        float inset = Math.Max(0, padding);
        var inner = RectangleF.Inflate(bounds, -inset, -inset);
        if (inner.Width <= 0 || inner.Height <= 0) return;
        float scale = Math.Min(inner.Width / Logo.Width, inner.Height / Logo.Height);
        float width = Logo.Width * scale, height = Logo.Height * scale;
        var target = new RectangleF(inner.X + (inner.Width - width) / 2, inner.Y + (inner.Height - height) / 2, width, height);
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.DrawImage(Logo, target);
    }

    /// <summary>An .ico file (PNG frames) with the mark at every size in <see cref="Sizes"/>.</summary>
    public static byte[] Ico(uint accent = ThemeColors.Accent)
    {
        var frames = Sizes.Select(s =>
        {
            using var bmp = Draw(s, accent);
            using var png = new MemoryStream();
            bmp.Save(png, ImageFormat.Png);
            return png.ToArray();
        }).ToList();
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        w.Write((ushort)0);
        w.Write((ushort)1);
        w.Write((ushort)Sizes.Length);
        int offset = 6 + 16 * Sizes.Length;
        for (int i = 0; i < Sizes.Length; i++)
        {
            byte side = (byte)(Sizes[i] >= 256 ? 0 : Sizes[i]);
            w.Write(side);
            w.Write(side);
            w.Write((byte)0);
            w.Write((byte)0);
            w.Write((ushort)1);
            w.Write((ushort)32);
            w.Write((uint)frames[i].Length);
            w.Write((uint)offset);
            offset += frames[i].Length;
        }
        foreach (var f in frames)
            w.Write(f);
        w.Flush();
        return ms.ToArray();
    }

    static Color Opaque(uint rgb) => Color.FromArgb(255, (int)(rgb >> 16) & 0xFF, (int)(rgb >> 8) & 0xFF, (int)rgb & 0xFF);
}
