using Azure;
using Azure.Storage.Blobs.Models;

namespace UploadingDocxAPI.Interfaces;

public interface IAzureBlobService
{ 
    Task<String> UploadFilesAsync(IFormFile file, string email);
    Task<Response<BlobInfo>> SetMetadataAsync(string fileName, string email);
}