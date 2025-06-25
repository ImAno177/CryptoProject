from alkindi import KEM, Signature

def gen_keys():
    # Kyber KEM
    with KEM("ML-KEM-1024") as kem:
        ky_pub_buf, ky_priv_buf = kem.generate_keypair()
    ky_pub = bytes(ky_pub_buf)
    ky_priv = bytes(ky_priv_buf)
    # Dilithium signature
    with Signature("ML-DSA-44") as sig:
        d_pub_buf, d_sk = sig.generate_keypair()
        d_pub = bytes(d_pub_buf)
        ky_sig = bytes(sig.sign(ky_pub, d_sk))
        assert sig.verify(ky_pub, ky_sig, d_pub), "Signature verify failed"
    return ky_pub.hex(), d_pub.hex(), ky_sig.hex(), ky_priv.hex()

if __name__ == "__main__":
    ky_pub, d_pub, ky_sig, ky_priv = gen_keys()
    print(ky_priv)
    print(ky_pub)
    print(d_pub)
    print(ky_sig)