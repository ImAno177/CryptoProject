using Crypto.APIServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.HelperClass
{
    internal class UploadFileHelper
    {
        static public Task Upload(string InputPath, string Filename)
        {
            return UploadFile.UploadFileAsync(InputPath, Filename);
        }
    }
}
