using System;
using System.Security.Cryptography;
using System.Text;

namespace Infrabel.ICT.Framework.Extension
{
    public static class ByteArrayExtensions
    {
        public static string ToHexadecimal(this byte[] content)
        {
            if (content == null || content.Length <= 0)
                return string.Empty;

            var sb = new StringBuilder();

            for (var i = 0; i < content.Length; i++)
                sb.AppendFormat("{0:X2}", content[i]);

            return sb.ToString();
        }

        public static string ToSHA256(this byte[] content)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            using (var hasher = SHA256.Create())
            {
                hasher.Initialize();
                return ToHexadecimal(hasher.ComputeHash(content));
            }
        }
    }
}