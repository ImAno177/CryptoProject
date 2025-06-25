import requests
from alkindi import KEM
from Crypto.Cipher import AES
import argparse
import sys

parser = argparse.ArgumentParser()
parser.add_argument("--username", required=True)
parser.add_argument("--password", required=True)
parser.add_argument("--file_id", required=True)
parser.add_argument("--output_filepath", required=True)
parser.add_argument("--output_filename", required=True)
parser.add_argument("--kyber_secret_hex", required=True)
args = parser.parse_args()

USERNAME = args.username
PASSWORD = args.password
FILE_ID = args.file_id
KYBER_SECRET_HEX = args.kyber_secret_hex
SERVER_URL = 'https://fastapi.crypto-lab.cloud'

resp = requests.get(
    f"{SERVER_URL}/download-file/{FILE_ID}",
    auth=(USERNAME, PASSWORD)
)
if resp.status_code != 200:
    print(f"[!] Failed to download: {resp.status_code} {resp.text}")
    sys.exit(1)

raw = resp.content

C_KEM_LEN = 1568
ENC_AES_KEY_LEN = 32
TAG2_LEN = 16
NONCE_KEM_LEN = 12
AES_KEY_ENC_TOTAL = C_KEM_LEN + ENC_AES_KEY_LEN + TAG2_LEN + NONCE_KEM_LEN
TAG_LEN = 16
NONCE_LEN = 12

aes_key_enc_bundle = raw[-AES_KEY_ENC_TOTAL:]
tag = raw[-(AES_KEY_ENC_TOTAL + TAG_LEN):-AES_KEY_ENC_TOTAL]
nonce = raw[-(AES_KEY_ENC_TOTAL + TAG_LEN + NONCE_LEN):-(AES_KEY_ENC_TOTAL + TAG_LEN)]
ciphertext = raw[:-(AES_KEY_ENC_TOTAL + TAG_LEN + NONCE_LEN)]

ciphertext_kem = aes_key_enc_bundle[:C_KEM_LEN]
enc_aes_key     = aes_key_enc_bundle[C_KEM_LEN:C_KEM_LEN + ENC_AES_KEY_LEN]
tag2            = aes_key_enc_bundle[C_KEM_LEN + ENC_AES_KEY_LEN : C_KEM_LEN + ENC_AES_KEY_LEN + TAG2_LEN]
nonce_kem       = aes_key_enc_bundle[-NONCE_KEM_LEN:]

kyber_secret = bytes.fromhex(KYBER_SECRET_HEX)
with KEM("ML-KEM-1024") as kem:
    shared_secret = kem.decaps(ciphertext_kem, kyber_secret)

wrap_key = shared_secret[:32]

cipher2 = AES.new(wrap_key, AES.MODE_GCM, nonce=nonce_kem)
aes_key = cipher2.decrypt_and_verify(enc_aes_key, tag2)

cipher = AES.new(aes_key, AES.MODE_GCM, nonce=nonce)
plaintext = cipher.decrypt_and_verify(ciphertext, tag)

output_path = f"{args.output_filepath}"
with open(output_path, "wb") as f:
    f.write(plaintext)

print(f"[+] Decryption complete. Saved to {output_path}")
