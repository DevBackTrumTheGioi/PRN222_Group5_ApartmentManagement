using System.Security.Cryptography;
using System.Text;

namespace PRN222_ApartmentManagement.Utils;

public static class PasswordGenerator
{
    private const string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";

    public static string GenerateRandomPassword(int length = 10)
    {
        var res = new StringBuilder();
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] uintBuffer = new byte[sizeof(uint)];

            while (length-- > 0)
            {
                rng.GetBytes(uintBuffer);
                uint num = BitConverter.ToUInt32(uintBuffer, 0);
                res.Append(AllowedChars[(int)(num % (uint)AllowedChars.Length)]);
            }
        }
        return res.ToString();
    }
}

