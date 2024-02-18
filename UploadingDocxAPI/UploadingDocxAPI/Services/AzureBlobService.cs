using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using UploadingDocxAPI.Interfaces;

namespace UploadingDocxAPI.Services;

public class AzureBlobService : IAzureBlobService
{
    private readonly  BlobServiceClient _blobCliet;
    private readonly BlobContainerClient _containerClient;
    
    public AzureBlobService(BlobServiceClient blobClient,IConfiguration configuration)
    {
        _blobCliet = blobClient;
        _containerClient = _blobCliet.GetBlobContainerClient(configuration.GetValue<string>("BlobContainerName"));
    }

    public async Task<String> UploadFiles(IFormFile file, string email)
    {
        BlobClient client = _containerClient.GetBlobClient(file.FileName);
        var streamContent = file.OpenReadStream();
        var responseFromUploading = await _containerClient.UploadBlobAsync(file.FileName, streamContent, default);
        var responseFromSettingMetadata = await SetMetadata(file.FileName, email);

        return $"{responseFromUploading}\n{responseFromSettingMetadata}";
    }

    public async Task<Response<BlobInfo>> SetMetadata(string fileName,string email)
    {
        BlobClient client = _containerClient.GetBlobClient(fileName);

        try
        {
            IDictionary<string, string> metadata =
                new Dictionary<string, string>();

            metadata.Add("email", email);
            return  await client.SetMetadataAsync(metadata);
        }
        catch (RequestFailedException e)
        {
            throw new RequestFailedException("Service request fails.");
        }

        return null;
    }
}