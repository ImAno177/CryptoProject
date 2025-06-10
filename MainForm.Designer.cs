namespace Crypto
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.DraggingSpace = new System.Windows.Forms.PictureBox();
            this.MainPanel = new System.Windows.Forms.Panel();
            this.HomeLabel = new System.Windows.Forms.Label();
            this.UploadBtn = new System.Windows.Forms.Label();
            this.DownloadBtn = new System.Windows.Forms.Label();
            this.DetailsBtn = new System.Windows.Forms.Label();
            this.UploadPt = new System.Windows.Forms.PictureBox();
            this.DownloadPt = new System.Windows.Forms.PictureBox();
            this.DetailsPt = new System.Windows.Forms.PictureBox();
            this.ExitButton = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.DraggingSpace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UploadPt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DownloadPt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DetailsPt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ExitButton)).BeginInit();
            this.SuspendLayout();
            // 
            // DraggingSpace
            // 
            this.DraggingSpace.Location = new System.Drawing.Point(0, 0);
            this.DraggingSpace.Name = "DraggingSpace";
            this.DraggingSpace.Size = new System.Drawing.Size(1134, 50);
            this.DraggingSpace.TabIndex = 0;
            this.DraggingSpace.TabStop = false;
            // 
            // MainPanel
            // 
            this.MainPanel.Location = new System.Drawing.Point(269, 69);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(931, 730);
            this.MainPanel.TabIndex = 1;
            // 
            // HomeLabel
            // 
            this.HomeLabel.AutoSize = true;
            this.HomeLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 22.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HomeLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.HomeLabel.Location = new System.Drawing.Point(12, 69);
            this.HomeLabel.Name = "HomeLabel";
            this.HomeLabel.Size = new System.Drawing.Size(134, 50);
            this.HomeLabel.TabIndex = 2;
            this.HomeLabel.Text = "Home";
            // 
            // UploadBtn
            // 
            this.UploadBtn.AutoSize = true;
            this.UploadBtn.Font = new System.Drawing.Font("Microsoft YaHei UI", 22.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UploadBtn.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.UploadBtn.Location = new System.Drawing.Point(51, 141);
            this.UploadBtn.Name = "UploadBtn";
            this.UploadBtn.Size = new System.Drawing.Size(157, 50);
            this.UploadBtn.TabIndex = 3;
            this.UploadBtn.Text = "Upload";
            this.UploadBtn.Click += new System.EventHandler(this.UploadBtn_Click);
            // 
            // DownloadBtn
            // 
            this.DownloadBtn.AutoSize = true;
            this.DownloadBtn.Font = new System.Drawing.Font("Microsoft YaHei UI", 22.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DownloadBtn.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.DownloadBtn.Location = new System.Drawing.Point(51, 215);
            this.DownloadBtn.Name = "DownloadBtn";
            this.DownloadBtn.Size = new System.Drawing.Size(212, 50);
            this.DownloadBtn.TabIndex = 4;
            this.DownloadBtn.Text = "Download";
            this.DownloadBtn.Click += new System.EventHandler(this.DownloadBtn_Click);
            // 
            // DetailsBtn
            // 
            this.DetailsBtn.AutoSize = true;
            this.DetailsBtn.Font = new System.Drawing.Font("Microsoft YaHei UI", 22.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DetailsBtn.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.DetailsBtn.Location = new System.Drawing.Point(51, 287);
            this.DetailsBtn.Name = "DetailsBtn";
            this.DetailsBtn.Size = new System.Drawing.Size(149, 50);
            this.DetailsBtn.TabIndex = 5;
            this.DetailsBtn.Text = "Details";
            this.DetailsBtn.Click += new System.EventHandler(this.DetailsBtn_Click);
            // 
            // UploadPt
            // 
            this.UploadPt.Image = global::Crypto.Properties.Resources.pointer;
            this.UploadPt.Location = new System.Drawing.Point(12, 141);
            this.UploadPt.Name = "UploadPt";
            this.UploadPt.Size = new System.Drawing.Size(40, 40);
            this.UploadPt.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.UploadPt.TabIndex = 6;
            this.UploadPt.TabStop = false;
            // 
            // DownloadPt
            // 
            this.DownloadPt.Image = global::Crypto.Properties.Resources.pointer;
            this.DownloadPt.Location = new System.Drawing.Point(12, 215);
            this.DownloadPt.Name = "DownloadPt";
            this.DownloadPt.Size = new System.Drawing.Size(40, 40);
            this.DownloadPt.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.DownloadPt.TabIndex = 7;
            this.DownloadPt.TabStop = false;
            this.DownloadPt.Visible = false;
            // 
            // DetailsPt
            // 
            this.DetailsPt.Image = global::Crypto.Properties.Resources.pointer;
            this.DetailsPt.Location = new System.Drawing.Point(12, 287);
            this.DetailsPt.Name = "DetailsPt";
            this.DetailsPt.Size = new System.Drawing.Size(40, 40);
            this.DetailsPt.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.DetailsPt.TabIndex = 8;
            this.DetailsPt.TabStop = false;
            this.DetailsPt.Visible = false;
            // 
            // ExitButton
            // 
            this.ExitButton.Image = global::Crypto.Properties.Resources.Exit;
            this.ExitButton.Location = new System.Drawing.Point(1150, 0);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(50, 50);
            this.ExitButton.TabIndex = 9;
            this.ExitButton.TabStop = false;
            this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // MainForm
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.Controls.Add(this.ExitButton);
            this.Controls.Add(this.DetailsPt);
            this.Controls.Add(this.DownloadPt);
            this.Controls.Add(this.UploadPt);
            this.Controls.Add(this.DetailsBtn);
            this.Controls.Add(this.DownloadBtn);
            this.Controls.Add(this.UploadBtn);
            this.Controls.Add(this.HomeLabel);
            this.Controls.Add(this.MainPanel);
            this.Controls.Add(this.DraggingSpace);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Home";
            ((System.ComponentModel.ISupportInitialize)(this.DraggingSpace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UploadPt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DownloadPt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DetailsPt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ExitButton)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox DraggingSpace;
        private System.Windows.Forms.Panel MainPanel;
        private System.Windows.Forms.Label HomeLabel;
        private System.Windows.Forms.Label UploadBtn;
        private System.Windows.Forms.Label DownloadBtn;
        private System.Windows.Forms.Label DetailsBtn;
        private System.Windows.Forms.PictureBox UploadPt;
        private System.Windows.Forms.PictureBox DownloadPt;
        private System.Windows.Forms.PictureBox DetailsPt;
        private System.Windows.Forms.PictureBox ExitButton;
    }
}