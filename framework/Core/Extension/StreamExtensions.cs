using System;
using System.IO;
using System.Threading.Tasks;

namespace Infrabel.ICT.Framework.Extension
{
    public static class StreamExtensions
    {
        public static async ValueTask<byte[]> ToBytesAsync(this Stream stream)
        {
            if(!stream.CanRead)
                throw new InvalidOperationException();

            stream.Seek(0, SeekOrigin.Begin);
            using(var ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms);

                return ms.ToArray();
            }
        }
    }
}