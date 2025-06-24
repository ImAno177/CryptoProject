import os
import time
import uuid
import bcrypt
from fastapi import FastAPI, Depends, Form, File, UploadFile, HTTPException
from fastapi.responses import StreamingResponse
from fastapi.security import HTTPBasic, HTTPBasicCredentials
from supabase import create_client
from minio import Minio
from alkindi import Signature
from dotenv import load_dotenv
load_dotenv()
# --- Configuration ---
SUPABASE_URL    = os.getenv('SUPABASE_URL')
SUPABASE_KEY    = os.getenv('SUPABASE_KEY')

MINIO_ENDPOINT   = os.getenv('MINIO_ENDPOINT')
MINIO_ACCESS_KEY = os.getenv('MINIO_ACCESS_KEY')
MINIO_SECRET_KEY = os.getenv('MINIO_SECRET_KEY')
MINIO_BUCKET     = os.getenv('MINIO_BUCKET')

minio_secure_str = os.getenv('MINIO_SECURE', 'False') # Mặc định là 'False' nếu không tìm thấy
MINIO_SECURE     = minio_secure_str.lower() in ('true', '1', 't', 'yes')

SERVER_PRIV = bytes.fromhex(os.getenv('SERVER_DILITHIUM_PRIV_HEX', ''))
SERVER_PUB  = bytes.fromhex(os.getenv('SERVER_DILITHIUM_PUB_HEX', ''))

# --- Init clients ---
supabase = create_client(SUPABASE_URL, SUPABASE_KEY)
minio = Minio(
    MINIO_ENDPOINT,
    access_key=MINIO_ACCESS_KEY,
    secret_key=MINIO_SECRET_KEY,
    secure=MINIO_SECURE
)
app = FastAPI()
security = HTTPBasic()

# --- Authentication ---
def verify_user(creds: HTTPBasicCredentials = Depends(security)) -> str:
    username = creds.username
    password = creds.password
    r = supabase.table('users').select('password_hash') \
        .eq('username', username).single().execute()
    if not r.data or not bcrypt.checkpw(password.encode(), r.data['password_hash'].encode()):
        raise HTTPException(status_code=401, detail='Not authenticated')
    return username

# --- Dilithium verify helper ---
def verify_dilithium(pub: bytes, dp: bytes, sg: bytes) -> bool:
    with Signature('ML-DSA-44') as s:
        return s.verify(pub, sg, dp)

# --- Endpoint: Register Kyber & Dilithium public key ---
@app.post('/register-keys/')
async def register_keys(
    ky_pub_hex: str = Form(...),
    dil_pub_hex: str = Form(...),
    ky_sig_hex: str = Form(...),
    current_user: str = Depends(verify_user)
):
    ky = bytes.fromhex(ky_pub_hex)
    dp = bytes.fromhex(dil_pub_hex)
    sg = bytes.fromhex(ky_sig_hex)
    if not verify_dilithium(ky, dp, sg):
        raise HTTPException(status_code=400, detail='Invalid signature')
    resp = supabase.table("users_keys") \
        .select("kyber_pub_hex") \
        .eq("username", current_user) \
        .limit(1) \
        .execute()

    if resp.data:  
        return {
            "message": "Already registered",
            "kyber_pub_hex": resp.data[0]["kyber_pub_hex"]
    }

    # insert KEYS
    supabase.table('users_keys').insert({
        'username': current_user,
        'kyber_pub_hex': ky_pub_hex,
        'dilithium_pub_hex': dil_pub_hex,
        'kyber_sig_hex': ky_sig_hex
    }).execute()
    return {'message': 'registered'}

# --- Endpoint: Upload file (AES‑GCM ciphertext, nonce, tag) ---
@app.post('/upload-file/')
async def upload_file(
    filename: str = Form(...),
    encrypted_file: UploadFile = File(...),
    nonce_file: UploadFile = File(...),
    tag_file: UploadFile = File(...),
    current_user: str = Depends(verify_user)
):
    fid = str(uuid.uuid4())
    # Initialize shared_with as empty array if missing
    supabase.table('files').insert({
        'file_id': fid,
        'owner': current_user,
        'filename': filename,
        'shared_with': []
    }).execute()
    base = f'file_{fid}/'
    # Upload each part with known length
    for name, up in [('file', encrypted_file), ('nonce.bin', nonce_file), ('tag.bin', tag_file)]:
        up.file.seek(0, os.SEEK_END)
        size = up.file.tell()
        up.file.seek(0)
        minio.put_object(MINIO_BUCKET, base + name, up.file, length=size)
    return {'file_id': fid}

# --- Endpoint: Get shared user's Kyber public key + signature ---
@app.post('/get-key/')
async def get_key(
    user: str = Form(...),
    nonce_A: str = Form(...),
    current_user: str = Depends(verify_user)
):
    # Use maybe_single to avoid exception
    result = supabase.table('users_keys') \
        .select('*') \
        .eq('username', user) \
        .maybe_single() \
        .execute()
    row = result.data
    if not row:
        raise HTTPException(status_code=404, detail='No such key')
    ky = bytes.fromhex(row['kyber_pub_hex'])
    dp = bytes.fromhex(row['dilithium_pub_hex'])
    sg = bytes.fromhex(row['kyber_sig_hex'])
    if not verify_dilithium(ky, dp, sg):
        raise HTTPException(status_code=400, detail='Invalid stored key')
    # Generate response
    nonce_B = os.urandom(16).hex()
    ts = str(int(time.time()))
    payload = (nonce_A + row['kyber_pub_hex'] + nonce_B + ts).encode()
    with Signature('ML-DSA-44') as s:
        sig_srv = bytes(s.sign(payload, SERVER_PRIV)).hex()
    return {
        'kyber_pub_hex': row['kyber_pub_hex'],
        'nonce_A': nonce_A,
        'nonce_B': nonce_B,
        'timestamp': ts,
        'signature': sig_srv
    }

# --- Endpoint: Share file (upload AES key ciphertext for recipient) ---
@app.post('/share-file/')
async def share_file(
    file_id: str = Form(...),
    to_user: str = Form(...),
    aes_key_enc: UploadFile = File(...),
    current_user: str = Depends(verify_user)
):
    md = supabase.table('files').select('*').eq('file_id', file_id).single().execute().data
    if not md or md['owner'] != current_user:
        raise HTTPException(status_code=403, detail='No access')
    # Ensure recipient key exists
    await get_key(user=to_user, nonce_A=os.urandom(16).hex(), current_user=current_user)
    path = f'file_{file_id}/aes_key.{to_user}'
    aes_key_enc.file.seek(0, os.SEEK_END)
    length = aes_key_enc.file.tell()
    aes_key_enc.file.seek(0)
    minio.put_object(MINIO_BUCKET, path, aes_key_enc.file, length=length)
    # Append to shared_with
    # Lấy danh sách hiện tại
    record = supabase.table('files').select('shared_with').eq('file_id', file_id).single().execute().data
    shared_with = record.get('shared_with') or []

    if to_user not in shared_with:
        shared_with.append(to_user)
        supabase.table('files').update({
             'shared_with': shared_with
    }).eq('file_id', file_id).execute()

    return {'message': 'shared'}

# --- Endpoint: List all files current_user can access ---
@app.get('/list-files/')
async def list_files(current_user: str = Depends(verify_user)):
    rows = supabase.table('files') \
        .select('file_id, filename, owner, shared_with').execute().data
    out = []
    for r in rows:
        if r['owner'] == current_user:
            out.append({'file_id': r['file_id'], 'filename': r['filename'], 'role': 'owner'})
        elif current_user in r.get('shared_with', []):
            out.append({'file_id': r['file_id'], 'filename': r['filename'], 'role': 'shared'})
    return {'files': out}

# --- Endpoint: Download all encrypted components (AES file + key) ---
@app.get('/download-file/{file_id}')
async def download_file(
    file_id: str,
    current_user: str = Depends(verify_user)
):
    f = supabase.table('files').select('*').eq('file_id', file_id).single().execute().data
    if not f or (current_user != f['owner'] and current_user not in f.get('shared_with', [])):
        raise HTTPException(status_code=403, detail='No access')
    parts = ['file', 'nonce.bin', 'tag.bin', f'aes_key.{current_user}']
    def streamer():
        for p in parts:
            obj = minio.get_object(MINIO_BUCKET, f'file_{file_id}/{p}')
            yield obj.read()
    return StreamingResponse(streamer(), media_type='application/octet-stream')
