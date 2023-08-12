using System.Security.Cryptography;
using System.Text;

namespace FunnyExperience.Server.Lib;

public static class SecurityHelpers
{
    public static string ComputeSha256Hash(string rawData)
    {
        // Create a SHA256   
        using var sha256Hash = SHA256.Create();
        // ComputeHash - returns byte array  
        var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

        // Convert byte array to a string   
        var builder = new StringBuilder();
        foreach (var @byte in bytes)
        {
            builder.Append(@byte.ToString("x2"));
        }
        return builder.ToString();
    }

    public static string ComputeHmacSha256Hash(string rawData, string secretKey)
    {
        var hash = new StringBuilder(); ;
        var secretkeyBytes = Encoding.UTF8.GetBytes(secretKey);
        var inputBytes = Encoding.UTF8.GetBytes(rawData);
        using (var hmac = new HMACSHA256(secretkeyBytes))
        {
            byte[] hashValue = hmac.ComputeHash(inputBytes);
            foreach (var theByte in hashValue)
            {
                hash.Append(theByte.ToString("x2"));
            }
        }

        return hash.ToString();
    }

    public static string ComputeSha512Hash(string rawData, string secretKey)
    {
        var hash = new StringBuilder(); ;
        var secretkeyBytes = Encoding.UTF8.GetBytes(secretKey);
        var inputBytes = Encoding.UTF8.GetBytes(rawData);
        using (var hmac = new HMACSHA512(secretkeyBytes))
        {
            byte[] hashValue = hmac.ComputeHash(inputBytes);
            foreach (var theByte in hashValue)
            {
                hash.Append(theByte.ToString("x2"));
            }
        }

        return hash.ToString();
    }
}