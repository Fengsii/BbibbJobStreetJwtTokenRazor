using BbibbJobStreetJwtToken.Interfaces;

namespace BbibbJobStreetJwtToken.Helpers
{
    public class FileHelper : IFileHelper
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FileHelper(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> UploadPdfAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File tidak boleh kosong");

            // Validasi tipe file
            if (Path.GetExtension(file.FileName).ToLower() != ".pdf")
                throw new ArgumentException("Hanya file PDF yang diperbolehkan");

            // Pastikan folder tujuan ada
            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, folderName);
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // Nama file unik
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadPath, uniqueFileName);

            // Simpan file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return uniqueFileName; // simpan nama ini di DB
        }

        public async Task<byte[]> DownloadFileAsync(string fileName, string folderName)
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, folderName, fileName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File tidak ditemukan");

            return await File.ReadAllBytesAsync(filePath);
        }


    }
}
