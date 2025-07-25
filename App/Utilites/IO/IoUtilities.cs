﻿using Microsoft.Win32;
using System.IO;
using System.Windows;

public class IoUtilities : Utilities
{
    public class Folder : IoUtilities
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
            Debugger.SendError($"Chemin {path}, erreur :{exception.ToString()}");
            return false;
        }

        return true;
    }

        public static bool CreateFolders(string[] paths)
    {
        foreach (string path in paths)
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception exception)
            {
                Debugger.SendError($"Chemin {path}, erreur :{exception.ToString()}");
                return false;
            }

        }

        return true;
    }

    }

    public class File : IoUtilities
    {
        public static string ReadAllText(string path)
        {
            string result = "";
            try
            {
                result = System.IO.File.ReadAllText(path);
            }
            catch (Exception exception)
            {
                Debugger.SendError($"Couldn't use ReadAllText on file {path} : {exception}");
            }
            return result;
        }

        public static bool AppendAllText(string path, string text)
        {
            try
            {
                System.IO.File.AppendAllText(path, text);
            }
            catch (Exception exception)
            {
                Debugger.SendError($"Couldn't use AppendAllText on file {path} : {exception}");
                return false;
            }
            return true;
        }

        public static bool AppendAllText(string path, MemoryStream memoryStream)
        {
            try
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    memoryStream.Position = 0;
                    memoryStream.CopyTo(fileStream);
                }
            }
            catch (Exception exception)
            {
                Debugger.SendError($"Couldn't save memoryStream to directory {path} because of error : {exception}");
                return false;
            }
            return true;
        }
    }
}
