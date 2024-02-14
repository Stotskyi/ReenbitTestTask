using Azure.Storage.Blobs;

namespace UploadingDocxAPI.Services;

public class AzureBlobService
{
    private readonly  BlobServiceClient _blobCliet;
    private readonly BlobContainerClient _containerClient;
    
    public AzureBlobService(BlobServiceClient blobCliet,IConfiguration configuration)
    {
        _blobCliet = blobCliet;
        _containerClient = _blobCliet.GetBlobContainerClient(configuration.GetValue<string>("BlobContainerName"));
    }

    public async Task<String> UploadFiles(IFormFile file)
    {
        BlobClient client = _containerClient.GetBlobClient(file.FileName);
        await using (MemoryStream data = new MemoryStream())
        {
            file.CopyTo(data);
            data.Position = 0;
            var response = await  _containerClient.UploadBlobAsync(file.FileName, data, default);
            return response.ToString();
        }

    }
}