using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace lab8
{
    class StringCrypter
    {
        public string CryptKey { private get; set; } = "SomeKey";

        public string Encrypt(string stringToEncrypt)
        {
            var cspp = new CspParameters
            {
                KeyContainerName = CryptKey
            };
            var rsa = new RSACryptoServiceProvider(cspp)
            {
                PersistKeyInCsp = true
            };
            return BitConverter.ToString(rsa.Encrypt(Encoding.UTF8.GetBytes(stringToEncrypt), true));
        }

        public string Decrypt(string stringToDecrypt)
        {
            try
            {
                var cspp = new CspParameters
                {
                    KeyContainerName = CryptKey
                };
                var rsa = new RSACryptoServiceProvider(cspp)
                {
                    PersistKeyInCsp = true
                };

                var decryptArray = stringToDecrypt.Split(new[] { "-" }, StringSplitOptions.None);
                var decryptByteArray = Array.ConvertAll(decryptArray, s => Convert.ToByte(byte.Parse(s, NumberStyles.HexNumber)));
                return Encoding.UTF8.GetString(rsa.Decrypt(decryptByteArray, true));
            }
            catch (Exception)
            {
                // ignored
            }
            return null;
        }
    }
}
