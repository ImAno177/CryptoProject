using Crypto.Models;
using System.IO;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Crypto
{
    public static class HelperFunction
    {
        public static async Task<List<FileMetadata>> LoadAllFile(UserData userData)
        {
            HttpClient httpClient = new HttpClient();
            string serverUrl = "https://fastapi.crypto-lab.cloud/list-files/";

            string authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userData.Username}:{userData.Password}"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);

            var response = await httpClient.GetAsync(serverUrl);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();

                var jsonDoc = JsonDocument.Parse(json);
                var fileArray = jsonDoc.RootElement.GetProperty("files");

                List<FileMetadata> files = new List<FileMetadata>();
                foreach (var file in fileArray.EnumerateArray())
                {
                    files.Add(new FileMetadata
                    {
                        fileID = file.GetProperty("file_id").GetString(),
                        fileName = file.GetProperty("filename").GetString(),
                        role = file.GetProperty("role").GetString()
                    });
                }

                return files;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                MessageBox.Show("Invalid Action!", "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show($"Failed: {response.StatusCode}", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        public static async Task GenerateKey(UserData userData)
        {
            if(File.Exists("kyber_private.hex"))
            {
                return;
            }    
            
            var psi = new ProcessStartInfo
            {
                FileName = "python", 
                Arguments = "GenKey.py",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(psi))
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                string[] lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                File.WriteAllText("kyber_private.hex", lines[0]);

                string kyberPub = lines[1];
                string dilithiumPub = lines[2];
                string signature = lines[3];

                using (var client = new HttpClient())
                {
                    var authBytes = Encoding.UTF8.GetBytes($"{userData.Username}:{userData.Password}");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));

                    var form = new MultipartFormDataContent
                    {
                        { new StringContent(kyberPub), "ky_pub_hex" },
                        { new StringContent(dilithiumPub), "dil_pub_hex" },
                        { new StringContent(signature), "ky_sig_hex" }
                    };

                    string url = "https://fastapi.crypto-lab.cloud/register-keys/";
                    var response = await client.PostAsync(url, form);

                    string responseText = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("✅ Keys registered successfully");
                    }
                    else
                    {
                        MessageBox.Show($"❌ Error: {response.StatusCode}");
                        MessageBox.Show(responseText);
                    }
                }
            }
        }

        public static void UploadFile(UserData userData, string filepath, List<string> shared)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"EncryptFile.py --username \"{userData.Username}\" --password \"{userData.Password}\" --file \"{filepath}\" --recipients {string.Join(" ", shared)}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(psi))
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();
            }
        }

        public static void DownloadFile(UserData userData, string filepath, string filename, string fileID)
        {
            string kyber_private = File.ReadAllText("kyber_private.hex");

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"DecryptFile.py --username \"{userData.Username}\" --password \"{userData.Password}\" --file_id \"{fileID}\" --output_filepath \"{filepath}\" --output_filename \"{filename}\" --kyber_secret_hex \"{kyber_private}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(psi))
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();
            }
        }
    }
}
