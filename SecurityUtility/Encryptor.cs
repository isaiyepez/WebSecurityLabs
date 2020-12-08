using System;
using System.Security.Cryptography;
using System.Text;

namespace SecurityUtility
{
    /// <summary>
    /// The Encryptor class provides an example of a cryptography wrapper.  This class 
    /// is provided as an example only.  In production, this class should acquire keys, salts and 
    /// initialization vectors from secure sources.  
    /// As a wrapper, however, the application is isolated from the underlying crypto library, 
    /// key sources, and algorithms.
    /// </summary>
    public class Encryptor
    {
        private byte[] key;
        private SymmetricAlgorithm cipher = null;

        public Encryptor() { }

        /// <summary>
        /// Generate a key from password
        /// </summary>
        /// <param name="password">used to generate key</param>
        public Encryptor(string password)
        {
            SetKeyFromPassword(password);
        }

        /// <summary>
        /// Creates a key from the password
        /// </summary>
        /// <param name="password"></param>
        public void SetKeyFromPassword(string password)
        {
            // TODO salt and iv should not be hard coded
            byte[] iv = { 10, 31, 244, 101, 53, 13, 7, 3 };
            var salt = password.ToUpper() + password.ToLower() + (password.Length * 31);
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(password,
                Encoding.Default.GetBytes(salt));
            Key = pdb.CryptDeriveKey("TripleDES", "SHA256", 192, iv);
        }

        /// <summary>
        /// AES Key
        /// </summary>
        public byte[] Key
        {
            get
            {
                if (key == null)
                {
                    key = Random();
                }
                return key;
            }
            set { key = value; cipher = null; }
        }

        /// <summary>
        /// Creates an AES cipher if one has not already been created.
        /// Uses a fixed IV - should not be used in production.
        /// </summary>
        public SymmetricAlgorithm Cipher
        {
            get
            {
                if (cipher == null)
                {
                    cipher = SymmetricAlgorithm.Create("Aes");  // or Aes.Create()
                    cipher.Padding = PaddingMode.ISO10126;
                    cipher.Key = Key;
                    // in production, the IV should be set from application
                    cipher.IV = new byte[] { 202, 115, 8, 12, 3, 31, 233, 113,
                        22, 15, 88, 102, 39, 21, 23, 13};
                }
                return cipher;
            }
        }

        /// <summary>
        /// Encrypt using AES.
        /// </summary>
        /// <param name="plainText">text to be encrypted</param>
        /// <returns>base 64 encoded encrypted text</returns>
        public string Encrypt(string plainText)
        {
            var enc = Cipher.CreateEncryptor();
            var bytes = Encoding.Default.GetBytes(plainText);
            var encbytes = enc.TransformFinalBlock(bytes, 0, bytes.Length);
            return Convert.ToBase64String(encbytes);

        }

        /// <summary>
        /// Decrypt using AES.  Set the key and IV for the Cipher property to the same
        /// values used to encrypt the data
        /// </summary>
        /// <param name="cipherText">base 64 encoded encrypted value</param>
        /// <returns>unencrypted text</returns>
        public string Decrypt(string cipherText)
        {
            var dec = Cipher.CreateDecryptor();
            var bytes = Convert.FromBase64String(cipherText);
            var res = dec.TransformFinalBlock(bytes, 0, bytes.Length);
            return Encoding.Default.GetString(res);
        }

        /// <summary>
        /// Computes a hash from plainText using salt and SHA 512.
        /// The plaintext parameter is salted with salt and then hashed 1024 times
        /// to obscure from dictionary lookups.
        /// </summary>
        /// <param name="plainText">text to compute has from</param>
        /// <param name="salt">any bytes that are used to obscure the plaintext</param>
        /// <returns>base 64 encoded hash</returns>
        public string Hash(string plainText, byte[] salt)
        {
            var hasher = SHA512.Create();
            var tsalt = Convert.ToBase64String(salt);
            plainText = tsalt + plainText + tsalt;
            byte[] hash = hasher.ComputeHash(Encoding.Default.GetBytes(plainText));
            for (int i = 0; i < 1024; i++)
                hash = hasher.ComputeHash(hash);

            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Given plainText and salt, checks that the hash matches
        /// </summary>
        /// <param name="plainText">text to check</param>
        /// <param name="salt">salt used in original hash</param>
        /// <param name="hash">original hash, base64 encoded</param>
        /// <returns></returns>
        public bool VerifyHash(string plainText, byte[] salt, string hash)
        {
            var test = Hash(plainText, salt);
            return test == hash;
        }

        /// <summary>
        /// returns a cyptographically strong random number of size bytes
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public byte[] Random(int size = 32)
        {
            var bytes = new byte[size];
            RandomNumberGenerator.Create().GetBytes(bytes);
            return bytes;
        }
    }
}
