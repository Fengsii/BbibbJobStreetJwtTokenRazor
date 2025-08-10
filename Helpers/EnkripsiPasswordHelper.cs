using BbibbJobStreetJwtToken.Interfaces;

namespace BbibbJobStreetJwtToken.Helpers
{
    public class EnkripsiPasswordHelper : IEnkripsiPassword
    {
        // Method untuk hash password menggunakan SHA256 (sederhana untuk contoh)
        // Untuk production, gunakan BCrypt atau Argon2

        //private string HashPassword(string password)
        //{
        //    return BCrypt.Net.BCrypt.HashPassword(password);
        //}

        //private bool VerifyPassword(string password, string hashedPassword)
        //{
        //    return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        //}



        //Method untuk hash password menggunakan BCrypt
        // Ubah dari private ke public static agar bisa diakses dari kelas lain
        //public static string HashPassword(string password)
        //{
        //    return BCrypt.Net.BCrypt.HashPassword(password);
        //}

        //// Method untuk verifikasi password
        //// Ubah dari private ke public static agar bisa diakses dari kelas lain
        //public static bool VerifyPassword(string password, string hashedPassword)
        //{
        //    return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        //}



        // Instance methods untuk DI
        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password tidak boleh kosong", nameof(password));

            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
                return false;

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch
            {
                return false;
            }
        }

        // Static methods tetap ada untuk backward compatibility
        public static string HashPasswordStatic(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPasswordStatic(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }


    }
}
