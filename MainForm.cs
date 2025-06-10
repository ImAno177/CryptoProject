using Crypto.HelperClass;
using Crypto.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crypto
{
    public partial class MainForm : Form
    {
        private FileListItem Picked;
        public MainForm()
        {
            InitializeComponent();
            LoadFiles.LoadAllFiles(this.FileList);
            Picked = new FileListItem();
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

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void F5Button_Click(object sender, EventArgs e)
        {
            LoadFiles.LoadAllFiles(this.FileList);
        }

        private void FileList_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = FileList.Rows[e.RowIndex];
                Picked.file_id = row.Cells[0].Value.ToString();
                Picked.original_filename = row.Cells[1].Value.ToString();
                Picked.size = int.Parse(row.Cells[2].Value.ToString());
                Picked.uploaded_at = row.Cells[3].Value.ToString();
            }
        }

        private async void DownloadBtn_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Title = "Save As";
                dialog.FileName = Picked.original_filename;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        await DownloadFileHelper.Download(Picked.file_id, dialog.FileName);
                        MessageBox.Show("Download complete!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Download failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
