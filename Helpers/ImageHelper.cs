using BbibbJobStreetJwtToken.Interfaces;

namespace BbibbJobStreetJwtToken.Helpers
{
    public class ImageHelper : IImageHelper
    {
        private readonly string _webRootPath;
        private readonly long _maxSizeInBytes = 5 * 1024 * 1024; // 5MB
        private readonly string[] _allowedExtensions = [".jpg", ".jpeg", ".png"];

        public ImageHelper(IWebHostEnvironment env)
        {
            _webRootPath = env.WebRootPath;
        }

        public string Save(IFormFile file, string folder = "uploads")
        {
            if (!IsValidFile(file, out string validationMessage))
            {
                throw new InvalidOperationException(validationMessage);
            }

            var folderPath = Path.Combine(_webRootPath, folder);
            Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            file.CopyTo(stream);

            return Path.Combine(folder, fileName).Replace("\\", "/");
        }

        public bool Delete(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return false;

            var fullPath = Path.Combine(_webRootPath, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (!File.Exists(fullPath)) return false;

            File.Delete(fullPath);
            return true;
        }

        private bool IsValidFile(IFormFile file, out string message)
        {
            message = "";

            if (file == null || file.Length == 0)
            {
                message = "File kosong atau tidak ditemukan.";
                return false;
            }

            if (file.Length > _maxSizeInBytes)
            {
                message = "Ukuran file melebihi 5MB.";
                return false;
            }

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedExtensions.Contains(extension))
            {
                message = "Format file tidak didukung. Hanya JPG, JPEG, dan PNG yang diperbolehkan.";
                return false;
            }

            return true;
        }

    }
}
