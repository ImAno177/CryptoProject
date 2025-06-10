using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Kems;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography; // For SHA256

public static class CryptoService
{
    private const string KemAlgorithmName = "ML-KEM-512"; // Match server
    private static readonly MLKemParameters KemParameters = GetKemParams(KemAlgorithmName); // e.g., MLKemParameters.ml_kem_512

    private static MLKemParameters GetKemParams(string algoName)
    {
        // Simplified: Add more robust mapping if needed
        if (algoName == "ML-KEM-512") return MLKemParameters.ml_kem_512;
        if (algoName == "ML-KEM-768") return MLKemParameters.ml_kem_768;
        if (algoName == "ML-KEM-1024") return MLKemParameters.ml_kem_1024;
        throw new ArgumentException("Unsupported KEM algorithm");
    }


    public static class HexConverter
    {
        public static string ToHexString(byte[] ba) => BitConverter.ToString(ba).Replace("-", "").ToLowerInvariant();
        public static byte[] FromHexString(string hex)
        {
            hex = hex.StartsWith("0x") ? hex.Substring(2) : hex;
            if (hex.Length % 2 != 0) throw new ArgumentException("Hex string must have an even number of characters.");
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }

    // --- ML-KEM Operations ---
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

    public static (byte[] encapsulatedKey, byte[] sharedSecret) EncapsulateKyber(byte[] serverPublicKeyBytes)
    {
        var serverPublicKeyParams = MLKemPublicKeyParameters.FromEncoding(KemParameters, serverPublicKeyBytes);
        var encapsulator = new MLKemEncapsulator(KemParameters);
        encapsulator.Init(serverPublicKeyParams);

        byte[] encapsulatedKey = new byte[encapsulator.EncapsulationLength];
        byte[] sharedSecret = new byte[encapsulator.SecretLength]; // This will be our AES key

        encapsulator.Encapsulate(encapsulatedKey, 0, encapsulatedKey.Length, sharedSecret, 0, sharedSecret.Length);
        return (encapsulatedKey, sharedSecret);
    }

    public static byte[] DecapsulateKyber(byte[] clientPrivateKeyBytes, byte[] encapsulatedKeyFromServerBytes)
    {
        var clientPrivateKeyParams = MLKemPrivateKeyParameters.FromEncoding(KemParameters, clientPrivateKeyBytes);
        var decapsulator = new MLKemDecapsulator(KemParameters);
        decapsulator.Init(clientPrivateKeyParams);

        byte[] sharedSecret = new byte[decapsulator.SecretLength]; // This is the AES key
        decapsulator.Decapsulate(encapsulatedKeyFromServerBytes, 0, encapsulatedKeyFromServerBytes.Length, sharedSecret, 0, sharedSecret.Length);
        return sharedSecret;
    }

    // --- AES-GCM Operations (using BouncyCastle) ---
    public static byte[] GenerateAesNonce(int size = 12) // Standard GCM nonce size
    {
        var nonce = new byte[size];
        new SecureRandom().NextBytes(nonce);
        return nonce;
    }
    public static byte[] GenerateAesKey(int size = 32) // 32 bytes for AES-256
    {
        var key = new byte[size];
        new SecureRandom().NextBytes(key);
        return key;
    }


    // Encrypts a stream and returns the tag. Ciphertext is written to outputStream.
    public static byte[] EncryptStreamAesGcm(Stream inputStream, Stream outputStream, byte[] key, byte[] nonce)
    {
        var cipher = new GcmBlockCipher(new AesEngine());
        var parameters = new AeadParameters(new KeyParameter(key), 128, nonce); // 128-bit tag
        cipher.Init(true, parameters); // true for encryption

        byte[] buffer = new byte[1024 * 1024]; // 1MB chunk size
        int bytesRead;
        while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            byte[] cipherChunk = new byte[cipher.GetUpdateOutputSize(bytesRead)];
            int len = cipher.ProcessBytes(buffer, 0, bytesRead, cipherChunk, 0);
            outputStream.Write(cipherChunk, 0, len);
        }
        byte[] finalChunk = new byte[cipher.GetOutputSize(0)];
        int finalLen = cipher.DoFinal(finalChunk, 0);
        outputStream.Write(finalChunk, 0, finalLen);

        return cipher.GetMac(); // Get the authentication tag
    }

    // Decrypts a stream. The inputStream MUST contain ciphertext + tag concatenated.
    public static void DecryptStreamAesGcm(Stream inputStreamWithTag, Stream outputStream, byte[] key, byte[] nonce)
    {
        var cipher = new GcmBlockCipher(new AesEngine());
        var parameters = new AeadParameters(new KeyParameter(key), 128, nonce); // 128-bit tag
        cipher.Init(false, parameters); // false for decryption

        byte[] buffer = new byte[1024 * 1024 + 16]; // Chunk size + potential tag
        int bytesRead;
        try
        {
            while ((bytesRead = inputStreamWithTag.Read(buffer, 0, buffer.Length)) > 0)
            {
                byte[] plainChunk = new byte[cipher.GetUpdateOutputSize(bytesRead)];
                int len = cipher.ProcessBytes(buffer, 0, bytesRead, plainChunk, 0);
                outputStream.Write(plainChunk, 0, len);
            }
            byte[] finalPlainChunk = new byte[cipher.GetOutputSize(0)];
            int finalLen = cipher.DoFinal(finalPlainChunk, 0); // Verifies tag
            outputStream.Write(finalPlainChunk, 0, finalLen);
        }
        catch (InvalidCipherTextException ex)
        {
            // Tag verification failed or other decryption error
            throw new CryptographicException("AES-GCM decryption failed. Tag mismatch or corrupted data.", ex);
        }
    }


    // --- SHA256 Checksum ---
    public static async System.Threading.Tasks.Task<string> CalculateSha256ChecksumAsync(string filePath)
    {
        using (var sha256 = SHA256.Create())
        using (var stream = File.OpenRead(filePath))
        {
            byte[] hash = await sha256.ComputeHashAsync(stream);
            return HexConverter.ToHexString(hash);
        }
    }
}