namespace TheBlogProject.Services
{
    public class BasicImageService : IImageService
    {
        public async Task<byte[]> EncodeImageAsync(IFormFile file)
        {
            if (file == null) return null;
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        public async Task<byte[]> EncodeImageAsync(string fileName)
        {
            var file = $"{Directory.GetCurrentDirectory()}/wwwroot/images/{fileName}";
            return await File.ReadAllBytesAsync(fileName);
        }

        public string DecodeImage(byte[] data, string type)
        {
            if (data is null || type is null) return null;
            return $"data:image/{type};base64,{Convert.ToBase64String(data)}";
        }

        public string ContentType(IFormFile file)
        {
            return file?.ContentType;
        }

        public int Size(IFormFile file)
        {
            return Convert.ToInt32(file?.Length);
        }
    }
}
