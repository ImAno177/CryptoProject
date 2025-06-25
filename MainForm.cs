using Crypto.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using System.Windows.Forms;

namespace Crypto
{
    public partial class MainForm : Form
    {
        private UserData userData;
        private FileMetadata picked;

        public MainForm(UserData userData, List<FileMetadata> fileMetadata)
        {
            InitializeComponent();
            this.userData = new UserData();
            this.userData = userData;

            try
            {
                FileList.Rows.Clear();

                foreach (var file in fileMetadata)
                {
                    FileList.Rows.Add(
                        file.fileID,
                        file.fileName,
                        file.role
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading files: " + ex.Message);
            }

            picked = new FileMetadata();

            RegisterKey();
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

        private async void LoadFiles()
        {
            List<FileMetadata> files = await HelperFunction.LoadAllFile(userData);
            FileList.Rows.Clear();

            foreach (var file in files)
            {
                FileList.Rows.Add(
                    file.fileID,
                    file.fileName,
                    file.role
                );
            }
        }

        private async void RegisterKey()
        {
            await HelperFunction.GenerateKey(userData);
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void F5Button_Click(object sender, EventArgs e)
        {
            LoadFiles();
        }

        private void FileList_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = FileList.Rows[e.RowIndex];
                picked.fileID = row.Cells[0].Value.ToString();
                picked.fileName = row.Cells[1].Value.ToString();
                picked.role = row.Cells[2].Value.ToString();
            }
        }

        private void DownloadBtn_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Title = "Save As";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string filePath = dialog.FileName;
                        string filename = Path.GetFileName(filePath);

                        using (FileStream fs = File.Create(filePath))
                        {}

                        HelperFunction.DownloadFile(userData, filePath, filename, picked.fileID);
                        MessageBox.Show("Download complete!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Download failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void UploadBtn_Click(object sender, EventArgs e)
        {
            var options = new List<string> { "user1", "user2", "user3" };
            options.Remove(userData.Username);
            List<string> recipients = new List<string>();
            using (var dialog = new UserCheckBoxForm(options))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var selected = dialog.SelectedOptions;
                    if (selected.Any())
                        recipients = selected;
                    else
                        MessageBox.Show("No options selected.");
                }
            }

            recipients.Add(userData.Username);

            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Open";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string filePath = dialog.FileName;
                        string originalFilename = Path.GetFileName(filePath);

                        HelperFunction.UploadFile(userData, filePath, recipients);

                        MessageBox.Show("Upload complete!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Upload failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
