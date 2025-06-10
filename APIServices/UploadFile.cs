using Crypto.HelperClass;
using Crypto.Models;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crypto.APIServices
{
    internal class UploadFile
    {
        private static readonly HttpClient client = new HttpClient();
        private const string baseUrl = "https://fastapi.crypto-lab.cloud";

        public static async Task UploadFileAsync(string InputPath, string Filename)
        {
            var responsePayload = new
            {
                server_kyber_pubkey_hex = "",
                session_id = ""
            };

            string jsonPayload = JsonSerializer.Serialize(responsePayload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{baseUrl}/upload/initiate", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var serverKeyInfo = JsonSerializer.Deserialize<ServerKeyResponse>(json);
            byte[] serverKyberPublicKey = Converter.HexToBytes(serverKeyInfo.server_kyber_pk_hex);
            string session_id = serverKeyInfo.session_id;

            byte[] aesKey = CryptoHelper.GenerateRandomBytes(32);
            byte[] nonce = CryptoHelper.GenerateRandomBytes(12);
            string tempEncryptedPath = Path.GetTempFileName();
            byte[] tag;
            string sha256Checksum = CryptoHelper.CalculateSha256Checksum(InputPath);

            using (var inputStream = File.OpenRead(InputPath))
            using (var outputStream = File.Create(tempEncryptedPath))
            {
                tag = EncryptStreamAesGcm(inputStream, outputStream, aesKey, nonce);
            }

            var (encapsulatedKey, _) = CryptoHelper.EncapsulateKyber(serverKyberPublicKey);

            using (var multipart = new MultipartFormDataContent())
            {
                multipart.Add(new StringContent(session_id), "session_id");
                multipart.Add(new StringContent(BitConverter.ToString(encapsulatedKey).Replace("-", "").ToLower()), "kyber_encapsulated_aes_key_hex");
                multipart.Add(new StringContent(BitConverter.ToString(nonce).Replace("-", "").ToLower()), "aes_nonce_hex");
                multipart.Add(new StringContent(BitConverter.ToString(tag).Replace("-", "").ToLower()), "aes_tag_hex");
                multipart.Add(new StringContent(sha256Checksum.ToLower()), "plaintext_sha256_checksum_hex");

                var fileContent = new ByteArrayContent(File.ReadAllBytes(tempEncryptedPath));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                multipart.Add(fileContent, "file", Path.GetFileName(InputPath));

                var uploadResponse = await client.PostAsync($"{baseUrl}/upload", multipart);
                //server bug: Upload processing error: name 'aes_tag_for_verification_bytes' is not defined

                MessageBox.Show(uploadResponse.ToString());

                uploadResponse.EnsureSuccessStatusCode();
            }

            File.Delete(tempEncryptedPath);
        }

        public static byte[] EncryptStreamAesGcm(Stream inputStream, Stream outputStream, byte[] key, byte[] nonce)
        {
            var cipher = new GcmBlockCipher(new AesEngine());
            var parameters = new AeadParameters(new KeyParameter(key), 128, nonce);
            cipher.Init(true, parameters);

            byte[] buffer = new byte[1024 * 1024];
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

            return cipher.GetMac();
        }
    }
}
