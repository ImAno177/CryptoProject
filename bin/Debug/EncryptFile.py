import requests
from alkindi import KEM
from Crypto.Cipher import AES
from Crypto.Random import get_random_bytes
import sys
import json
import os
import argparse

parser = argparse.ArgumentParser()
parser.add_argument("--username", required=True, help="Username")
parser.add_argument("--password", required=True, help="Password")
parser.add_argument("--file", required=True, help="Path to file to encrypt")
parser.add_argument("--recipients", nargs='+', required=True)
args = parser.parse_args()

USERNAME = args.username
PASSWORD = args.password
FILE_PATH = args.file
RECIPIENTS = args.recipients
SERVER_URL = 'https://fastapi.crypto-lab.cloud'

aes_key = get_random_bytes(32)
nonce = get_random_bytes(12)

with open(FILE_PATH, "rb") as f:
    plaintext = f.read()

cipher = AES.new(aes_key, AES.MODE_GCM, nonce=nonce)
ciphertext, tag = cipher.encrypt_and_digest(plaintext)

filename_only = os.path.basename(FILE_PATH)

upload_resp = requests.post(
    f'{SERVER_URL}/upload-file/',
    auth=(USERNAME, PASSWORD),
    data={'filename': filename_only},
    files={
        'encrypted_file': ('file', ciphertext),
        'nonce_file': ('nonce.bin', nonce),
        'tag_file': ('tag.bin', tag)
    }
)
if upload_resp.status_code != 200:
    print("[!] Upload failed:", upload_resp.status_code, upload_resp.text)
    sys.exit(1)

upload_data = upload_resp.json()
file_id = upload_data['file_id']
print(f"[+] Uploaded file_id: {file_id}")

for recipient in RECIPIENTS:
    print(f"[*] Sharing with {recipient}...")

    getkey_resp = requests.post(
        f'{SERVER_URL}/get-key/',
        auth=(USERNAME, PASSWORD),
        data={
            'user': recipient,
            'nonce_A': get_random_bytes(16).hex()
        }
    )
    if getkey_resp.status_code != 200:
        print(f"[!] Failed to get key for {recipient}: {getkey_resp.status_code} {getkey_resp.text}")
        continue

    kyber_pub = bytes.fromhex(getkey_resp.json()['kyber_pub_hex'])

    try:
        with KEM("ML-KEM-1024") as kem:
            ciphertext_kem, shared_secret = kem.encaps(kyber_pub)
    except Exception as e:
        print(f"[!] Kyber encapsulation failed for {recipient}: {e}")
        continue
    
    ciphertext_kem = bytes(ciphertext_kem)
    shared_secret = bytes(shared_secret)

    wrap_key = shared_secret[:32]
    nonce_kem = get_random_bytes(12)

    cipher2 = AES.new(wrap_key, AES.MODE_GCM, nonce=nonce_kem)
    enc_aes_key, tag2 = cipher2.encrypt_and_digest(aes_key)

    wrapped = ciphertext_kem + enc_aes_key + tag2 + nonce_kem

    share_resp = requests.post(
        f'{SERVER_URL}/share-file/',
        auth=(USERNAME, PASSWORD),
        data={
            'file_id': file_id,
            'to_user': recipient
        },
        files={
            'aes_key_enc': ('aes_key.bin', wrapped)
        }
    )
