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

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            try
            {
                MemoryStream memoryStream = new();
                // Copy to Async is a method of the IFormFile Interface
                await file.CopyToAsync(memoryStream);
                // ToArray converts the memoryStream variable to a byte array for storage
                byte[] byteFile = memoryStream.ToArray();

                // Clears the memoryStream
                memoryStream.Close();
                memoryStream.Dispose();

                return byteFile;

            }
            catch (Exception)
            {

                throw;
            }
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
