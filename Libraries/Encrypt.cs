using System.Security.Cryptography;

namespace HospitalAppl.Libraies{

public class Encrypt
{
    public static bool MatchSHA1(byte[] p1, byte[] p2)
        {
        bool result = false;
        if (p1 != null && p2 != null)
            {
            if (p1.Length == p2.Length)
                {
                result = true;
                for (int i = 0; i < p1.Length; i++)
                    {
                    if (p1[i] != p2[i])
                        {
                        result = false;
                        break;
                        }
                    }
                }
            }
        return result;
        }

    public static byte[] GetSHA1(string password)
    {
        SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
        return sha.ComputeHash(System.Text.Encoding.ASCII.GetBytes(password));
    }
};
}
