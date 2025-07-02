public class MinecraftDownloadVersionManager_1_20_1 : IMinecraftDownloadVersionManager
{
    private readonly string versionsManifest;
    private readonly string path;

    public MinecraftDownloadVersionManager_1_20_1(string versionsManifestParameter, string pathParameter)
    {
        versionsManifest = versionsManifestParameter;
        path = pathParameter;
    }

    public async Task<bool> MainDownload()
    {
        /*
        string[] paths = { $@"{path}\assets\indexes", $@"{path}\assets\objects", $@"{path}\libraries" }; //Creates some directories
        FolderUtility.CreateFolder(paths);

        HttpUtility client = new HttpUtility();

        #region versions manifest
        

        JsonUtility jsonUtility = new JsonUtility(versionsManifest);

        if (!jsonUtility.GetPropertyPath("id", "1.20.1", out string? VersionPath)) //Finds the path of the "id" property
        {
            Debug.WriteLine($"The 1.20.1 property was not found");
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
                Debug.WriteLine($"Couldn't fetch versionUrl for 1.20.1");
                return false;
            }
            i++;
            nullValues++; 
        }
        #endregion
        #endregion
        //WARNING GET SOME NULL CHECK
        //consider making this part into its own thing if many others version need this too.
        string? versionManifest = await client.GetStringAsync(values[0]!.ToString()!); //Downloads the selected version manifest
        if (String.IsNullOrEmpty(versionManifest))
        {
            Debug.WriteLine("Can't get the version manifest");
            return false;
        }
        if (!HashChecker.isHashTheSameForString(versionManifest, values[1]!.ToString()!))
        {
            Debug.WriteLine("1.20.1 version manifest is corrupted");
            return false;
        }

        jsonUtility = new JsonUtility(versionManifest);

        #region assets

        jsonUtility.GetPropertyPathOfValueFromKey("assetIndex", out string? assetPath);
        keys = ["url", "sha1", "totalSize"];
        jsonUtility.GetProperties(keys, assetPath, out values, out List<string?> _);

        string? assetManifest = await client.GetStringAsync(values[0]!.ToString()!); //NULL CHECKER NEEDED lol (for the url), downloads the asset manifest
        if (String.IsNullOrEmpty(assetManifest))
        {
            Debug.WriteLine("Can't get the version manifest");
            return false;
        }
        if (!HashChecker.isHashTheSameForString(assetManifest, values[1]!.ToString()!))
        {
            Debug.WriteLine("asset manifest is corrupted");
            return false;
        }
        #region looping throught the assets and main download thing for them

        jsonUtility = new JsonUtility(assetManifest);


        #endregion
        #endregion
        */

        return true;
    }



}


