using System.Diagnostics;
using System.Windows;

public static class McDownload // takes care of the minecraft downloading process
{
	private static string? mcDir;
	private static string appDir = Environment.CurrentDirectory;
	private static readonly string versionsManifestUrl = "https://launchermeta.mojang.com/mc/game/version_manifest_v2.json";


	public static void SetMcDir(string? McDir)
	{
		mcDir = McDir;
	}

	public static string? GetMCDir()
	{
		return mcDir;
	}

	private static string? getMinecraftRootDir(string InitialDirectory, string Title = "please choose folder(s)")
	{
        return FolderUtility.FolderPathRequest(false, null, InitialDirectory, Title);
    }

	public static async Task<bool> DownloadMinecraft(string version) //This is gonna be a mess
	{
		
		SetMcDir(getMinecraftRootDir(appDir, "Please choose a folder to install Minecraft to.")); //extra steps
		if (mcDir == null)
		{
			MessageBox.Show("Un chemin est nécessaire pour installer Minecraft !"); //remove that, or make it different
			return false;
		}

		Debug.WriteLine(mcDir);

		if (version == "1.20.1")
		{
            
		}

        return false;
		


		
		
	}




	private static bool downloadAssets(string minecraftDirectory, string version, string? versionManifest)
	{
		return true;
	}
}
