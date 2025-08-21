using System.Diagnostics;

public class Utilities
{
    public static string UserDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    public static string AppDir = Environment.CurrentDirectory;
    public static Stopwatch Stopwatch = Stopwatch.StartNew();
}
