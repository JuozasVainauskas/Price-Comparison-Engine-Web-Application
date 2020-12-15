using System;

namespace PCE_Web.Classes
{
    class GenerateHash
    {
        public static string CreateSalt(int size)
        {
            var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            var buffer = new byte[size];
            rng.GetBytes(buffer);
            return Convert.ToBase64String(buffer);
        }

        public static string GenerateSha256Hash(string input, string salt)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(input + salt);
            var sha256HashString = new System.Security.Cryptography.SHA256Managed();
            var hash = sha256HashString.ComputeHash(bytes);

            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }
    }
}
