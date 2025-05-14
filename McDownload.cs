public static class McDownload
{
	private static string? mcDir;
	private static string appDir = Environment.CurrentDirectory;
	private static string versionsManifestUrl = "https://launchermeta.mojang.com/mc/game/version_manifest_v2.json";
	private static string? versionsManifest;


	public static async Task<bool> DownloadMinecraft(string version)
	{
		SetMcDir(getMinecraftRootDir(appDir, "Please choose a folder to install Minecraft to."));
		HttpUtility client = new HttpUtility();

		//get the version manifest
		versionsManifest = await client.GetAsync(versionsManifestUrl);
		return true;
	}

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


	private static void downloadAssets(string version)
	{

	}
}
