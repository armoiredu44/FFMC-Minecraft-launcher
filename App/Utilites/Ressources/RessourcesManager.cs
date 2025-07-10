using Minecraft_launcher;

public static class RessourcesManager
{
    private static readonly string versionsManifestUrl = @"https://piston-meta.mojang.com/mc/game/version_manifest_v2.json";
    static List<string> downloadSpeedHistory = new List<string>();

    public static async Task<(bool, string?)> DownloadMinecraft(string version) //it's a pain, but at least not as much as the utility class
    {
        (bool success, string? versionsManifest) = await getVersionsManifest();
        if (!success || String.IsNullOrEmpty(versionsManifest))
        {
            return (false, "Couldn't fetch versions manifest");
        }
        foreach(string downloadReport in downloadSpeedHistory)
        {
            Debugger.SendInfo(downloadReport);
        }
        return (true, versionsManifest);
        /*
        if (!askForMinecraftDirectory(out string? minecraftDirectory))
        {
            return (false, "Minecraft path isn't valid");
        }

        if (!createABunchOfDirectories(version, minecraftDirectory!))
        {
            return (false, "Error while creating directories");
        }

        #region versionManifest

        JsonUtility versionsManifestManager = new JsonUtility(versionsManifest);

        if (!versionsManifestManager.GetPropertyPath("id", version, out List<AllTypes> versionPath, true))
        {
            return (false, $"Couldn't find the id property for the selected version : {version}");
        }

        string[] versionPathKeys = ["url", "sha1"];

        if (!versionsManifestManager.GetProperties(versionPathKeys, versionPath, out List<AllTypes> foundPropertiesInVersionPath))
        {
            return (false, "Couldn't find the properties");
        }

        int versionPropertiesIndex = -1;
        foreach (string key in versionPathKeys)
        {

            if (String.IsNullOrEmpty(foundPropertiesInVersionPath[++versionPropertiesIndex].Value.ToString()))
            {
                return (false, $"Property {key} is null, cannot continue");
            }
            
        }
        string? versionManifest = null;
        bool corrupted = false;
        
        for (int i = 0; i < 3; i++)
        {
            (success, versionManifest) = await getVersionManifest(foundPropertiesInVersionPath[0].Value.ToString()!);
            if (!success)
            {
                return (false, $"Couldn't get the version manifest for {version}");
            }

            if (!HashChecker.isHashTheSame(versionManifest!, foundPropertiesInVersionPath[1].Value.ToString()!))
            {
                corrupted = true;
                Debugger.SendWarn("Version manifest seems corrupted, trying again...");
            }
            else
            {
                corrupted = false;
                break;
            }
        }
        if (corrupted)
            return (false, "Version manifest is still corrupted after 3 tries, check you connection and try again later");

        JsonUtility versionManifestManager = new JsonUtility(versionManifest!);

        if(!versionManifestManager.GetProperties(["assets"], [], out List<AllTypes> assetValueOutput))
        {
            return (false, "Couldn't find the assets value in the version manifest");
        }

        int assetsValue;
        try
        {
            assetsValue = (int)assetValueOutput[0].Value;
        }
        catch (Exception ex)
        {
            return (false, $"Couldn't convert assetsValue to int : {ex}");
        }
        

        
        
        #endregion














        return (false, "End");
    }
        */
    }
        private static async Task<(bool, string?)> getVersionsManifest()
        {
            (bool success, AllTypes content) result;
            using (HttpUtility client =  new HttpUtility())
            {
                var downloadProgress = new Progress<(long totalReadByte, double downloadSpeed)>(progress =>
                {
                    UIManager.Instance.MainDownloadTextBlock = progress.downloadSpeed.ToString();
                    downloadSpeedHistory.Add(progress.downloadSpeed.ToString());
                    Debugger.SendInfo("report received, updating UI");
                });
                var fileCorrupted = new Progress<bool>(corruption => 
                { if (corruption)
                    {
                        Debugger.SendError("File corrupted");
                    } 
                });
                result = await client.GetAsync("https://hel1-speed.hetzner.com/1GB.bin", downloadProgress, fileCorrupted, "string");
            }
            if (!result.success)
            {
                Debugger.SendError("Couln't fetch the versions manifest");
                return (false, null);
            }
            if (String.IsNullOrEmpty(result.content.Value.ToString()))
            {
                Debugger.SendError("Downloaded value is incorrect");
                return (false, null);
            }
            return (result.success, result.content.Value.ToString());
        }
    /*
    private static bool askForMinecraftDirectory(out string? minecraftDirectory)
    {
        minecraftDirectory = IoUtilities.Folder.FolderPathRequest(false, null, null, "Please choose a folder to install minecraft to.");
        if (minecraftDirectory == null)
        {
            return false;
        }
        minecraftDirectory = minecraftDirectory + @"\.minecraft";
        return true;
    }

    private static bool createABunchOfDirectories(string version, string minecraftDirectory)
    {
        string[] paths = [
            @$"{minecraftDirectory}\assets\indexes",
            @$"{minecraftDirectory}\assets\objects",
            @$"{minecraftDirectory}\libraries",
            @$"{{{minecraftDirectory}}}\versions\{version}\{version}-natives-{DateTime.Now.ToString("ddMMMyyyy_HH:mm:ss.fff")}",
            @$"{{{minecraftDirectory}}}\runtimes"];


        if (!IoUtilities.Folder.CreateFolders(paths))
        {
            return false;
        }
        return true;
    }

    private static async Task<(bool, string?)> getVersionManifest(string versionManifestUrl)
    {
        (bool success, string? versionManifest) result;

        using (HttpUtility client = new HttpUtility())
        {
            result = await client.GetStringAsync(versionManifestUrl);
        }
        if (!result.success)
        {
            Debugger.SendError("Couln't fetch the versions manifest");
            return (false, null);
        }
        return result;
    }

    private static async Task<bool> getClient(string versionManifest, JsonUtility versionManifestManager)
    {
        if (!versionManifestManager.GetPropertyPath("client", null, out List<AllTypes> clientPath, true))
        {
            return false;
        }

        if (!versionManifestManager.GetPropertyPath("client_mappings", null, out List<AllTypes> client_mappingsPath, true))
        {
            return false;
        }

        string[] keys = ["sha1", "size", "url"];

        if (!versionManifestManager.GetProperties(keys, clientPath, out List<AllTypes> clientOutput))
        {
            Debugger.SendError("Could not find keys for the client property");
            return false;
        }

        if (!versionManifestManager.GetProperties(keys, client_mappingsPath, out List<AllTypes> client_mappingsOutput))
        {
            Debugger.SendError("Could not find keys for the client_mappings property");
            return false;
        }

        List<List<AllTypes>> clientsOutput = [clientOutput, client_mappingsOutput];

        int i = -1;

        using (HttpUtility httpClient = new HttpUtility())
        {
            foreach (var client in clientsOutput)
            {
                foreach (string key in keys)
                {
                    if (String.IsNullOrEmpty(client[++i].Value.ToString()))
                    {
                        Debugger.SendError($"Property {key} wasn't found for client property, cannot continue");
                        return false;
                    }
                }
                (bool success, string? content) output = await httpClient.GetStringAsync(client[2].Value.ToString()!);
                if (!output.success || String.IsNullOrEmpty(output.content))
                {
                    Debugger.SendError("Couldn't fetch client.jar or client.txt");
                    return false;
                }
                
                for (int j = 0; j < 3; j++)
                {
                    if (!HashChecker.isHashTheSame())
                }

            }
        }

        
     */   
}
