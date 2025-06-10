using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Kems;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;
using System.IO;

namespace Crypto.HelperClass
{
    internal class CryptoHelper
    {
        private const string KemAlgorithmName = "ML-KEM-512"; // Match server
        private static readonly MLKemParameters KemParameters = MLKemParameters.ml_kem_512;

        public static (byte[] publicKey, byte[] privateKey) GenerateKyberKeyPair()
        {
            var random = new SecureRandom();
            var keyGenParameters = new MLKemKeyGenerationParameters(random, KemParameters);
            var generator = new MLKemKeyPairGenerator();
            generator.Init(keyGenParameters);
            var keyPair = generator.GenerateKeyPair();

            var publicKeyParams = (MLKemPublicKeyParameters)keyPair.Public;
            var privateKeyParams = (MLKemPrivateKeyParameters)keyPair.Private;

            return (publicKeyParams.GetEncoded(), privateKeyParams.GetEncoded());
        }

        public static byte[] DecapsulateKyber(byte[] clientPrivateKeyBytes, byte[] encapsulatedKeyFromServerBytes)
        {
            var clientPrivateKeyParams = MLKemPrivateKeyParameters.FromEncoding(KemParameters, clientPrivateKeyBytes);
            var decapsulator = new MLKemDecapsulator(KemParameters);
            decapsulator.Init(clientPrivateKeyParams);

            byte[] sharedSecret = new byte[decapsulator.SecretLength];
            decapsulator.Decapsulate(encapsulatedKeyFromServerBytes, 0, encapsulatedKeyFromServerBytes.Length, sharedSecret, 0, sharedSecret.Length);
            return sharedSecret;
        }

        public static byte[] GenerateRandomBytes(int length)
        {
            var bytes = new byte[length];
            new SecureRandom().NextBytes(bytes);
            return bytes;
        }

        public static (byte[] encapsulatedKey, byte[] sharedSecret) EncapsulateKyber(byte[] serverPublicKeyBytes)
        {
            var serverPublicKeyParams = MLKemPublicKeyParameters.FromEncoding(KemParameters, serverPublicKeyBytes);
            var encapsulator = new MLKemEncapsulator(KemParameters);
            encapsulator.Init(serverPublicKeyParams);

            byte[] encapsulatedKey = new byte[encapsulator.EncapsulationLength];
            byte[] sharedSecret = new byte[encapsulator.SecretLength];

            encapsulator.Encapsulate(encapsulatedKey, 0, encapsulatedKey.Length, sharedSecret, 0, sharedSecret.Length);
            return (encapsulatedKey, sharedSecret);
        }

        public static string CalculateSha256Checksum(string filePath)
        {
            using (var sha256 = SHA256.Create())
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hash = sha256.ComputeHash(stream);
                return Converter.ToHexString(hash);
            }
        }
    }
}
