using System.Drawing;
using System.Windows.Forms;

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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.FileList = new System.Windows.Forms.DataGridView();
            this.FileIDHeader = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FilenameCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SizeCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UploadAtCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DownloadBtn = new System.Windows.Forms.Button();
            this.UploadBtn = new System.Windows.Forms.Button();
            this.ExitButton = new System.Windows.Forms.PictureBox();
            this.DraggingSpace = new System.Windows.Forms.PictureBox();
            this.F5Button = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.FileList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ExitButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DraggingSpace)).BeginInit();
            this.SuspendLayout();
            // 
            // FileList
            // 
            this.FileList.AllowUserToResizeRows = false;
            this.FileList.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.FileList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.FileList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.FileList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FileIDHeader,
            this.FilenameCol,
            this.SizeCol,
            this.UploadAtCol});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(254)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.FileList.DefaultCellStyle = dataGridViewCellStyle2;
            this.FileList.GridColor = System.Drawing.Color.Gainsboro;
            this.FileList.Location = new System.Drawing.Point(12, 69);
            this.FileList.Name = "FileList";
            this.FileList.RowHeadersVisible = false;
            this.FileList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.FileList.Size = new System.Drawing.Size(1020, 719);
            this.FileList.TabIndex = 14;
            // 
            // FileIDHeader
            // 
            this.FileIDHeader.HeaderText = "FileID";
            this.FileIDHeader.Name = "FileIDHeader";
            this.FileIDHeader.ReadOnly = true;
            this.FileIDHeader.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.FileIDHeader.Width = 400;
            // 
            // FilenameCol
            // 
            this.FilenameCol.HeaderText = "Filename";
            this.FilenameCol.Name = "FilenameCol";
            this.FilenameCol.ReadOnly = true;
            this.FilenameCol.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.FilenameCol.Width = 250;
            // 
            // SizeCol
            // 
            this.SizeCol.HeaderText = "Size";
            this.SizeCol.Name = "SizeCol";
            this.SizeCol.ReadOnly = true;
            this.SizeCol.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SizeCol.Width = 170;
            // 
            // UploadAtCol
            // 
            this.UploadAtCol.HeaderText = "Upload At";
            this.UploadAtCol.Name = "UploadAtCol";
            this.UploadAtCol.ReadOnly = true;
            this.UploadAtCol.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.UploadAtCol.Width = 197;
            // 
            // DownloadBtn
            // 
            this.DownloadBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DownloadBtn.Location = new System.Drawing.Point(1049, 232);
            this.DownloadBtn.Name = "DownloadBtn";
            this.DownloadBtn.Size = new System.Drawing.Size(139, 57);
            this.DownloadBtn.TabIndex = 11;
            this.DownloadBtn.Text = "Download";
            this.DownloadBtn.UseVisualStyleBackColor = true;
            this.DownloadBtn.Click += new System.EventHandler(this.DownloadBtn_Click);
            // 
            // UploadBtn
            // 
            this.UploadBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UploadBtn.Location = new System.Drawing.Point(1049, 378);
            this.UploadBtn.Name = "UploadBtn";
            this.UploadBtn.Size = new System.Drawing.Size(139, 57);
            this.UploadBtn.TabIndex = 12;
            this.UploadBtn.Text = "Upload";
            this.UploadBtn.UseVisualStyleBackColor = true;
            this.UploadBtn.Click += new System.EventHandler(this.UploadBtn_Click);
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
            // DraggingSpace
            // 
            this.DraggingSpace.Location = new System.Drawing.Point(0, 0);
            this.DraggingSpace.Name = "DraggingSpace";
            this.DraggingSpace.Size = new System.Drawing.Size(1134, 50);
            this.DraggingSpace.TabIndex = 0;
            this.DraggingSpace.TabStop = false;
            // 
            // F5Button
            // 
            this.F5Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.F5Button.Location = new System.Drawing.Point(1049, 69);
            this.F5Button.Name = "F5Button";
            this.F5Button.Size = new System.Drawing.Size(139, 40);
            this.F5Button.TabIndex = 13;
            this.F5Button.Text = "Refresh";
            this.F5Button.UseVisualStyleBackColor = true;
            this.F5Button.Click += new System.EventHandler(this.F5Button_Click);
            // 
            // MainForm
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.Controls.Add(this.F5Button);
            this.Controls.Add(this.UploadBtn);
            this.Controls.Add(this.DownloadBtn);
            this.Controls.Add(this.FileList);
            this.Controls.Add(this.ExitButton);
            this.Controls.Add(this.DraggingSpace);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Home";
            ((System.ComponentModel.ISupportInitialize)(this.FileList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ExitButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DraggingSpace)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox DraggingSpace;
        private System.Windows.Forms.PictureBox ExitButton;
        private System.Windows.Forms.DataGridView FileList;
        private System.Windows.Forms.Button DownloadBtn;
        private System.Windows.Forms.Button UploadBtn;
        private System.Windows.Forms.Button F5Button;
        private DataGridViewTextBoxColumn FileIDHeader;
        private DataGridViewTextBoxColumn FilenameCol;
        private DataGridViewTextBoxColumn SizeCol;
        private DataGridViewTextBoxColumn UploadAtCol;
    }
}