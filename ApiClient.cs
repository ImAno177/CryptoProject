using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json; // Needs System.Net.Http.Json NuGet
using System.Text.Json.Serialization; // For attributes if needed
using System.Threading.Tasks;

// --- Request/Response Models (matching server Pydantic models) ---
public class UploadInitiateResponse
{
    [JsonPropertyName("session_id")]
    public string SessionId { get; set; }
    [JsonPropertyName("server_kyber_pk_hex")]
    public string ServerKyberPkHex { get; set; }
}

public class UploadResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; }
    [JsonPropertyName("file_id")]
    public string FileId { get; set; }
}

public class DownloadInitiateRequest
{
    [JsonPropertyName("client_kyber_pk_hex")]
    public string ClientKyberPkHex { get; set; }
}

public class DownloadInitiateResponse
{
    [JsonPropertyName("download_token")]
    public string DownloadToken { get; set; }
    [JsonPropertyName("kyber_encapsulated_aes_key_hex")]
    public string KyberEncapsulatedAesKeyHex { get; set; }
    [JsonPropertyName("aes_nonce_hex")]
    public string AesNonceHex { get; set; }
    [JsonPropertyName("aes_tag_hex")]
    public string AesTagHex { get; set; }
}

public class FileListItem
{
    [JsonPropertyName("file_id")]
    public string FileId { get; set; }
    [JsonPropertyName("original_filename")]
    public string OriginalFilename { get; set; }
    [JsonPropertyName("size")]
    public long Size { get; set; } // Changed to long for C#
    [JsonPropertyName("uploaded_at")]
    public string UploadedAt { get; set; }
}


public class ApiClient
{
    private readonly HttpClient _httpClient;
    private string _baseUrl;

    public ApiClient(string baseUrl)
    {
        _httpClient = new HttpClient();
        _baseUrl = baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/";
    }

    public void SetBaseUrl(string baseUrl)
    {
        _baseUrl = baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/";
    }

    public async Task<List<FileListItem>> ListFilesAsync()
    {
        var response = await _httpClient.GetAsync(_baseUrl + "files");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<FileListItem>>();
    }

    public async Task<UploadInitiateResponse> InitiateUploadAsync()
    {
        var response = await _httpClient.PostAsync(_baseUrl + "upload/initiate", null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UploadInitiateResponse>();
    }

    public async Task<UploadResponse> UploadFileAsync(
        string sessionId, string kyberEncapsulatedAesKeyHex, string aesNonceHex, string aesTagHex,
        string plaintextSha256ChecksumHex, string filePath, string originalFileName)
    {
        using var multipartFormContent = new MultipartFormDataContent();
        multipartFormContent.Add(new StringContent(sessionId), "session_id");
        multipartFormContent.Add(new StringContent(kyberEncapsulatedAesKeyHex), "kyber_encapsulated_aes_key_hex");
        multipartFormContent.Add(new StringContent(aesNonceHex), "aes_nonce_hex");
        multipartFormContent.Add(new StringContent(aesTagHex), "aes_tag_hex");
        multipartFormContent.Add(new StringContent(plaintextSha256ChecksumHex), "plaintext_sha256_checksum_hex");

        var fileStreamContent = new StreamContent(File.OpenRead(filePath)); // This should be the *encrypted* file
        fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        multipartFormContent.Add(fileStreamContent, name: "file", fileName: originalFileName);

        var response = await _httpClient.PostAsync(_baseUrl + "upload", multipartFormContent);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UploadResponse>();
    }

    public async Task<DownloadInitiateResponse> InitiateDownloadAsync(string fileId, string clientKyberPkHex)
    {
        var requestPayload = new DownloadInitiateRequest { ClientKyberPkHex = clientKyberPkHex };
        var response = await _httpClient.PostAsJsonAsync(_baseUrl + $"download/initiate/{fileId}", requestPayload);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DownloadInitiateResponse>();
    }

    // Downloads the encrypted file stream to the specified output path
    public async Task DownloadFileStreamAsync(string downloadToken, string outputFilePath)
    {
        var response = await _httpClient.GetAsync(_baseUrl + $"download/{downloadToken}", HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        using (var stream = await response.Content.ReadAsStreamAsync())
        using (var fileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await stream.CopyToAsync(fileStream);
        }
    }
}