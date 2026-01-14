using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RND_API.Services;

namespace RND_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FtpServerController : ControllerBase
    {
        [HttpPost()]
        [Consumes("multipart/form-data")]
        [Route("FileUploadInFtpServer")]
        public async Task<IActionResult> FileUploadInFtpServer(
           IFormFile file,
           [FromServices] FtpService ftp)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File missing");

            var remotePath = $"/uploads/{file.FileName}";
            await ftp.UploadAsync(file, remotePath);

            return Ok("Uploaded successfully");
        }

        [HttpGet()]
        [Route("FileDownloadFromFtpServer")]
        public async Task<IActionResult> FileDownloadFromFtpServer(
            string fileName,
            [FromServices] FtpService ftp)
        {
            var remotePath = $"/uploads/{fileName}";
            var fileBytes = await ftp.DownloadAsync(remotePath);

            return File(fileBytes, "application/octet-stream", fileName);
        }
    }
}
