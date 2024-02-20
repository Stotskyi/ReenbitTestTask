using Microsoft.AspNetCore.Mvc;
using UploadingDocxAPI.Interfaces;
using UploadingDocxAPI.Services;

namespace UploadingDocxAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AttachmentController : ControllerBase
{
    private readonly IAzureBlobService _service;

    public AttachmentController(IAzureBlobService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile file,[FromForm] string email)
    {
        var response = await _service.UploadFilesAsync(file, email);
        return Ok(response);
    }
   
}