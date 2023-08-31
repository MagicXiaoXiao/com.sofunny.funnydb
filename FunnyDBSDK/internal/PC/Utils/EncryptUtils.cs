#if UNITY_STANDALONE || UNITY_EDITOR
using System;
using System.Text;
using System.Security.Cryptography;

/// <summary>
/// Do Encrypt For Ingest
/// </summary>
namespace SoFunny.FunnyDB.PC {
    internal sealed class EncryptUtils {
        public static string GetEncryptSign(string KeySecret, string content) {
            string base64Str = string.Empty;
            using (HMACSHA256 mac = new HMACSHA256(Encoding.UTF8.GetBytes(KeySecret)))
            {
                byte[] hash = mac.ComputeHash(Encoding.UTF8.GetBytes(content));
                base64Str = Convert.ToBase64String(hash);
            }
            return base64Str;
        }
    }
}
#endif