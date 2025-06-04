using System;

public class RessourcesManager
{
    public static IMinecraftDownloadVersionManager DownloadMinecraft(string version)
    {
        return version switch
        {
            "1.20.1" => new MinecraftDownloadVersionManager_1_20_1(),
            _ => throw new ArgumentException($"version inconnue : {version}")
        };
    }
}
