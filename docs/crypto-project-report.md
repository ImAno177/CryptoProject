# Post-Quantum Secure File Sharing - Public Report

This is a public-safe summary of the UIT Cryptography course project. The historical demo endpoints (`fastapi.crypto-lab.cloud` and `minio-ui.crypto-lab.cloud`) and demo accounts (`admin`, `user1`-`user3`) were revoked after the demonstration. Passwords, private keys, and student identifiers are intentionally omitted here.

## Project brief

The system models authenticated internal file sharing over the internet. A user can upload a file, share it with selected recipients, and download it only when the server-side access-control list permits the request.

## Security goals

- **Transport security:** protect client-to-service traffic with HTTPS.
- **Key authenticity:** register public keys with a post-quantum signature and prevent silent key replacement.
- **Access control:** allow downloads only for the file owner or an explicitly authorized recipient.
- **Confidentiality and integrity:** encrypt file content with AES-GCM and protect the file key for each recipient.

## System design

- **Windows client:** C# WinForms application for login, key registration, upload, sharing, listing, and download.
- **Authenticated API:** handles user authentication, key registration, file metadata, sharing, and ACL checks.
- **Key and metadata store:** keeps user records, public keys, file ownership, and sharing metadata.
- **Object storage:** stores encrypted file data and recipient-specific wrapped AES keys.
- **TLS edge:** the original deployment used a reverse proxy and certificate-based HTTPS between the client and service.

## Cryptographic flow

1. The client generates ML-KEM-1024 and ML-DSA-44 key pairs and signs the ML-KEM public-key registration.
2. The client generates a fresh AES-256 key and encrypts the file with AES-GCM, retaining the nonce and authentication tag.
3. For sharing, the AES key is encapsulated for the recipient's ML-KEM public key and associated with the file ACL.
4. On download, the API checks the owner/recipient ACL. The authorized client decapsulates the AES key and verifies/decrypts the ciphertext locally.

## Demonstration

The [demo video](crypto-project-demo.mp4) covers the Windows client workflow from login and key registration through encrypted upload, sharing, authorization checks, and download.

## Reproduction notes

The client targets .NET Framework 4.7.2 and uses Python helpers for cryptographic operations and HTTP requests. The backend, deployment configuration, credentials, and private service endpoints are not part of this public repository.

## Limitations

This is a course prototype. A production system would need an independent security review, managed key storage, credential rotation, hardened server-side authorization, dependency maintenance, and a documented recovery process.
