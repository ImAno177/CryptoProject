To-Do List: Post-Quantum CRYSTALS-KYBER Encryption for Cloud-native storage
🧩 Giai đoạn 1: Chuẩn bị môi trường
    Cài đặt Docker & Docker Compose trên máy lab
    Cài đặt MinIO (S3-compatible object storage) bằng Docker
        Cổng mặc định: 9000 (API), 9001 (web console)
    Tạo bucket thử nghiệm trên MinIO (ví dụ: encrypted-data)
    Cài Python 3.10+ và các thư viện: requests, pycryptodome, flask, flask-jwt-extended, sqlite3, minio

🔐 Giai đoạn 2: Tích hợp thuật toán Kyber
    Clone thư viện Kyber (PQClean) và build bản C
    Viết Python wrapper với ctypes để sử dụng từ Flask API
    Kiểm thử:
        Sinh Kyber keypair
        Mã hóa AES key bằng Kyber
        Giải mã thành công AES key

🧠 Giai đoạn 3: Xử lý dữ liệu mã hóa
    Sinh AES key ngẫu nhiên (256-bit)
    Mã hóa file bằng AES (GCM hoặc CBC)
    Mã hóa AES key bằng Kyber
    Lưu:
        Ciphertext file vào MinIO
        Encrypted AES key vào SQLite
        Ghi metadata (file name, size, upload time, uploader, hash, ...) vào SQLite

🌐 Giai đoạn 4: Xây dựng RESTful API
    Auth & Session Layer:
        Tạo route POST /login – trả JWT token nếu đúng user/pass
        Middleware Flask kiểm tra JWT token ở các route upload/download
        Ghi session login thành công vào SQLite (timestamp, IP)
    Upload/Download Layer
        POST /upload:
            Yêu cầu header: Authorization: Bearer <JWT>
            Xử lý mã hóa, lưu file vào MinIO
            Lưu metadata & encrypted key vào SQLite
        GET /download/<file_id>:
            Yêu cầu token
            Giải mã AES key từ Kyber
            Tải file, giải mã, trả về cho client
    Metadata API
        GET /files: Lấy danh sách file đã upload (theo user)
        GET /file/<id>: Trả thông tin metadata

🗃️ Giai đoạn 5: Database – SQLite
    Tạo SQLite database storage.db gồm 3 bảng:
        users(id, username, password_hash, salt)
        sessions(id, username, login_time, ip, user_agent)
        files(id, filename, size, uploader, upload_time, encrypted_aes_key, hash)
    Ghi log login mỗi phiên
    Ghi metadata file khi upload thành công

📦 Giai đoạn 6: Đóng gói hệ thống
    Tạo Dockerfile cho Flask API
    Viết docker-compose.yml:
        Flask API
        MinIO
        SQLite

🧪 Giai đoạn 7: Kiểm thử và đánh giá
    Upload file với token → kiểm tra mã hóa
    Download file với token → kiểm tra giải mã
    Kiểm tra log session
    Kiểm tra dữ liệu lưu trong SQLite
    So sánh thời gian mã hóa/giải mã

📚 Giai đoạn 8: Báo cáo lab
    Viết báo cáo chia rõ:
    Mục tiêu
    Kiến trúc hệ thống
    API & GUI flow
    Kết quả & log thực nghiệm
    Vẽ sơ đồ hệ thống (Mermaid/draw.io):
        Người dùng → WinForms → Flask API → Kyber/AES → MinIO/SQLite
    Đưa ảnh GUI, đoạn mã tiêu biểu, bảng metadata mẫu