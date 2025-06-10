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
        public MainForm()
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

        private void UploadBtn_Click(object sender, EventArgs e)
        {
            UploadPt.Visible = true;
            DownloadPt.Visible = false;
            DetailsPt.Visible = false;
        }

        private void DownloadBtn_Click(object sender, EventArgs e)
        {
            UploadPt.Visible = false;
            DownloadPt.Visible = true;
            DetailsPt.Visible = false;
        }

        private void DetailsBtn_Click(object sender, EventArgs e)
        {
            UploadPt.Visible = false;
            DownloadPt.Visible = false;
            DetailsPt.Visible = true;
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
