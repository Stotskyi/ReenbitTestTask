using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace UploadingDocxAPI.Services;

public class AzureBlobService
{
    private readonly  BlobServiceClient _blobCliet;
    private readonly BlobContainerClient _containerClient;
    
    public AzureBlobService(BlobServiceClient blobClient,IConfiguration configuration)
    {
        _blobCliet = blobClient;
        _containerClient = _blobCliet.GetBlobContainerClient(configuration.GetValue<string>("BlobContainerName"));
    }

    public async Task<String> UploadFiles(IFormFile file)
    {
        BlobClient client = _containerClient.GetBlobClient(file.FileName);
        var streamContent = file.OpenReadStream();
        var response = await  _containerClient.UploadBlobAsync(file.FileName, streamContent, default);
        return response.ToString();
        await using (MemoryStream data = new MemoryStream())
        {
            file.CopyTo(data);
            data.Position = 0;
            
        }
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
            Console.WriteLine($"HTTP error code {e.Status}: {e.ErrorCode}");
        }

        return null;
    }
}