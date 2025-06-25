using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Crypto.Models;
using System.Collections.Generic;
using System.Text.Json;

namespace Crypto
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll")]
        private static extern void ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern void SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        private void Drag_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private async void LoginButton_Click(object sender, EventArgs e)
        {
            HttpClient httpClient = new HttpClient();
            string serverUrl = "https://fastapi.crypto-lab.cloud/list-files/";

            string username = UsernameTb.Text.Trim();
            string password = PasswordTb.Text;

            string authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);

            try
            {
                var response = await httpClient.GetAsync(serverUrl);
                if (response.IsSuccessStatusCode)
                {
                    UserData userData = new UserData();
                    userData.Username = username;
                    userData.Password = password;

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

                    MainForm mainForm = new MainForm(userData, files);
                    this.Hide();
                    mainForm.Show();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    MessageBox.Show("Invalid password!", "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show($"Login Failed: {response.StatusCode}", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Internet Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
