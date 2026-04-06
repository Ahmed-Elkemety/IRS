using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IRS.BLL.Help
{
    public static class Helper
    {
        public static string Hash(this string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Input cannot be null or empty", nameof(input));

            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
        public static string GenerateOtp(int digits = 6)
        {
            var random = new Random();
            var min = (int)Math.Pow(10, digits - 1);
            var max = (int)Math.Pow(10, digits) - 1;
            return random.Next(min, max).ToString();
        }

        public static string HashOtp(string code, string secret)
        {
            return (code + secret).Hash();
        }

        public static string GenerateSessionToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
