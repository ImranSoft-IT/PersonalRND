using FluentFTP;
using System.Net;

namespace RND_API.Services
{
    public class FtpService
    {
        private readonly IConfiguration _config;

        public FtpService(IConfiguration config)
        {
            _config = config;
        }

        private AsyncFtpClient CreateClient()
        {
            var config = new FtpConfig
            {
                DataConnectionType = FtpDataConnectionType.AutoPassive,
                RetryAttempts = 3
            };

            var client = new AsyncFtpClient(
                host: _config["FtpSettings:Host"],
                credentials: new NetworkCredential(
                    _config["FtpSettings:Username"],
                    _config["FtpSettings:Password"]
                ),
                config: config
            );
            return client;
        }

        public async Task UploadAsync(IFormFile file, string remotePath)
        {
            await using var client = CreateClient();
            await client.Connect();

            await using var stream = file.OpenReadStream();
            await client.UploadStream(stream, remotePath, FtpRemoteExists.Overwrite);
        }

        public async Task<byte[]> DownloadAsync(string remotePath)
        {
            await using var client = CreateClient();
            await client.Connect();

            await using var ms = new MemoryStream();
            await client.DownloadStream(ms, remotePath);
            return ms.ToArray();
        }
    }

}
