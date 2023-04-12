using System.Security.Cryptography;
using System.Text;

namespace Binance
{
    internal class BinanceRSA
    {
        public static void SetRSAImportPrivateKey(RSA rsa, string privateKeyPath)
        {
            string privateKey = File.ReadAllText(privateKeyPath);
            rsa.ImportFromPem(privateKey);
        }

        public static string SignData(RSA rsa, string api_params_with_timestamp)
        {
            byte[] signatureBytes = rsa.SignData(Encoding.UTF8.GetBytes(api_params_with_timestamp), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(signatureBytes);
        }

    }
     
}
