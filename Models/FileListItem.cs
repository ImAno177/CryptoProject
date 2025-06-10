using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Models
{
    internal class FileListItem
    {
        public string file_id { get; set; }
        public string original_filename { get; set; }
        public long size { get; set; }
        public string uploaded_at { get; set; }
    }
}
