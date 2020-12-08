using NUnit.Framework;
using System;
using System.Security.Cryptography;

namespace SecurityUtility
{
    public class EncryptorTest
    {
        private Encryptor sut;
        [SetUp]
        public void Setup()
        {
            sut = new Encryptor("KeyPassword");
        }

        [Test]
        public void TestEncryptorKeyValid()
        {
            Assert.IsNotNull(sut.Key);
        }

        [Test]
        public void TestEncryptDecryptValid()
        {
            var text = "This is a test";
            var result = sut.Encrypt(text);
            Assert.AreEqual(text, sut.Decrypt(result));
        }
        [Test]
        public void TestEncryptDecryptInvalid()
        {
            var text = "This is a test";
            var result = sut.Encrypt(text);
            var sut2 = new Encryptor("DifferentKey");
            Assert.Throws<CryptographicException>(() => sut2.Decrypt(result));
        }

        [Test]
        public void TestHashCodeValid()
        {
            var text = "This is a test";
            var salt = sut.Random();
            var result = sut.Hash(text, salt);
            Assert.IsTrue(sut.VerifyHash(text, salt, result));
        }
        [Test]
        public void TestHashCodeInvalidSalt()
        {
            var text = "This is a test";
            var salt = sut.Random();
            var result = sut.Hash(text, salt);
            Assert.IsFalse(sut.VerifyHash(text, sut.Random(), result));
        }
        [Test]
        public void TestHashCodeInvalidText()
        {
            var text = "This is a test";
            var salt = sut.Random();
            var result = sut.Hash(text, salt);
            Assert.IsFalse(sut.VerifyHash("Different Text", salt, result));
        }

    }
}