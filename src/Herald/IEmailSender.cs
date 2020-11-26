using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Herald
{
    public interface IEmailSender
    {
        public Task<SendResponse> SendAsync(Email email, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Saves email to disk, useful for debugging purposes
    /// </summary>
    public class FilesystemEmailSender : IEmailSender
    {
        private readonly string _emailDumpDir;
        private readonly IRenderer _renderer;

        public FilesystemEmailSender(string emailDumpDir, IRenderer renderer)
        {
            _emailDumpDir = emailDumpDir;
            _renderer = renderer;
        }

        public async Task<SendResponse> SendAsync(Email email, CancellationToken cancellationToken = default)
        {
            var basename = GenerateFilename(email);
            var path = Path.Join(_emailDumpDir, basename);

            var contents = await email.RenderBodyAsync(_renderer, cancellationToken);
            await File.WriteAllTextAsync(path, contents, cancellationToken);

            return SendResponse.Success;
        }

        private string GenerateFilename(Email email)
        {
            var now = DateTime.UtcNow.ToString("s").Replace(":", "").Replace("-", "");
            var random = Guid.NewGuid().ToString().Substring(0, 6);
            var basename = $"{now}.{random}.{email.Subject ?? "nosubject"}.txt";
            return basename;
        }
    }
}