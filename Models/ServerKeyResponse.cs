using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Models
{
    internal class ServerKeyResponse
    {
        public string server_kyber_pk_hex { get; set; }
        public string session_id { get; set; }
    }
}
