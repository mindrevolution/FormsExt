using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Umbraco.Core.Macros;

namespace FormsExt.Xslt
{
    [XsltExtension]
    public class Library
    { 
        public static string GenerateTrackingToken(string email)
        {
            return HashSHA256(System.Configuration.ConfigurationManager.AppSettings["FormsExt:FormsExt:TrackingTokenSalt"] + email);
        }
        public static string HashSHA256(string plaintext)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(plaintext));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }

        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int Keysize = 256;
        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;

        public static string EncryptUrlEncoded(string plaintext)
        {
            return System.Web.HttpContext.Current.Server.UrlEncode(Encrypt(plaintext));
        }
        public static string Encrypt(string plaintext)
        {
            return Encrypt(plaintext, System.Configuration.ConfigurationManager.AppSettings["FormsExt:Passphrase"]);
        }
        public static string Encrypt(string plaintext, string passphrase)
        {
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
            using (var password = new Rfc2898DeriveBytes(passphrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plaintextBytes, 0, plaintextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var ciphertextBytes = saltStringBytes;
                                ciphertextBytes = ciphertextBytes.Concat(ivStringBytes).ToArray();
                                ciphertextBytes = ciphertextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(ciphertextBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string ciphertext)
        {
            return Decrypt(ciphertext, System.Configuration.ConfigurationManager.AppSettings["FormsExt:Passphrase"]);
        }
        public static string Decrypt(string ciphertext, string passphrase)
        {
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of ciphertext]
            var ciphertextBytesWithSaltAndIv = Convert.FromBase64String(ciphertext);
            // Get the saltbytes by extracting the first 32 bytes from the supplied ciphertext bytes.
            var saltStringBytes = ciphertextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied ciphertext bytes.
            var ivStringBytes = ciphertextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the ciphertext string.
            var ciphertextBytes = ciphertextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(ciphertextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passphrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(ciphertextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plaintextBytes = new byte[ciphertextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plaintextBytes, 0, plaintextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plaintextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }

    }
}
