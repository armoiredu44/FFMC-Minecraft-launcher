using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

public class IMinecraftDownloadVersionManager_1_20_1 : IMinecraftDownloadVersionManager
{
    public async Task<bool> MainDownload(string path, string versionManifestUrl, string version)
    {
        string[] paths = { $@"{path}\assets\indexes", $@"{path}\assets\objects", $@"{path}\libraries" };
        FolderUtility.CreateFolder(paths);

        HttpUtility client = new HttpUtility();

        string? versionsManifest = await client.GetStringAsync(versionManifestUrl);
        if (String.IsNullOrEmpty(versionsManifest))
        {
            Debug.WriteLine("Can't get the versions manifest");
            return false;
        }

        JsonUtility jsonUtility = new JsonUtility(versionsManifest);

        if (!jsonUtility.GetPropertyPath("id", version, out string? VersionPath))
        {
            Debug.WriteLine($"The {version} property was not found");
            return false;
        }

        string[] keys = { "url", "sha1" };

        if (!jsonUtility.GetProperties(keys, VersionPath, out List<object?> values, out List<string?> _))
        {
            Debug.WriteLine("Couldn't find some keys");
            return false;
        }

        int i = 0;
        int nullValues = 0;
        foreach (object? value in values)
        {
            if (value == null)
            {
                Debug.WriteLine($"couldn't find a value for{keys[i]}");
            }
            i++;
            nullValues++; //resume here
        }

    }

}


