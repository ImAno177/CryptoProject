namespace crypto_lab
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtServerUrl = new TextBox();
            btnDownload = new Button();
            dgvFiles = new DataGridView();
            btnSelectUploadFile = new Button();
            btnUpload = new Button();
            btnListFiles = new Button();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            lblSelectedUploadFile = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvFiles).BeginInit();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // txtServerUrl
            // 
            txtServerUrl.Location = new Point(31, 34);
            txtServerUrl.Name = "txtServerUrl";
            txtServerUrl.Size = new Size(1340, 49);
            txtServerUrl.TabIndex = 0;
            txtServerUrl.TextChanged += txtServerUrl_TextChanged;
            // 
            // btnDownload
            // 
            btnDownload.Location = new Point(1418, 225);
            btnDownload.Name = "btnDownload";
            btnDownload.Size = new Size(169, 52);
            btnDownload.TabIndex = 1;
            btnDownload.Text = "download";
            btnDownload.UseVisualStyleBackColor = true;
            btnDownload.Click += btnDownload_Click;
            // 
            // dgvFiles
            // 
            dgvFiles.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvFiles.Location = new Point(0, 304);
            dgvFiles.Name = "dgvFiles";
            dgvFiles.RowHeadersWidth = 92;
            dgvFiles.Size = new Size(1614, 629);
            dgvFiles.TabIndex = 2;
            // 
            // btnSelectUploadFile
            // 
            btnSelectUploadFile.Location = new Point(1202, 124);
            btnSelectUploadFile.Name = "btnSelectUploadFile";
            btnSelectUploadFile.Size = new Size(169, 52);
            btnSelectUploadFile.TabIndex = 3;
            btnSelectUploadFile.Text = "Select File";
            btnSelectUploadFile.UseVisualStyleBackColor = true;
            btnSelectUploadFile.Click += btnSelectUploadFile_Click;
            // 
            // btnUpload
            // 
            btnUpload.Location = new Point(1418, 133);
            btnUpload.Name = "btnUpload";
            btnUpload.Size = new Size(169, 52);
            btnUpload.TabIndex = 4;
            btnUpload.Text = "Upload";
            btnUpload.UseVisualStyleBackColor = true;
            btnUpload.Click += btnUpload_Click;
            // 
            // btnListFiles
            // 
            btnListFiles.Location = new Point(1418, 31);
            btnListFiles.Name = "btnListFiles";
            btnListFiles.Size = new Size(169, 52);
            btnListFiles.TabIndex = 5;
            btnListFiles.Text = "List file";
            btnListFiles.UseVisualStyleBackColor = true;
            btnListFiles.Click += btnListFiles_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(36, 36);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 955);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1614, 22);
            statusStrip1.TabIndex = 6;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(0, 11);
            // 
            // lblSelectedUploadFile
            // 
            lblSelectedUploadFile.AutoSize = true;
            lblSelectedUploadFile.Location = new Point(31, 129);
            lblSelectedUploadFile.Name = "lblSelectedUploadFile";
            lblSelectedUploadFile.Size = new Size(0, 42);
            lblSelectedUploadFile.TabIndex = 8;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(17F, 42F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1614, 977);
            Controls.Add(lblSelectedUploadFile);
            Controls.Add(statusStrip1);
            Controls.Add(btnListFiles);
            Controls.Add(btnUpload);
            Controls.Add(btnSelectUploadFile);
            Controls.Add(dgvFiles);
            Controls.Add(btnDownload);
            Controls.Add(txtServerUrl);
            Name = "MainForm";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)dgvFiles).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtServerUrl;
        private Button btnDownload;
        private DataGridView dgvFiles;
        private Button btnSelectUploadFile;
        private Button btnUpload;
        private Button btnListFiles;
        private StatusStrip statusStrip1;
        private Label lblSelectedUploadFile;
        private ToolStripStatusLabel toolStripStatusLabel1;
    }
}
