using Algorithm;

namespace AlgorithmTest
{
    public class MHCipherTest()
    {
        [Fact]
        public void encryptDecryptTest()
        {
            var keyGen = new SimpleKeyGenerator(); // domyślny mnożnik i modulo
            var cipher = new MHCipher(keyGen);
            string message = "Hello World!";

            long[] encrypted = cipher.Encrypt(message);
            string decrypted = cipher.decrypt(encrypted);

            Assert.Equal(message, decrypted);
        }

        [Fact]
        public void encryptTest()
        {
            var keyGen = new SimpleKeyGenerator(4571, 7187);
            var cipher = new MHCipher(keyGen);
            string message = "A";

            long[] publicKey = keyGen.generatePublicKey(keyGen.generateDefaultPrivateKey());

            List<long> expected = new();

            foreach (char ch in message)
            {
                int val = ch;
                long sum = 0;
                for (int i = 0; i < 8; i++)
                {
                    if ((val & 1) == 1)
                        sum += publicKey[i];
                    val >>= 1;
                }
                expected.Add(sum);
            }

            long[] actual = cipher.Encrypt(message);

            Assert.Equal(expected.Count, actual.Length);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i], actual[i]);
            }
        }
    }
}
