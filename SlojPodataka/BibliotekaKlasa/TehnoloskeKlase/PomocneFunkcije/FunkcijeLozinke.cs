using System.Security.Cryptography;
using System.Text;

namespace BibliotekaKlasa.TehnoloskeKlase.PomocneFunkcije
{
    public static class FunkcijeLozinke
    {
        public static string GenerisiSalt(int duzina = 16)
        {
            byte[] saltBajtovi = new byte[duzina];
            using var randomBroj = RandomNumberGenerator.Create();
            randomBroj.GetBytes(saltBajtovi);
            return Convert.ToBase64String(saltBajtovi);
        }

        public static string IzracunajHash(string lozinka, string salt)
        {
            using var sha256 = SHA256.Create();
            var saltovanaLozinka = lozinka + salt;
            byte[] hashBajtovi = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltovanaLozinka));
            return Convert.ToBase64String(hashBajtovi);
        }

        public static bool ProveriLozinku(string lozinka, string salt, string hash)
        {
            return IzracunajHash(lozinka, salt) == hash;
        }
    }
}
