using System.Security.Cryptography;
using System.Text;
public class HashChecker: Utilities
{
    public static bool isHashTheSameForString(string toCheck, string hash)
    {
        string obtainedHash = getHashOfString(toCheck);
        if (obtainedHash == hash)
            return true;
        return false;
    }

    private static string getHashOfString(string toCheck)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(toCheck);
        using (SHA1 sha1 = SHA1.Create())
        {
            byte[] hashBytes = sha1.ComputeHash(inputBytes);

            string HexBytes =Convert.ToHexStringLower(hashBytes);
            return HexBytes;
        }

    }
}
