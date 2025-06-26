using System.IO;
public class Debugger : Utilities
{
    static string currentLogDirectory = @$"{appDir}\logs\{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.log";
    static string messageDateFormat = DateTime.Now.ToString("ddMMMyyyy HH:mm:ss.fff");
    public static void CreateLogFileAtStartup()
    {
        FolderUtility.CreateFolder(@$"{appDir}\logs");
        SendInfo(@$"log file succesfully created at {appDir}\logs\{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.txt");
    }
    public static void SendInfo(string message)
    {
        File.AppendAllText(currentLogDirectory, $"[{messageDateFormat}] [Info] {message}\n");
    }

    public static void SendWarn(string message)
    {
        File.AppendAllText(currentLogDirectory, $"[{messageDateFormat}] [Warn] {message}\n");
    }

    public static void SendError(string message)
    {
        File.AppendAllText(currentLogDirectory, $"[{messageDateFormat}] [Error] {message}\n");
    }

}
