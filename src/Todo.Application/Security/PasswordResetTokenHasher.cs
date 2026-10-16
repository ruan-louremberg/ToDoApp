using System.Security.Cryptography;
using System.Text;

namespace ToDoApp.Application.Security;

internal static class PasswordResetTokenHasher
{
    public static string Hash(string token)
    {
        var tokenBytes = Encoding.UTF8.GetBytes(token);
        return Convert.ToHexString(SHA256.HashData(tokenBytes));
    }
}
