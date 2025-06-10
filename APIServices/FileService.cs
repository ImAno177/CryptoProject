using Crypto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.APIServices
{
    internal class FileService
    {
        private readonly HttpClient httpClient;
        public FileService()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://fastapi.crypto-lab.cloud/");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<FileListItem>> GetFilesAsync()
        {
            HttpResponseMessage response = await httpClient.GetAsync("files");
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<FileListItem>>(json);
        }
    }
}
