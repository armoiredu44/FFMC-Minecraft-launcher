using Microsoft.Win32;
using System.IO;
using System.Windows;

public class FolderUtility : Utilities
{
    public static string? FolderPathRequest(bool Multiselect = false, string? DefaultDirectory = null /*TO DO default: subfolder under root -->t*/, string? InitialDirectory = null, string Title = "please choose folder(s)")
	{
        //basically a default parameter
        if(DefaultDirectory == null)
            DefaultDirectory = userDir; //why? idk

        if(InitialDirectory == null)
            InitialDirectory = appDir;

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

    public static bool CreateFolder(string path)
    {
        try
        {
            Directory.CreateDirectory(path);
        }
        catch (Exception exception)
        {
            MessageBox.Show($"Chemin {path}, erreur :{exception.ToString()}");
            return false;
        }

        return true;
    }

    public static bool CreateFolder(string[] paths)
    {
        foreach (string path in paths)
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Chemin {path}, erreur :{exception.ToString()}");
                return false;
            }

        }

        return true;
    }
}
