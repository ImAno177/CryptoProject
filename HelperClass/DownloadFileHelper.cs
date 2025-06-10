using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crypto.APIServices;

namespace Crypto.HelperClass
{
    internal class DownloadFileHelper
    {
        static public Task Download(string fileID, string Outputpath)
        {
            return DownloadFile.DownloadFileAsync(fileID, Outputpath);
        }
    }
}
