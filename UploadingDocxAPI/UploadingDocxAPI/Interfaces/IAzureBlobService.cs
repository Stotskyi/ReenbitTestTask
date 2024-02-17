using Azure;
using Azure.Storage.Blobs.Models;

namespace UploadingDocxAPI.Interfaces;

public interface IAzureBlobService
{ 
    Task<String> UploadFiles(IFormFile file, string email);
    Task<Response<BlobInfo>> SetMetadata(string fileName, string email);
}