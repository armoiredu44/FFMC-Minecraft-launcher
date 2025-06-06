using System.IO;
using System.Security.Cryptography;
using System.Text;

public interface IMinecraftDownloadVersionManager
{
    Task<bool> MainDownload();
}
