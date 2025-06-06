using System.Diagnostics;
using System.Windows;

public class RessourcesManager
{
    private static readonly string versionsManifestUrl = @"https://piston-meta.mojang.com/mc/game/version_manifest_v2.json";

    public static async Task<IMinecraftDownloadVersionManager?> DownloadMinecraft(string version)
    {
        HttpUtility client = new HttpUtility();
        string? versionsManifest = await client.GetStringAsync(versionsManifestUrl); //Downloads the versions json manifest
        if (String.IsNullOrEmpty(versionsManifest))
        {
            Debug.WriteLine("Can't get the versions manifest");
            return null;
        }
        string? path = FolderUtility.FolderPathRequest(false, null, null, $"please choose a folder for Minecraft {version}");
        if (String.IsNullOrEmpty(path))
        {
            Debug.WriteLine("Couldn't get the path");
            MessageBox.Show("A path is necessary to download Minecraft !");
            return null;
            
        }
        return version switch //Versions dowload factory
        {
            "1.20.1" => new MinecraftDownloadVersionManager_1_20_1(versionsManifest, path),
            _ => throw new ArgumentException($"version inconnue : {version}")
        };
    }

    
}
