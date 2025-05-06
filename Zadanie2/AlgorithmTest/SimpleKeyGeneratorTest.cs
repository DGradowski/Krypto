using Algorithm;

namespace AlgorithmTest
{
    public class SimpleKeyGeneratorTest
    {
        [Fact]
        public void CorretLengthTest()
        {
            var keyGen = new SimpleKeyGenerator();
            long[] privateKey = keyGen.generateDefaultPrivateKey();
            long[] publicKey = keyGen.generatePublicKey(privateKey);
            Assert.Equal(8, privateKey.Length);
        }
    }
}