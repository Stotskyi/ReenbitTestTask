using Microsoft.AspNetCore.Mvc;
using UploadingDocxAPI.Services;

namespace UploadingDocxAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AttachmentController : ControllerBase
{
    private readonly AzureBlobService _service;

    public AttachmentController(AzureBlobService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        var response = await _service.UploadFiles(file);
        return Ok(response);
    } 
}