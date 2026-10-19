namespace Ryu.Tests;

/// <summary>The sidebar uses the transparent high-resolution Ryu mark without a text wordmark.</summary>
public static class WordmarkTests
{
    static string Assets
    {
        get
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Ryu.sln")))
                dir = dir.Parent;
            return Path.Combine(dir!.FullName, "src", "Ryu", "Ui", "Assets");
        }
    }

    [Test]
    public static void The_sidebar_shows_the_Ryu_logo_without_a_separate_name()
    {
        string ui = Path.GetFullPath(Path.Combine(Assets, ".."));
        string xaml = File.ReadAllText(Path.Combine(ui, "MainWindow.xaml"));
        Assert.True(xaml.Contains("Ui/Assets/ryu-logo.png", StringComparison.Ordinal), "Ryu logo asset");
        Assert.True(xaml.Contains("x:Name=\"BrandLogo\"", StringComparison.Ordinal), "visible image control");
        Assert.True(!xaml.Contains("x:Name=\"Wordmark\"", StringComparison.Ordinal), "no separate brand name");
        Assert.True(File.Exists(Path.Combine(Assets, "ryu-logo.png")), "transparent 4K logo is packaged as an app resource");
    }
}
