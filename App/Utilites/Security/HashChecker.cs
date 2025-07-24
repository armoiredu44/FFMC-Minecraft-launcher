using System.Security.Cryptography;
using System.IO;
using System.Runtime.Intrinsics.Arm;
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
    
    public class IncrementalHasher
    {
        private readonly SHA1 _sha1 = SHA1.Create();

        public void AddBlock(byte[] buffer, int bytesRead)
        {
            _sha1.TransformBlock(buffer, 0, bytesRead, null, 0);
        }

        public string FinalizeHash()
        {
            byte[] hashBytes = _sha1.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
            return Convert.ToHexStringLower(hashBytes);
        }
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
