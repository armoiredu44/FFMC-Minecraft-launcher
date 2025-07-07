using System.Security.Cryptography;
using System.IO
public class HashChecker: Utilities
{
    public static bool isHashTheSame(byte[] toCheck, string hash)
    {
        string obtainedHash = getHash(toCheck);
        if (obtainedHash == hash)
            return true;
        return false;
    }

    public static bool isHashTheSame(MemoryStream toCheck, string hash)
    {
        string obtainedHash = getHash(toCheck);
        if (obtainedHash == hash)
            return true;
        return false;
    }

    private static string getHash(byte[] inputBytes)
    {
        using (SHA1 sha1 = SHA1.Create())
        {
            byte[] hashBytes = sha1.ComputeHash(inputBytes);

            string HexBytes = Convert.ToHexStringLower(hashBytes);
            return HexBytes;
        }

    }

    private static string getHash(MemoryStream inputBytes)
    {
        using (SHA1 sha1 = SHA1.Create())
        {
            byte[] hashBytes = sha1.ComputeHash(inputBytes);

            string HexBytes = Convert.ToHexStringLower(hashBytes);
            return HexBytes;
        }

    }
}
