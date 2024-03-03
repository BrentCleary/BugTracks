using BugTracks.Services.Interfaces;

namespace BugTracks.Services
{
    public class BTFileService : IBTFileService
    {
        private readonly string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };

        public string ConvertByteArrayToFile(byte[] fileData, string extension)
        {
            try
            {
                string imageBased64Data = Convert.ToBase64String(fileData);
                return string.Format($"data:{extension}; base64,{imageBased64Data}");
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            throw new NotImplementedException();
        }

        public string FormatFileSize(long bytes)
        {
            throw new NotImplementedException();
        }

        public string GetFileIcon(string file)
        {
            throw new NotImplementedException();
        }
    }
}
