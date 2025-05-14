using Microsoft.Win32;

public static class FolderUtility
{
    private static string userDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    public static string? FolderPathRequest(bool Multiselect = false, string? DefaultDirectory = null /*TO DO default: subfolder under root -->t*/, string? InitialDirectory = null, string Title = "please choose folder(s)")
	{
        //basically a default parameter
        if(DefaultDirectory == null)
            DefaultDirectory = userDir; //why? idk

        if(InitialDirectory == null)
            InitialDirectory = Environment.CurrentDirectory;

        OpenFolderDialog getBaseDir = new OpenFolderDialog
        {
            Multiselect = Multiselect,
            DefaultDirectory = DefaultDirectory,
            InitialDirectory = InitialDirectory,
            Title = Title
        };

        bool? hasChosenAFolder = getBaseDir.ShowDialog(); //can be null somehow

        if (hasChosenAFolder == true)
        {
            return getBaseDir.FolderName;

        }
        return null;
    }

    public static string GetUserDir()
    {
        return userDir;
    }
}
