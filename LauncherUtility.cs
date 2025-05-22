using System;

public static class LauncherUtility //takes care of everything about the launcher
{
	private static string mcVersion = "1.20.1";

	public static string GetMcVersion()
	{
		return mcVersion;
	}

	public static void SetMcVerion(string version)
	{
		mcVersion = version;
	}

	public async static void DownloadMinecraft()
	{
		await McDownload.DownloadMinecraft(mcVersion);
	}
}
