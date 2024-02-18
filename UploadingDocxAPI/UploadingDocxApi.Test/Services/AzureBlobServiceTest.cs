using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using UploadingDocxAPI.Interfaces;
using UploadingDocxAPI.Services;

namespace UploadingDocxApi.Test.Services;

public class AzureBlobServiceTest
{
    [Fact]
    public async Task AzureBlobService_UploadFiles_ReturnsBlobUri()
    {
        // Arrange
        var fakeBlobServiceClient = A.Fake<BlobServiceClient>();
        var fakeContainerClient = A.Fake<BlobContainerClient>();
        var fakeBlobClient = A.Fake<BlobClient>();

        A.CallTo(() => fakeBlobServiceClient.GetBlobContainerClient(A<string>._)).Returns(fakeContainerClient);
        A.CallTo(() => fakeContainerClient.GetBlobClient(A<string>._)).Returns(fakeBlobClient);

        var azureBlobService = new AzureBlobService(fakeBlobServiceClient, A.Fake<IConfiguration>());
        var fakeFile = A.Fake<IFormFile>();
        var fakeEmail = A.Fake<String>();

        // Act
        var result = await azureBlobService.UploadFiles(fakeFile, fakeEmail);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<string>(result);
    }

    [Fact]
    public async Task AzureBlobService_SetMetadata_ReturnsResponse()
    {
        // Arrange
        var fakeBlobServiceClient = A.Fake<BlobServiceClient>();
        var fakeContainerClient = A.Fake<BlobContainerClient>();
        var fakeBlobClient = A.Fake<BlobClient>();

        A.CallTo(() => fakeBlobServiceClient.GetBlobContainerClient(A<string>._)).Returns(fakeContainerClient);
        A.CallTo(() => fakeContainerClient.GetBlobClient(A<string>._)).Returns(fakeBlobClient);

        var azureBlobService = new AzureBlobService(fakeBlobServiceClient, A.Fake<IConfiguration>());

        // Act
        var result = await azureBlobService.SetMetadata("example.docx", "test@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<Response<BlobInfo>>(result);
    }

    [Fact]
    public async Task UploadFiles_WhenSettingMetadataFails_ShouldReturnNull()
    {
        // Arrange
        var fakeBlobServiceClient = A.Fake<BlobServiceClient>();
        var fakeContainerClient = A.Fake<BlobContainerClient>();
        var fakeBlobClient = A.Fake<BlobClient>();

        A.CallTo(() => fakeBlobServiceClient.GetBlobContainerClient(A<string>._)).Returns(fakeContainerClient);
        A.CallTo(() => fakeContainerClient.GetBlobClient(A<string>._)).Returns(fakeBlobClient);

        var fakeAzureBlobService = A.Fake<IAzureBlobService>(); // Use the interface
        A.CallTo(() => fakeAzureBlobService.SetMetadata("example.docx", "s2332fandrii@gmail.com"))
            .ThrowsAsync(new RequestFailedException("Service request fails."));

        // Assert
        await Assert.ThrowsAsync<RequestFailedException>(() =>
            fakeAzureBlobService.SetMetadata("example.docx", "s2332fandrii@gmail.com"));
    }
}



