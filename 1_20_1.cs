using Microsoft.Windows;
using System.Diagnostics;
using System.Windows;
using System.Security.Cryptography;
using System.Text;

public static class Download_1_20_1
{
    public async static Task<bool> DownloadVersion1_20_1(string minecraftDirectory, string versionsManifestUrl, string version)
    {
        //Creation of a bunch of directories

        FolderUtility.CreateFolder($@"{minecraftDirectory}\assets\indexes"); //this creates 2 directories
        FolderUtility.CreateFolder($@"{minecraftDirectory}\assets\objects");
        FolderUtility.CreateFolder($@"{minecraftDirectory}\libraries");


		
        HttpUtility client = new HttpUtility(); //consider using a better memory optimisation with IDisposable

        #region versions manifest part

        string? versionsManifest = await client.GetAsync(versionsManifestUrl);
	    if (versionsManifest == null)
	    {
		    MessageBox.Show("versionManifest was not successfully downloaded, TO FIX");
		    return false;
	    }

        //find the 1.20.1 object in the version manifest

        JsonUtility JsonUtility = new JsonUtility(versionsManifest);

        if (!JsonUtility.GetPropertyPath("id", version, out string? manifestVersionPath))
        {
            MessageBox.Show("Version object not found. TO FIX");
            return false;
        }

        string[] key = { "url", "sha1" };

        if (!JsonUtility.GetProperties(key, manifestVersionPath, out List<object?> value, out List<string?> types))
        { //This will not work, GetProperties never returns false for now, consider implementing it.
            MessageBox.Show("Some of the keys were not found. TO FIX");
            return false;
        }

        if (value[0] == null)
        {
            MessageBox.Show($"Key {key[0]} not found");
            return false;
        }

        if (types[0] != "String")
        {
            MessageBox.Show("VersionUrl was not the right data type : This is NOT GOOD");
            return false;
        }

        string versionUrl = value[0]!.ToString()!; //must know the data type in advance

        string versionUrlSha1 = value[1]!.ToString()!; //same

        #endregion

        #region 1.20.1 manifest part

        #region getting the file

        string? versionJson = await client.GetAsync(versionUrl);

        if (versionJson == null)
        {
            MessageBox.Show("Couldn't get the 1.20.1 Json file");
            return false;
        }

        if (isHashTheSame(versionUrlSha1, versionJson))
        {
            Debug.WriteLine("hash is the same !");
        }
        else
        {
            Debug.WriteLine("hash is different");
        }

        #endregion

        #region assetIndex part

        JsonUtility = new JsonUtility(versionJson);

        if (!JsonUtility.GetPropertyPath("id", "5", out string? assetPath))
        {
            MessageBox.Show("AssetIndex not found");
                        return false;
        }

        string[] assetKeys = { "url", "sha1", "totalSize" };

        if (!JsonUtility.GetProperties(assetKeys, assetPath, out List<object?> assetValues, out List<string?> assetTypes)) //will awlays be true, consider fixing it.
        {
            MessageBox.Show("Asset properties not found.");
            return false;
        }

        if (assetValues[0] is null)
        {
            MessageBox.Show("couldn't find url");
            return false;
        }
        if (assetValues[1] is null)
        {
            MessageBox.Show("couldn't find sha1");
            return false;
        }

        string assetUrl = assetValues[0]!.ToString()!; //I like those "!"

        string assetsSha1 = assetValues[1]!.ToString()!;

        int? assetsTotalSize = JsonUtility.ConvertJsonElementToInt(assetValues[2]);

        foreach (string? type in assetTypes)
        {
            Debug.WriteLine(type);
        }

        string? assetIndexFile = await client.GetAsync(assetUrl);

        if (!isHashTheSame(assetsSha1, assetIndexFile!))
        {
            MessageBox.Show("asset file is corrupted");
            return false;
        }

        Debug.WriteLine("asset file is the same");
        return true;
        #endregion

        #endregion

    }

    public static bool isHashTheSame(string givenHash, string strToHash) //This method was easy to write ! TODO : consider making its own class
    {
        byte[] bytes = Encoding.UTF8.GetBytes(strToHash);

        using (SHA1 sha1 = SHA1.Create()) //This is memory optimised 🙂
        {
            byte[] computedHash = sha1.ComputeHash(bytes); 

            string hexHash = Convert.ToHexStringLower(computedHash); //Lower because that's how it is in the json

            if (hexHash == givenHash)
                return true;
            return false;

        }
           
    }
}
