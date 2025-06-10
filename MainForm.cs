namespace crypto_lab
{
    public partial class MainForm : Form
    {
        private ApiClient _apiClient;
        private string? _selectedFileForUploadPath;
        private byte[]? _clientKyberPrivateKeyForDownload;
        public MainForm()
        {
            InitializeComponent();
            if (txtServerUrl.Text == string.Empty)
            {
                txtServerUrl.Text = "https://fastapi.crypto-lab.cloud"; // Ví dụ URL mặc định
            }
            _apiClient = new ApiClient(txtServerUrl.Text);
        }

        private void txtServerUrl_TextChanged(object sender, EventArgs e)
        {
            if (_apiClient != null && !string.IsNullOrWhiteSpace(txtServerUrl.Text))
            {
                _apiClient.SetBaseUrl(txtServerUrl.Text);
                UpdateStatus($"Server URL updated to: {txtServerUrl.Text}");
            }
        }
        private void UpdateStatus(string message)
        {
            if (statusStrip1.InvokeRequired)
            {
                statusStrip1.Invoke(new Action<string>(UpdateStatus), message);
            }
            else
            {
                toolStripStatusLabel1.Text = message;
                Console.WriteLine($"Status: {message}"); // Ghi log ra Console để debug
            }
        }

        private async void btnListFiles_Click(object? sender, EventArgs? e)
        {
            try
            {
                UpdateStatus("Đang lấy danh sách file...");
                var files = await _apiClient.ListFilesAsync();
                dgvFiles.DataSource = files; // Gán danh sách file cho DataGridView
                                             // Cấu hình các cột cho dgvFiles nếu cần
                if (dgvFiles.Columns["FileId"] != null) dgvFiles.Columns["FileId"]!.HeaderText = "File ID";
                if (dgvFiles.Columns["OriginalFilename"] != null) dgvFiles.Columns["OriginalFilename"]!.HeaderText = "Tên File Gốc";
                if (dgvFiles.Columns["Size"] != null) dgvFiles.Columns["Size"]!.HeaderText = "Kích Thước (Bytes)";
                if (dgvFiles.Columns["UploadedAt"] != null) dgvFiles.Columns["UploadedAt"]!.HeaderText = "Ngày Tải Lên";

                UpdateStatus($"Tìm thấy {files.Count} file.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lấy danh sách file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus("Lỗi khi lấy danh sách file.");
            }
        }

        private void btnSelectUploadFile_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Chọn file để tải lên";
                ofd.Filter = "All files (*.*)|*.*"; // Bộ lọc file
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _selectedFileForUploadPath = ofd.FileName;
                    lblSelectedUploadFile.Text = Path.GetFileName(_selectedFileForUploadPath);
                    UpdateStatus($"Đã chọn file: {lblSelectedUploadFile.Text}");
                }
            }
        }

        private async void btnUpload_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedFileForUploadPath))
            {
                MessageBox.Show("Vui lòng chọn một file để tải lên.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string tempEncryptedFilePath = Path.GetTempFileName(); // File tạm để lưu file đã mã hóa
            try
            {
                UpdateStatus("Bắt đầu quá trình tải lên...");
                // 1. Client: Lấy Kyber public key (pk_s) từ Server
                UpdateStatus("Đang lấy public key từ server...");
                var initiateResponse = await _apiClient.InitiateUploadAsync();
                var serverKyberPkBytes = CryptoService.HexConverter.FromHexString(initiateResponse.ServerKyberPkHex);

                // 2. Client: Tạo khóa AES (ss1), nonce (n1)
                // Khóa AES (ss1) được tạo bằng cách đóng gói (encapsulate) với pk_s của server.
                UpdateStatus("Đang tạo khóa phiên và đóng gói...");
                var (encapsulatedAesKeyBytes, aesKeyForFileEncryption) = CryptoService.EncapsulateKyber(serverKyberPkBytes);
                var aesNonce = CryptoService.GenerateAesNonce(); // Nonce cho mã hóa file

                // 3. Client: Mã hóa file bằng AES-GCM (ss1, n1) -> file_encrypted, tag (t1)
                UpdateStatus("Đang mã hóa file...");
                byte[] aesTag;
                using (var inputFileStream = File.OpenRead(_selectedFileForUploadPath))
                using (var tempEncryptedFileStream = File.OpenWrite(tempEncryptedFilePath))
                {
                    aesTag = CryptoService.EncryptStreamAesGcm(inputFileStream, tempEncryptedFileStream, aesKeyForFileEncryption, aesNonce);
                }

                // 4. Client: Tính SHA256 checksum của file gốc (plaintext_sha256_checksum_hex)
                UpdateStatus("Đang tính checksum file gốc...");
                string sha256ChecksumOriginal = await CryptoService.CalculateSha256ChecksumAsync(_selectedFileForUploadPath);

                // 5. Client: Gửi session_id, ct1 (encapsulated_aes_key_hex), n1 (aes_nonce_hex), 
                //    t1 (aes_tag_hex), plaintext_sha256_checksum_hex, và file_encrypted lên Server.
                UpdateStatus("Đang tải file lên server...");
                var uploadResponse = await _apiClient.UploadFileAsync(
                    initiateResponse.SessionId,
                    CryptoService.HexConverter.ToHexString(encapsulatedAesKeyBytes),
                    CryptoService.HexConverter.ToHexString(aesNonce),
                    CryptoService.HexConverter.ToHexString(aesTag),
                    sha256ChecksumOriginal,
                    tempEncryptedFilePath, // Đường dẫn đến file ĐÃ MÃ HÓA TẠM THỜI
                    Path.GetFileName(_selectedFileForUploadPath)
                );

                UpdateStatus($"Tải lên thành công! File ID: {uploadResponse.FileId}");
                MessageBox.Show($"File '{Path.GetFileName(_selectedFileForUploadPath)}' đã được tải lên thành công.\nFile ID: {uploadResponse.FileId}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Làm mới danh sách file
                btnListFiles_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi trong quá trình tải lên: {ex.Message}\nChi tiết: {ex.InnerException?.Message}\nStack Trace: {ex.StackTrace}", "Lỗi Tải Lên", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus("Tải lên thất bại.");
            }
            finally
            {
                // Xóa file mã hóa tạm
                if (File.Exists(tempEncryptedFilePath))
                {
                    try { File.Delete(tempEncryptedFilePath); }
                    catch (IOException ioEx) { Console.WriteLine($"Không thể xóa file tạm: {ioEx.Message}"); }
                }
                _selectedFileForUploadPath = null; // Reset
                lblSelectedUploadFile.Text = "Chưa chọn file";
            }
        }

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            if (dgvFiles.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một file từ danh sách để tải xuống.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var selectedFileItem = dgvFiles.SelectedRows[0].DataBoundItem as FileListItem;
            if (selectedFileItem == null)
            {
                MessageBox.Show("Không thể xác định thông tin file đã chọn.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            string fileIdToDownload = selectedFileItem.FileId;
            string originalFileName = selectedFileItem.OriginalFilename;

            string tempDownloadedEncryptedPath = Path.GetTempFileName(); // File tạm để lưu file mã hóa tải về
            string saveFilePath; // Đường dẫn người dùng chọn để lưu file giải mã

            using (var sfd = new SaveFileDialog())
            {
                sfd.FileName = originalFileName; // Tên file gợi ý
                sfd.Title = "Chọn nơi lưu file đã giải mã";
                sfd.Filter = "All files (*.*)|*.*";
                if (sfd.ShowDialog() != DialogResult.OK)
                {
                    UpdateStatus("Hủy tải xuống.");
                    return;
                }
                saveFilePath = sfd.FileName;
            }

            try
            {
                UpdateStatus($"Bắt đầu tải xuống '{originalFileName}'...");
                // 1. Client: Tạo cặp khóa Kyber (pk_c, sk_c)
                UpdateStatus("Đang tạo cặp khóa Kyber cho client...");
                var (clientPublicKeyBytes, clientPrivateKeyBytes) = CryptoService.GenerateKyberKeyPair();
                _clientKyberPrivateKeyForDownload = clientPrivateKeyBytes; // Lưu khóa bí mật để dùng sau

                // 2. Client: Gửi pk_c (client_kyber_pk_hex) cho Server
                UpdateStatus("Đang gửi public key của client đến server...");
                var downloadInitiateResponse = await _apiClient.InitiateDownloadAsync(
                    fileIdToDownload,
                    CryptoService.HexConverter.ToHexString(clientPublicKeyBytes)
                );

                // Server trả về: download_token, kyber_encapsulated_aes_key_hex (ct_s), 
                // aes_nonce_hex (n_s), aes_tag_hex (t_s)
                var serverEncapsulatedAesKeyBytes = CryptoService.HexConverter.FromHexString(downloadInitiateResponse.KyberEncapsulatedAesKeyHex);
                var aesNonceFromServer = CryptoService.HexConverter.FromHexString(downloadInitiateResponse.AesNonceHex);
                var aesTagFromServer = CryptoService.HexConverter.FromHexString(downloadInitiateResponse.AesTagHex);

                // 3. Client: Dùng sk_c để mở gói ct_s -> lấy khóa AES (ss_s)
                UpdateStatus("Đang mở gói để lấy khóa AES phiên...");
                var aesKeyForDecryption = CryptoService.DecapsulateKyber(_clientKyberPrivateKeyForDownload, serverEncapsulatedAesKeyBytes);

                // 4. Client: Download file đã được server mã hóa lại (/download/{download_token})
                UpdateStatus("Đang tải file (đã được server mã hóa lại)...");
                await _apiClient.DownloadFileStreamAsync(downloadInitiateResponse.DownloadToken, tempDownloadedEncryptedPath);

                // 5. Client: Giải mã file đã download (dùng ss_s, n_s, t_s)
                UpdateStatus("Đang giải mã file...");
                // Tạo một MemoryStream chứa (file tải về + tag) để DecryptStreamAesGcm xử lý
                using (var combinedStream = new MemoryStream())
                {
                    using (var encryptedFileStream = File.OpenRead(tempDownloadedEncryptedPath))
                    {
                        await encryptedFileStream.CopyToAsync(combinedStream); // Sao chép ciphertext
                    }
                    combinedStream.Write(aesTagFromServer, 0, aesTagFromServer.Length); // Nối tag vào cuối
                    combinedStream.Position = 0; // Reset vị trí stream để đọc từ đầu

                    using (var decryptedFileStream = File.OpenWrite(saveFilePath)) // File đích đã giải mã
                    {
                        // Hàm DecryptStreamAesGcm của chúng ta mong đợi stream đầu vào chứa (ciphertext + tag)
                        CryptoService.DecryptStreamAesGcm(combinedStream, decryptedFileStream, aesKeyForDecryption, aesNonceFromServer);
                    }
                }

                UpdateStatus($"File '{originalFileName}' đã được tải xuống và giải mã thành công tại: {saveFilePath}");
                MessageBox.Show($"File '{originalFileName}' đã được tải xuống và giải mã thành công!", "Hoàn Tất Tải Xuống", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi trong quá trình tải xuống: {ex.Message}\nChi tiết: {ex.InnerException?.Message}\nStack Trace: {ex.StackTrace}", "Lỗi Tải Xuống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus("Tải xuống thất bại.");
                // Xóa file giải mã dở nếu có lỗi
                if (File.Exists(saveFilePath))
                {
                    try { File.Delete(saveFilePath); } catch { /* Bỏ qua lỗi xóa */ }
                }
            }
            finally
            {
                // Xóa file mã hóa tạm đã tải về
                if (File.Exists(tempDownloadedEncryptedPath))
                {
                    try { File.Delete(tempDownloadedEncryptedPath); }
                    catch (IOException ioEx) { Console.WriteLine($"Không thể xóa file tạm tải về: {ioEx.Message}"); }
                }
                _clientKyberPrivateKeyForDownload = null; // Xóa khóa bí mật sau khi dùng
            }
        }  
    }
}
