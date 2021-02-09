using System;
using System.Net;

namespace Infrabel.ICT.Framework.Extension
{
    public static class IPAddressExtensions
    {
        public static string FormatAddress(this IPAddress value) => ReferenceEquals(value, IPAddress.None) ? string.Empty : value.ToString();

        public static bool ValidateMaskedIPv4Address(this IPAddress value, IPAddress netMask)
        {
            var addressBytes = value.GetAddressBytes();
            var netMaskBytes = netMask.GetAddressBytes();
            if (addressBytes.Length != 4 || addressBytes.Length != netMaskBytes.Length)
                return false;

            var andXorSum = 0;
            var orSum = 0;
            for (var i = 0; i < addressBytes.Length; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    andXorSum += BitAtPosition(addressBytes[i], j) &
                                 (BitAtPosition(addressBytes[i], j) ^ BitAtPosition(netMaskBytes[i], j));
                    orSum += BitAtPosition(addressBytes[i], j) | BitAtPosition(netMaskBytes[i], j);
                }
            }

            return andXorSum > 0 && orSum < 32;
        }

        public static bool ValidateIPv4NetMask(this IPAddress value)
        {
            var addressBytes = value.GetAddressBytes();
            if (addressBytes.Length != 4)
                return false;

            //Must start with 1
            if (BitAtPosition(addressBytes[0], 0) == 0)
                return false;

            var oneForbidden = false;
            for (var i = 1; i < addressBytes.Length; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    if (!oneForbidden)
                    {
                        if (BitAtPosition(addressBytes[i], j) == 0)
                            oneForbidden = true;
                        else if (i == addressBytes.Length - 1 && j == 7)
                            return false;
                    }
                    else if (BitAtPosition(addressBytes[i], j) == 1)
                        return false;
                }
            }

            return true;
        }

        private static int BitAtPosition(byte value, int position)
        {
            if (position > 7)
                throw new ArgumentOutOfRangeException(nameof(position));
            var shift = 7 - position;

            return (value & (1 << shift)) >> shift;
        }
    }
}