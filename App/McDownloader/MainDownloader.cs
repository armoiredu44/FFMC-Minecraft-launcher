using Minecraft_launcher;
using System.Text;
using System.Windows.Navigation;
public static class MainDownloader
{
    private static readonly string versionsManifestUrl = @"https://piston-meta.mojang.com/mc/game/version_manifest_v2.json";
    private static readonly string assetsUrl = @"https://resources.download.minecraft.net/";

    public static async Task<(bool, string?)> DownloadMinecraft(string version) //PLEASE MAKE THE CODE MORE EXPLICIT THIS IS A PAIN
    {
        string assetValue;
        (bool success, string? versionsManifest) = await getVersionsManifest(); //downloads the versions manifest
        if (!success || String.IsNullOrEmpty(versionsManifest))
        {
            return (false, "Couldn't fetch versions manifest");
        }
        
        if (!askForMinecraftDirectory(out string? minecraftDirectory)) //if the out variable is null then it ouputs false, so every use of it shouldn't be null
        {
            return (false, "Minecraft path isn't valid");
        }

        if (!createABunchOfDirectories(version, minecraftDirectory!)) //self-explanatory
        {
            return (false, "");
        }

        #region versionManifest

        JsonUtility versionsManifestManager = new JsonUtility(versionsManifest);

        if (!versionsManifestManager.GetPropertyPath("id", version, out List<AllTypes> versionPath, false)) //finds the path of the directory the id property is in
        {
            return (false, $"Couldn't find the id property for the selected version : {version}");
        }

        string[] versionPathKeys = ["url", "sha1"];

        if (!versionsManifestManager.GetProperties(versionPathKeys, versionPath, out List<AllTypes> foundPropertiesInVersionPath)) //now retrieves other properties inside that path
        {
            return (false, "Couldn't find the properties");
        }

        int versionPropertiesIndex = -1;
        foreach (string key in versionPathKeys) //checks if the values are valid
        {

            if (String.IsNullOrEmpty(foundPropertiesInVersionPath[++versionPropertiesIndex].Value.ToString()))
            {
                return (false, $"Property {key} is null, cannot continue");
            }
            
        }

        (success, string? versionManifest) = await getVersionManifest(foundPropertiesInVersionPath[0].Value.ToString()!, foundPropertiesInVersionPath[1].Value.ToString()!); //downloads the version-specific manifest from the values found in the versions manifest
        if (!success || String.IsNullOrEmpty(versionManifest))
        {
            return (false, $"Couldn't get the version manifest for {version}");
        }


        JsonUtility versionManifestManager = new JsonUtility(versionManifest);

        #endregion versionManifest

        #region client

        (success, string outputMessage) = await getClient(versionManifest, version, minecraftDirectory!, versionManifestManager); //downloads the client files
         if (!success)
        {
            return (false, outputMessage);
        }
        #endregion client

        #region assets

        if (!versionManifestManager.GetProperties(["assets"], [], out List<AllTypes> assetValueOutput)) //retrieve the value of the assets property, because it should the name of the index file
        {
            return (false, "Couldn't find the assets value in the version manifest");
        }

        if (String.IsNullOrEmpty(assetValueOutput[0].Value.ToString()))
        {
            return (false, "invalid assetValue");
        }

        try //this try bloc is useless
        {
            assetValue = assetValueOutput[0].Value.ToString()!;
            //Debugger.SendInfo(assetValue);
        }
        catch (Exception ex)// fix this soon, ToString() can't generate exceptions
        {
            return (false, $"Couldn't convert assetsValue to int : {ex}");
        }

        if (!versionManifestManager.GetPropertyPath("assetIndex", null, out List<AllTypes> assetIndexPath, true)) //finds the path of the assetIndex property
        {
            return (false, "couldn't find the assetIndex property");
        }


        string[] assetIndexProperties = ["sha1", "size", "totalSize", "url"];

        if (!versionManifestManager.GetProperties(assetIndexProperties, assetIndexPath, out List<AllTypes> assetIndexValues)) //retrieves values from the assetIndex property
        {
            return (false, "couldn't find requested properties for assetIndex");
        }

        int assetIndexPropertiesIndex = -1;
        foreach (string key in assetIndexProperties) //checks for invalid values
        {
            if (string.IsNullOrEmpty(assetIndexValues[++assetIndexPropertiesIndex].Value.ToString()))
            {
                return (false, $"Property {key} is null, cannot continue");
            }
        }

        (success, string assetIndex) = await getAssetIndex(assetIndexValues[3].Value.ToString()!, assetValue, minecraftDirectory!, assetIndexValues[0].Value.ToString()!);

        if (!success || string.IsNullOrEmpty(assetIndex))
        {
            return (false, "the asset index file is null");
        }
        int assetSize;
        try
        {
            Debugger.SendInfo("value : " + (assetIndexValues[1].Value)); //YOU WERE FIXING THIS
            assetSize = (int)assetIndexValues[1].Value;
        }
        catch (Exception ex)
        {
            return (false, $"error while converting a value : {ex}");
        }

        (success, string message) = await downloadAssets(assetIndex, assetSize, minecraftDirectory!);

        if (!success)
        {
            return (false, message);
        }




        #endregion assets

        return (true, "made it to the end");
        
    }
    private static async Task<(bool, string?)> getVersionsManifest()
    {
        (bool success, AllTypes content) result;
        //string testUrl = "https://ash-speed.hetzner.com/1GB.bin";

        UIHelper.SetMainDownloadProgressBarMaximum(250905);
        Debugger.SendInfo("Versions manifest");

        result = await DownloadHelper.DownloadWithProgressAsync(versionsManifestUrl, "byte[]", //so cleeeeeaaaaan 🌟✨ | looking at this weeks later, I'm horrified
            progressBytes => UIHelper.UpdateMainDownloadProgressBarTarget(progressBytes),
            finalProgressBytes => UIHelper.UpdateMainDownloadProgressBarTarget(finalProgressBytes),
            progressSpeed => UIManager.MainDownloadTextBlock.Text = (progressSpeed?.ToString("F2") ?? "") + " MB/s",
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

        Debugger.SendInfo("version manifest");
        result = await DownloadHelper.DownloadWithProgressAsync(versionManifestUrl, "byte[]", 
            progressBytes => UIHelper.UpdateMainDownloadProgressBarTarget(progressBytes),
            finalProgressBytes => UIHelper.UpdateMainDownloadProgressBarTarget(finalProgressBytes),
            progressSpeed => UIManager.MainDownloadTextBlock.Text = (progressSpeed?.ToString("F2") ?? "") + " MB/s",
            isCorrupted => Debugger.SendError("File is corrupted"),
            size => UIHelper.SetMainDownloadProgressBarMaximum(size),
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

        foreach (List<AllTypes> output in bothOutputs) //verification
        {
            foreach (AllTypes keyOutput in output)
            {
                if (String.IsNullOrEmpty(keyOutput.ToString()))
                {
                    return (false, "a property's value wasn't valid during client gathering");
                }
            }
        }

        for (int i = 0; i < 2; i++) //download
        {
            List<AllTypes> listToProcess = bothOutputs[i];

            bool shouldReturn = false;

            (bool success, AllTypes content) result;

            Debugger.SendInfo("client");
            // to anyone who's reading this call, I am sorry
            result = await DownloadHelper.DownloadWithProgressAndWriteAsync(listToProcess[1].Value.ToString()!, null, @$"{minecraftDirectory}\versions\{version}", 
                bytesProgress => UIHelper.UpdateMainDownloadProgressBarTarget(bytesProgress),
                finalBytesProgress => UIHelper.UpdateMainDownloadProgressBarTarget(finalBytesProgress),
                speedProgress => UIManager.MainDownloadTextBlock.Text = (speedProgress?.ToString("F2") ?? "" )+ " MB/s",
                isCorrupted => { Debugger.SendError("Is Client or client_mappings corrupted ? " + isCorrupted);
                    shouldReturn = true;
                },
                obtainedSize => UIHelper.SetMainDownloadProgressBarMaximum(obtainedSize),
                listToProcess[0].Value.ToString()!);

            if (shouldReturn)
                return (false, "an error occurred");

            if (!result.success)
                return (false, "an error occured");
        }

        return (true, "client files are downloaded");
    }

    private static async Task<(bool success, string content)> getAssetIndex(string assetIndexUrl, string assetID, string minecraftDirectory, string hash)
    {
        (bool success, AllTypes versionManifest) result;

        bool shouldReturn = false;

        string assetIndexDirectory = minecraftDirectory + @$"/assets/indexes/";

        string fileName = $"{assetID}.json";

        string fullAssetIndexDirectory = assetIndexDirectory + fileName;

        Debugger.SendInfo("assetIndex"); 
        //downloading the file to the assigned directory
        result = await DownloadHelper.DownloadWithProgressAndWriteAsync(assetIndexUrl, fileName, assetIndexDirectory,
            totalReadBytes => UIHelper.UpdateMainDownloadProgressBarTarget(totalReadBytes),
            lastTotalReadBytes => UIHelper.UpdateMainDownloadProgressBarTarget(lastTotalReadBytes),
            speed => UIManager.MainDownloadTextBlock.Text = (speed?.ToString("F2") ?? "") + " MB/s",
            corrupted => {
                Debugger.SendError("Is Client or client_mappings corrupted ? " + corrupted);
                shouldReturn = true;
            },
            obtainedSize => UIManager.MainDownloadProgressBar.Maximum = obtainedSize,
            hash);

        if (shouldReturn)
            return (false, "file corrupted");

        if (!result.success)
            return (false, "an error occured");

        //Reading the file to make it ready for use
        Debugger.SendInfo("started reading asset Index file");
        if (!IoUtilities.File.ReadAllText(fullAssetIndexDirectory, out string assetIndex))
        {
            Debugger.SendError("ended reading the file cuz of error");
            return (false, "couldn't read the assetIndex file");
        }
        Debugger.SendInfo("ended reading the file");
        return (true, assetIndex);
    }

    private static async Task<(bool success, string content)> downloadAssets(string assetIndex, int assetsSize, string minecraftDirectory)
    {
        JsonUtility assetIndexManager = new JsonUtility(assetIndex);

        if (!assetIndexManager.GetPropertyPath("objects", null, out List<AllTypes> objectsPath, true))
        {
            return (false, "Could find objects in asset index.");
        }

        string[] keys = ["hash"];

        if (!assetIndexManager.GetPropertyPath("icons/icon_128x128.png", null, out List<AllTypes> relativePath, true))
        {
            return (false, "could find main property path");
        }

        JsonUtility.PathEditor.cutList(relativePath, objectsPath.Count + 1, out List<AllTypes> cutRelativePath); //Can't remember what that does.

        List<List<AllTypes>> finalList = [cutRelativePath];

        assetIndexManager.GetValuesInElementList(objectsPath, keys, finalList, out List<List<AllTypes>> foundValues);

        Debugger.SendInfo($"there are {foundValues.Count} elements");

        int i = 0;
        foreach (List<AllTypes> valuesPerObject in foundValues)
        {
            i++;
            foreach (AllTypes value in valuesPerObject)
            {
                if (String.IsNullOrEmpty(value.Value.ToString())){
                    return (false, $"A value in object n° {i} wasn't found.");
                }
            }
        }
        UIHelper.SetMainDownloadProgressBarMaximum(assetsSize);

        foreach (List<AllTypes> listOfValues in foundValues) //make the download here
        {
            string hash = listOfValues[0].Value.ToString()!;
            string first2Hexs = hash.Substring(0, 2);
            string relativeObjectPath = $"{first2Hexs}/{hash}";
            string assetUrl = $"{assetsUrl}/{relativeObjectPath}";
            string assetDirectory = minecraftDirectory + @$"/assets/objects/{relativeObjectPath}";

            long previousTotalReadBytes = 0;

            bool shouldReturn = false;

            (bool success, AllTypes message) result;

            result = await DownloadHelper.DownloadWithProgressAndWriteAsync(assetUrl, null, assetDirectory,
                totalBytesForCurrentAsset =>
                {
                    double progressBarValue = UIManager.MainDownloadProgressBar.Value;
                    UIHelper.UpdateMainDownloadProgressBarTarget(progressBarValue + totalBytesForCurrentAsset - previousTotalReadBytes);
                    previousTotalReadBytes = totalBytesForCurrentAsset;
                }, FinalBytesForCurrentObject =>
                {
                    double progressBarValue = UIManager.MainDownloadProgressBar.Value;
                    UIHelper.UpdateMainDownloadProgressBarTarget(progressBarValue + FinalBytesForCurrentObject - previousTotalReadBytes);
                    previousTotalReadBytes = FinalBytesForCurrentObject;
                }, speedUpdate => UIManager.MainDownloadTextBlock.Text = (speedUpdate?.ToString("F2") ?? "") + " MB/s",
                corrution => shouldReturn = true,
                null, hash);

            if (shouldReturn)
                return (false, "an object is corrupted");
            else if (!result.success)
                return (false, "error during an asset download");
        }
        return (true, "successfully downloaded the assets");
    }


}
