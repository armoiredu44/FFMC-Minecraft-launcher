using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

public class MinecraftDownloadVersionManager_1_20_1 : IMinecraftDownloadVersionManager
{
    public static async Task<bool> MainDownload(string path, string versionManifestUrl, string version)
    {
        string[] paths = { $@"{path}\assets\indexes", $@"{path}\assets\objects", $@"{path}\libraries" }; //Creates some directories
        FolderUtility.CreateFolder(paths);

        HttpUtility client = new HttpUtility();

        #region versions manifest
        string? versionsManifest = await client.GetStringAsync(versionManifestUrl); //Downloads the versions json manifest
        if (String.IsNullOrEmpty(versionsManifest))
        {
            Debug.WriteLine("Can't get the versions manifest");
            return false;
        }

        JsonUtility jsonUtility = new JsonUtility(versionsManifest);

        if (!jsonUtility.GetPropertyPath("id", version, out string? VersionPath)) //Finds the path of the "id" property
        {
            Debug.WriteLine($"The {version} property was not found");
            return false;
        }

        string[] keys = { "url", "sha1" };

        if (!jsonUtility.GetProperties(keys, VersionPath, out List<object?> values, out List<string?> _)) //Gets some other properties from that path
        {
            Debug.WriteLine("Couldn't find some keys");
            return false;
        }

        #region values debug
        int i = 0;
        int nullValues = 0;
        foreach (object? value in values) //Debug for null values
        {
            if (value == null)
            {
                Debug.WriteLine($"couldn't find a value for{keys[i]}");
            }
            
            if (value == null && i == 0)
            {
                Debug.WriteLine($"Couldn't fetch versionUrl for {version}");
                return false;
            }
            i++;
            nullValues++; 
        }
        #endregion
        #endregion
        
        //consider making this part into its own thing, maybe
        string? versionManifest = await client.GetStringAsync(values[0]!.ToString()!); //Downloads the selected version manifest
        if (String.IsNullOrEmpty(versionManifest))
        {
            Debug.WriteLine("Can't get the version manifest");
            return false;
        }

        jsonUtility = new JsonUtility(versionManifest);

        jsonUtility.GetPropertyPath("assetIndex")

    }

}


