using Xunit;
using Algorithm;

namespace Algorithm.Tests
{
    public class MHCipherTests
    {
        [Fact]
        public void EncryptWithCustom8bitPrivateKey()
        {
            var keyGen = new SimpleKeyGenerator(10, 439);
            long[] privateKey = { 3, 5, 15, 25, 54, 110, 225 };
            var cipher = new MHCipher(keyGen, privateKey);
            string plainText = "hello";
            string expectedCipher = "280,422,707,406,230";

            string actualCipher = cipher.Encrypt(plainText);

            Assert.Equal(expectedCipher, actualCipher);
        }

        [Fact]
        public void DecryptWithCustom8bitPrivateKey()
        {
            var keyGen = new SimpleKeyGenerator(10, 439);
            long[] privateKey = { 3, 5, 15, 25, 54, 110, 225 };
            var cipher = new MHCipher(keyGen, privateKey);
            string encrypted = "280,422,707,406,230";
            string expectedPlainText = "Hello";

            string decrypted = cipher.Decrypt(encrypted);

            Assert.Equal(expectedPlainText, decrypted);
        }

    }
}