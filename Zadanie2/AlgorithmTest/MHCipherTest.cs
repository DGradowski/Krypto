using Xunit;
using Algorithm;

namespace AlgorithmTests
{
    public class MHCipherTests
    {
        [Fact]
        public void EncryptWithCustom8bitPrivateKey()
        {
            var keyGen = new SimpleKeyGenerator(4571, 7187);
            long[] privateKey = { 2, 4, 8, 16, 32, 64, 128, 256 }; 
            var cipher = new MHCipher(keyGen, privateKey);
            string plainText = "Hello";
            string expectedCipher = "6442,15489,12139,12139,20962";

            string actualCipher = cipher.Encrypt(plainText);

            Assert.Equal(expectedCipher, actualCipher);
        }

        [Fact]
        public void DecryptWithCustom8bitPrivateKey()
        {
            var keyGen = new SimpleKeyGenerator(4571, 7187);
            long[] privateKey = { 2, 4, 8, 16, 32, 64, 128, 256 }; 
            var cipher = new MHCipher(keyGen, privateKey);
            string encrypted = "6442,15489,12139,12139,20962";
            string expectedPlainText = "Hello";

            string decrypted = cipher.Decrypt(encrypted);

            Assert.Equal(expectedPlainText, decrypted);
        }

        [Fact]
        public void EncryptWithCustom6bitPrivateKey()
        {
            var keyGen = new SimpleKeyGenerator(10, 439);
            long[] privateKey = { 3, 5, 15, 25, 54, 110 }; 
            var cipher = new MHCipher(keyGen, privateKey);
            string plainText = "World";
            string expectedCipher = "522,431,702,181,523,351,50";

            string actualCipher = cipher.Encrypt(plainText);

            Assert.Equal(expectedCipher, actualCipher);
        }

        [Fact]
        public void DecryptWithCustom6bitPrivateKey()
        {
            var keyGen = new SimpleKeyGenerator(10, 439);
            long[] privateKey = { 3, 5, 15, 25, 54, 110 };
            var cipher = new MHCipher(keyGen, privateKey);
            string encrypted = "522,431,702,181,523,351,50";
            string expectedPlainText = "World";

            string decrypted = cipher.Decrypt(encrypted);

            Assert.Equal(expectedPlainText, decrypted);
        }
    }
}
