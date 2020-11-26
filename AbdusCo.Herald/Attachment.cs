using System.IO;

namespace AbdusCo.Herald
{
    public class Attachment
    {
        public FileInfo File { get; private set; }
        public string Name { get; private set; }
        public Stream Stream { get; private set; }
        public string Mime { get; private set; }


        private Attachment()
        {
        }

        /// <summary>
        /// Create attachment from a file on disk
        /// </summary>
        /// <param name="file">File to attach</param>
        /// <param name="attachmentName">Optional, filename is used by default</param>
        /// <returns></returns>
        public static Attachment FromFile(FileInfo file, string attachmentName = null)
        {
            return new Attachment
            {
                File = file,
                Name = attachmentName ?? file.Name,
                Mime = MimeHelper.GetMimeType(file.Extension)
            };
        }

        public static Attachment FromStream(Stream stream, string attachmentName, string mimeType = null)
        {
            return new Attachment
            {
                Stream = stream,
                Name = attachmentName,
                Mime = MimeHelper.GetMimeType(new FileInfo(attachmentName).Extension),
            };
        }
    }
}