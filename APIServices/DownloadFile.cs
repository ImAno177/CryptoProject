using Crypto.Models;
using Crypto.HelperClass;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using System.Security.Cryptography;

namespace Crypto.APIServices
{
    internal class DownloadFile
    {
        private static readonly HttpClient client = new HttpClient();
        private const string baseUrl = "https://fastapi.crypto-lab.cloud";

        public static async Task DownloadFileAsync(string fileId, string outputPath)
        {
            var (publicKey, privateKey) = CryptoHelper.GenerateKyberKeyPair();

            var requestPayload = new
            {
                client_kyber_pk_hex = BitConverter.ToString(publicKey).Replace("-", "").ToLower()
            };

            string jsonPayload = JsonSerializer.Serialize(requestPayload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var initiateResponse = await client.PostAsync($"{baseUrl}/download/initiate/{fileId}", content);
            initiateResponse.EnsureSuccessStatusCode();
            string initiateJson = await initiateResponse.Content.ReadAsStringAsync();

            var downloadInfo = JsonSerializer.Deserialize<DownloadInitiateResponse>(initiateJson)
                ?? throw new Exception("Failed to deserialize initiate response");

            byte[] encapsulatedKey = Converter.HexToBytes(downloadInfo.kyber_encapsulated_aes_key_hex);
            byte[] aesKey = CryptoHelper.DecapsulateKyber(privateKey, encapsulatedKey);

            byte[] aesNonce = Converter.HexToBytes(downloadInfo.aes_nonce_hex);
            byte[] aesTag = Converter.HexToBytes(downloadInfo.aes_tag_hex);

            string tempEncryptedPath = Path.GetTempFileName();

            using (var fs = new FileStream(tempEncryptedPath, FileMode.Create, FileAccess.Write))
            {
                var fileResponse = await client.GetAsync($"{baseUrl}/download/{downloadInfo.download_token}");
                fileResponse.EnsureSuccessStatusCode();
                await fileResponse.Content.CopyToAsync(fs);

                fs.Write(aesTag, 0, aesTag.Length);
            }

            DecryptStreamAesGcm(tempEncryptedPath, outputPath, aesKey, aesNonce);

            File.Delete(tempEncryptedPath);

            Console.WriteLine($"File saved to: {outputPath}");
        }

        private static void DecryptStreamAesGcm(string encryptedPath, string outputPath, byte[] key, byte[] nonce)
        {
            using (var inputStream = new FileStream(encryptedPath, FileMode.Open, FileAccess.Read))
            using (var outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
            {
                var cipher = new GcmBlockCipher(new AesEngine());
                var parameters = new AeadParameters(new KeyParameter(key), 128, nonce);
                cipher.Init(false, parameters);

                byte[] buffer = new byte[1024 * 1024 + 16];
                int bytesRead;
                try
                {
                    while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        byte[] plainChunk = new byte[cipher.GetUpdateOutputSize(bytesRead)];
                        int len = cipher.ProcessBytes(buffer, 0, bytesRead, plainChunk, 0);
                        outputStream.Write(plainChunk, 0, len);
                    }

                    byte[] finalPlainChunk = new byte[cipher.GetOutputSize(0)];
                    int finalLen = cipher.DoFinal(finalPlainChunk, 0);
                    outputStream.Write(finalPlainChunk, 0, finalLen);
                }
                catch (InvalidCipherTextException ex)
                {
                    throw new CryptographicException("AES-GCM decryption failed. Tag mismatch or corrupted data.", ex);
                }
            }
        }
    }
}
