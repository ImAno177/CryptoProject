using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Models
{
    internal class DownloadInitiateResponse
    {
        public string download_token { get; set; }
        public string kyber_encapsulated_aes_key_hex { get; set; }
        public string aes_nonce_hex { get; set; }
        public string aes_tag_hex { get; set; }
    }
}
