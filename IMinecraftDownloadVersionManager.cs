using System.IO;
using System.Security.Cryptography;
using System.Text;

public interface IMinecraftDownloadVersionManager
{
    bool MainDownload(string path, string versionManifestUrl);
}
