public class Debugger : Utilities
{
    static string currentLogDirectory = @$"{appDir}\logs\{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.log";
    public static void CreateLogFileAtStartup()
    {
        IoUtilities.Folder.CreateFolder(@$"{appDir}\logs");
        SendInfo(@$"log file succesfully created at {appDir}\logs\{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.txt");
    }
    public static void SendInfo(string message)
    {
        IoUtilities.File.AppendAllText(currentLogDirectory, $"[{DateTime.Now.ToString("ddMMMyyyy HH:mm:ss.fff")}] [Info] {message}\n");
    }

    public static void SendWarn(string message)
    {
        IoUtilities.File.AppendAllText(currentLogDirectory, $"[{DateTime.Now.ToString("ddMMMyyyy HH:mm:ss.fff")}] [Warn] {message}\n");
    }

    public static void SendError(string message)
    {
        IoUtilities.File.AppendAllText(currentLogDirectory, $"[{DateTime.Now.ToString("ddMMMyyyy HH:mm:ss.fff")}] [Error] {message}\n");
    }

}
