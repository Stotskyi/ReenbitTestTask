using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UploadingDocxAPI.Controllers;
using UploadingDocxAPI.Services;

namespace UploadingDocxApi.Test;

public class AttachmentControllerTest
{
    [Fact]
    public async Task AttachmentController_UploadFile_ReturnSuccess()
    {
        var fakeService = A.Fake<AzureBlobService>();
        var controller = new AttachmentController(fakeService);
        var file = A.Fake<IFormFile>();
        var email = "test@example.com";

        // Act
        var result = await controller.UploadFile(file, email);

        // Assert
        Assert.IsType<OkResult>(result);
    }
}