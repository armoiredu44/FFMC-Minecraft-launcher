using Minecraft_launcher;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
public static class MainDownloader
{
    private static readonly string versionsManifestUrl = @"https://piston-meta.mojang.com/mc/game/version_manifest_v2.json";

    public static async Task<(bool, string?)> DownloadMinecraft(string version)
    {
        (bool success, string? versionsManifest) = await getVersionsManifest();
        if (!success || String.IsNullOrEmpty(versionsManifest))
        {
            return (false, "Couldn't fetch versions manifest");
        }
        
        if (!askForMinecraftDirectory(out string? minecraftDirectory))
        {
            return (false, "Minecraft path isn't valid");
        }

        if (!createABunchOfDirectories(version, minecraftDirectory!))
        {
            return (false, "");
        }

        #region versionManifest

        JsonUtility versionsManifestManager = new JsonUtility(versionsManifest);

        if (!versionsManifestManager.GetPropertyPath("id", version, out List<AllTypes> versionPath, false))
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

        (success, string? versionManifest) = await getVersionManifest(foundPropertiesInVersionPath[0].Value.ToString()!, foundPropertiesInVersionPath[1].Value.ToString()!);
        if (!success || String.IsNullOrEmpty(versionManifest))
        {
            return (false, $"Couldn't get the version manifest for {version}");
        }


        JsonUtility versionManifestManager = new JsonUtility(versionManifest);

        if(!versionManifestManager.GetProperties(["assets"], [], out List<AllTypes> assetValueOutput))
        {
            return (false, "Couldn't find the assets value in the version manifest");
        }

        if (String.IsNullOrEmpty(assetValueOutput[0].Value.ToString()))
        {
            return (false, "invalid assetValue");
        }

        int assetValue;
        try
        {
            assetValue = (int)assetValueOutput[0].Value;
        }
        catch (Exception ex)
        {
            return (false, $"Couldn't convert assetsValue to int : {ex}");
        }
        

        
        return (true, "made it to the end");
        #endregion
        /*
        
        #endregion














        return (false, "End");
    }
        */
    }
    private static async Task<(bool, string?)> getVersionsManifest()
    {
        (bool success, AllTypes content) result;
        //string testUrl = "https://ash-speed.hetzner.com/1GB.bin";

        UIManager.MainDownloadProgressBar.MainDownloadProgressBarMaximum = 249111; 

        result = await DownloadHelper.DownloadWithProgressAsync(versionsManifestUrl, "byte[]", //so cleeeeeaaaaan 🌟✨
            progressBytes => UIManager.MainDownloadProgressBar.MainDownloadProgressBarValue = progressBytes,
            progressSpeed => UIManager.MainDownloadTextBlock.MainDownloadTextBlockText = progressSpeed?.ToString("F2") ?? "",
            isCorrupted => Debugger.SendError("File is corrupted"));

        if ((result.content.Value is not byte[] bytes))
        {
            return (false, null);
        }
        
        return (result.success, Encoding.UTF8.GetString(bytes));
    }
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
            @$"{minecraftDirectory}\versions\{version}\{version}-natives-{DateTime.Now.ToString("ddMMMyyyy_HH-mm-ss.fff")}",
            @$"{minecraftDirectory}\runtimes"];


        if (!IoUtilities.Folder.CreateFolders(paths))
        {
            return false;
        }
        return true;
    }

    private static async Task<(bool, string?)> getVersionManifest(string versionManifestUrl, string hash)
    {
        (bool success, AllTypes versionManifest) result;

        result = await DownloadHelper.DownloadWithProgressAsync(versionManifestUrl, "byte[]", //so cleeeeeaaaaan 🌟✨
            progressBytes => UIManager.MainDownloadProgressBar.MainDownloadProgressBarValue = progressBytes,
            progressSpeed => UIManager.MainDownloadTextBlock.MainDownloadTextBlockText = progressSpeed?.ToString("F2") ?? "",
            isCorrupted => Debugger.SendError("File is corrupted"),
            size => UIManager.MainDownloadProgressBar.MainDownloadProgressBarMaximum = size,
            hash);

        if (!(result.versionManifest.Value is byte[] bytes))
        {
            return (false, null);
        }

        return (result.success, Encoding.UTF8.GetString(bytes));
    }

    private static async Task<(bool success, string content)> getClient(string versionManifest, string version, string minecraftDirectory, JsonUtility versionManifestManager)
    {
        if (!versionManifestManager.GetPropertyPath("client", null, out List<AllTypes> clientPath, true)) //kinda not reliable but whatever
        {
            return (false, "couldn't find client");
        }

        if (!versionManifestManager.GetPropertyPath("client_mappings", null, out List<AllTypes> client_mappingsPath, true))
        {
            return (false, "couldn't find client_mappings");
        }

        string[] keys = ["sha1","url","size"];

        if (!versionManifestManager.GetProperties(keys, clientPath, out List<AllTypes> clientOutput))
        {
            Debugger.SendError("Could not find keys for the client property");
            return (false, "couldn't find properties in client");
        }

        if (!versionManifestManager.GetProperties(keys, client_mappingsPath, out List<AllTypes> client_mappingsOutput))
        {
            Debugger.SendError("Could not find keys for the client_mappings property");
            return (false, "couldn't find properties in client_mappins");
        }

        List<AllTypes>[] bothOutputs = [clientOutput, client_mappingsOutput];

        foreach (List<AllTypes> output in bothOutputs)
        {
            foreach (AllTypes keyOutput in output)
            {
                if (String.IsNullOrEmpty(keyOutput.ToString()))
                {
                    return (false, "a property's value wasn't valid during client gathering");
                }
            }
        }

        for (int i = 0; i < 2; i++)
        {
            List<AllTypes> listToProcess = bothOutputs[i];

            (bool success, AllTypes content) result;

            result = await DownloadHelper.DownloadWithProgressAndWriteAsync(listToProcess[2].Value.ToString()!, null, @$"{minecraftDirectory}\versions\{version})",
                bytesProgress => UIManager.MainDownloadProgressBar.MainDownloadProgressBarValue = bytesProgress,
                speedProgress => UIManager.MainDownloadTextBlock.MainDownloadTextBlockText = speedProgress?.ToString("F2") ?? "",
                



        }
    }
}
