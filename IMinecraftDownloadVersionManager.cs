using System.IO;
using System.Security.Cryptography;
using System.Text;

interface IMinecraftDownloadVersionManager
{
    bool MainDownload(string path, string versionManifestUrl);
}
